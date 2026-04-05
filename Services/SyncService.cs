using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Diagnostics;
using WeighForce.Data;
using WeighForce.Models;
using Microsoft.Extensions.DependencyInjection;
using WeighForce.Dtos;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using ShellProgressBar;

namespace WeighForce.Services
{
    public class UpdateCheck<T> where T : class
    {
        public T Value { get; set; }
        public bool Update { get; set; }
    }

    public class MatchResult
    {
        public bool Found { get; set; }
        public bool Created { get; set; }
        public bool Prefilled { get; set; }
        public string Direction { get; set; }
    }
    class Updateables
    {
        public List<ProductWriteDto> Products { get; set; } = new List<ProductWriteDto>();
        public List<ContractWriteDto> Contracts { get; set; } = new List<ContractWriteDto>();
        public List<CountryWriteDto> Countries { get; set; } = new List<CountryWriteDto>();
        public List<UnitWriteDto> Units { get; set; } = new List<UnitWriteDto>();
        public List<OfficeWriteDto> Offices { get; set; } = new List<OfficeWriteDto>();
        public List<BranchWriteDto> Branches { get; set; } = new List<BranchWriteDto>();
        public List<TIWriteDto> TransportInstructions { get; set; } = new List<TIWriteDto>();
        public List<TransporterWriteDto> Transporters { get; set; } = new List<TransporterWriteDto>();
        public List<DispatchWriteDto> Dispatches { get; set; } = new List<DispatchWriteDto>();
    }
    class Updates
    {
        public List<ProductSyncDto> Products { get; set; }
        public List<ContractSyncDto> Contracts { get; set; }
        public List<CountrySyncDto> Countries { get; set; }
        public List<UnitSyncDto> Units { get; set; }
        public List<OfficeSyncDto> Offices { get; set; }
        public List<BranchSyncDto> Branches { get; set; }
        public List<TISyncDto> TransportInstructions { get; set; }
        public List<TransporterSyncDto> Transporters { get; set; }
        public List<DispatchFullSyncDto> Dispatches { get; set; }
        public List<EventLogDto> EventLogs { get; set; }
        public List<TIChangeDto> TIChanges { get; set; }
    }

    public class SyncService : IHostedService
    {
        private Timer _timer;
        int running = 0;
        private const long TimerInterval = 120000;
        private readonly CrmService _crmService;
        private readonly UserService _userService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly SyncStatus _status;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private DateTime _startDate;
        private DateTime _initialStartDate;
        private DateTime _syncStartTime;
        private DateTime _endDate;
        private Meta _lastUpdateMeta;
        private bool _init = true;
        private ApplicationDbContext _context;
        private OnTractContext _otContext;
        private readonly ILogger<SyncService> _logger;
        private bool _authSync = false;
        private readonly IWebHostEnvironment _env;
        bool _syncError = false;
        private readonly bool _verbose;
        private readonly bool _benchmark;
        ProgressBarOptions options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Yellow,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '='
        };
        ProgressBarOptions childOptions = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkGreen,
            ProgressCharacter = '.'
        };

        public SyncService(CrmService crmService, IServiceScopeFactory scopeFactory, IMapper mapper, IConfiguration configuration, ILogger<SyncService> logger, SyncStatus status, UserService userService, IWebHostEnvironment env)
        {
            _crmService = crmService;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _status = status;
            _userService = userService;
            _env = env;
            _verbose = _configuration.GetValue("Sync:Verbose", false);
            _benchmark = _configuration.GetValue("Sync:Benchmark", false);
        }

        public async Task Sync()
        {
            using var scope = _scopeFactory.CreateScope();
            _syncError = false;

            _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                if (Interlocked.CompareExchange(ref running, 1, 0) != 0)
                    return;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                var serverStatus = await _crmService.GetLock();
                if (!serverStatus && _init)
                {
                    _init = false;
                    Console.WriteLine("Sync Failed. Could not connect to the CRM: {0}", DateTime.UtcNow);
                }

                if (!serverStatus && !_init)
                    Console.Write("x");

                if (!serverStatus)
                {
                    _syncError = true;
                    return;
                }
                _syncStartTime = DateTime.Now;
                _crmService.syncTime = _syncStartTime;
                _lastUpdateMeta = _context.Meta.Where(m => m.name == "LastUpdate").FirstOrDefault();
                var steps = _init ? 4 : 3;
                steps += _lastUpdateMeta == null ? 1 : 0;
                steps += _configuration.GetValue("Sync:Trains", false) ? 1 : 0;
                using (var pbar = new ProgressBar(steps, "Fetching Meta Data", options))
                {
                    if (_init) pbar.WriteLine($"Sync Started At: {DateTime.UtcNow}");
                    // var firstSync = _context.Meta.Where(m => m.name == "FirstSyncComplete").FirstOrDefault();

                    if (_lastUpdateMeta == null)
                    {
                        _initialStartDate = DateTime.Parse("2021-01-01 01:00:0");
                        await ChunkUpdates(async () =>
                        {
                            var initial = await _crmService.Get<Updateables>("updates/meta", _endDate);
                            await SaveReferencesAsync(initial, pbar, true);
                        }, 100);
                        await ChunkUpdates(async () =>
                        {
                            var tis = await _crmService.Get<List<TIWriteDto>>("transport-instructions", _endDate);
                            await UpdateModelAsync<TransportInstruction, TIWriteDto>(tis, pbar);
                        }, 100);
                        if (_verbose) Console.WriteLine("Importing Meta Complete");
                        // _context.Add(new Meta
                        // {
                        //     name = "FirstSyncComplete",
                        //     value = "True"
                        // });
                        pbar.Tick("Fetching Meta Data");
                    }
                    if (_init && !_configuration.GetValue("Sync:IsMaster", false)) await GetUsers(pbar);

                    _crmService.startTime = DateTime.Now.ToUniversalTime();
                    try
                    {
                        _startDate = _lastUpdateMeta != null ? DateTime.Parse(_lastUpdateMeta.value).ToUniversalTime() : DateTime.UtcNow.AddDays(-7);
                    }
                    catch
                    {
                        _startDate = DateTime.UtcNow.AddDays(-7);
                    }
                    bool res = true;
                    if (_configuration.GetValue("Sync:Meta", false))
                    {
                        // TODO:
                    }
                    _initialStartDate = _startDate;
                    await ChunkUpdates(async () =>
                    {
                        var updatedMeta = new Updates
                        {
                            Products = UpdatedProducts(),
                            Contracts = UpdatedContracts(),
                            Countries = UpdatedCountries(),
                            Units = UpdatedUnits(),
                            Offices = UpdatedOffices(),
                            Branches = UpdatedBranches(),
                            Transporters = UpdatedTransporters(),
                            TIChanges = TIChanges()
                        };
                        var results = await _crmService.Post<Updateables>(updatedMeta, "updates", _endDate);
                        await SaveReferencesAsync(results, pbar);
                    });
                    pbar.Tick("Fetching Transport Instructions");
                    await ChunkUpdates(async () =>
                    {
                        var updatedMeta = new Updates
                        {
                            TransportInstructions = UpdatedTIs(),
                        };
                        var results = await _crmService.Post<Updateables>(updatedMeta, "updates/tis", _endDate);
                        await SaveReferencesAsync(results, pbar);
                    });
                    pbar.Tick("Fetching Dispatches");
                    await ChunkUpdates(async () =>
                    {
                        var updatedMeta = new Updates
                        {
                            Dispatches = UpdatedDispatches()
                        };
                        var results = await _crmService.Post<Updateables>(updatedMeta, "updates/dispatches", _endDate, "held_dispatches=1");
                        await SaveReferencesAsync(results, pbar);
                    });
                    await ChunkUpdates(async () =>
                    {
                        var updatedMeta = new Updates
                        {
                            EventLogs = EventLogs(),
                        };
                        await _crmService.Post<Updateables>(updatedMeta, "updates/logs", _endDate);
                    });
                    if (_configuration.GetValue("Sync:Trains", false))
                    {
                        pbar.Tick("Fetching Train Data");
                        try
                        {
                            _otContext = scope.ServiceProvider.GetRequiredService<OnTractContext>();
                            res = GetTrains(_otContext, _context);
                            pbar.Tick();
                        }
                        catch (Exception e)
                        {
                            if (_env.IsDevelopment()) Console.WriteLine(e);
                            _logger.LogError("Could not Connect to DMT DB", DateTimeOffset.UtcNow);
                            _syncError = true;
                        }
                    }
                    if (res && _configuration.GetValue("Sync:IsMaster", false))
                        res = LinkWagons(_context);

                    if (_configuration.GetValue("Sync:IsMaster", false))
                        LinkTempDispatches();
                    pbar.Tick("Sync Complete");
                    if (_init)
                    {
                        _init = false;
                        pbar.WriteLine($"Sync Complete At: {DateTime.UtcNow}");
                    }
                    await UpdateLastUpdateMeta(_lastUpdateMeta, _syncStartTime);
                    await _crmService.CancelLock();
                }
            }
            catch (Exception e)
            {
                if (_env.IsDevelopment())
                    Console.WriteLine(e);
                _logger.LogError("Error", e, "Could not connect to application DB", DateTimeOffset.UtcNow);
                _syncError = true;
            }

            return;
        }

        public delegate Task ChunkAction();

        public async Task ChunkUpdates(ChunkAction action, int batchSize = 4)
        {

            if ((_syncStartTime - _initialStartDate).Days > batchSize)
            {
                var i = 0;
                var remaining = _syncStartTime.AddDays(-(i + batchSize)) - _initialStartDate;
                while (remaining.Milliseconds > 0)
                {
                    _startDate = _syncStartTime.AddDays(-(i + batchSize)) > _initialStartDate ? _syncStartTime.AddDays(-(i + batchSize)) : _initialStartDate;
                    _endDate = _syncStartTime.AddDays(-i);
                    _crmService.startTime = _startDate;
                    if (_verbose)
                        Console.WriteLine("{0} - {1}: Remaining {2} Days", _startDate, _endDate, remaining.Days);
                    try
                    {
                        await action();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    remaining = _syncStartTime.AddDays(-(i + batchSize)) - _initialStartDate;
                    i += batchSize;
                }
            }
            else
            {
                _startDate = _initialStartDate;
                _endDate = _syncStartTime;
                _crmService.startTime = _startDate;
                await action();
            }
        }

        public async Task<bool> GetUsers(ProgressBar pbar)
        {
            if (!_configuration.GetValue("Sync:IsMaster", false) && !_authSync)
            {
                pbar.Tick("Fetching Users");
                try
                {
                    var sync = await _userService.GetUsers();
                    var users = _mapper.Map<List<ApplicationUser>>(sync.Users);
                    using var child = pbar.Spawn(users.Count, "Fetching Users", childOptions);
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (!_context.ApplicationUser.Any(s => s.Email == users[i].Email))
                        {
                            // var rem = _context.ApplicatioNUser.Where(s => s.Email == users[i].Email).First();
                            // _context.OfficeUser.RemoveRange(_context.OfficeUser.Where(r => r.UserId == users[i].Id));
                            // _context.UserRoles.RemoveRange(_context.UserRoles.Where(r => r.UserId == users[i].Id));
                            // _context.ApplicationUser.Remove(rem);
                            // _context.SaveChanges();
                            _context.ApplicationUser.Add(users[i]);
                        }
                        child.Tick();
                    }
                    _context.SaveChanges();
                    sync.UserRoles.ForEach(u =>
                    {
                        var user = _context.ApplicationUser.Find(u.UserId);
                        var role = _context.Roles.Where(r => r.Name == u.Role).First();
                        if (user == null || role == null) return;
                        if (!_context.UserRoles.Any(s => s.UserId == user.Id && s.RoleId == role.Id))
                        {
                            _context.UserRoles.Add(new IdentityUserRole<string>
                            {
                                UserId = user.Id,
                                RoleId = role.Id
                            });
                        }
                    });
                    _context.SaveChanges();
                    sync.UserLocations.ForEach(u =>
                    {
                        var user = _context.ApplicationUser.Find(u.UserId);
                        var office = _context.Office.Where(r => r.Name == u.Location).FirstOrDefault();
                        if (user == null || office == null) return;
                        if (user != null && office != null && !_context.OfficeUser.Any(s => s.UserId == user.Id && s.OfficeId == office.Id))
                        {
                            _context.OfficeUser.Add(new OfficeUser
                            {
                                UserId = user.Id,
                                OfficeId = office.Id
                            });
                        }
                    });
                    _authSync = true;
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        private async Task SaveReferencesAsync(Updateables result, ProgressBar pbar, bool meta = false)
        {
            if (_verbose && result.Countries.Count > 0)
                pbar.WriteLine($"Importing {result.Countries.Count} Countries");
            await UpdateModelAsync<Country, CountryWriteDto>(result.Countries, pbar);
            if (_verbose && result.Units.Count > 0)
                pbar.WriteLine($"Importing {result.Units.Count} Units");
            await UpdateModelAsync<Unit, UnitWriteDto>(result.Units, pbar);
            if (_verbose && result.Offices.Count > 0)
                pbar.WriteLine($"Importing {result.Offices.Count} Offices");
            await UpdateModelAsync<Office, OfficeWriteDto>(result.Offices, pbar);
            if (_verbose && result.Branches.Count > 0)
                pbar.WriteLine($"Importing {result.Branches.Count} Branches");
            await UpdateModelAsync<Branch, BranchWriteDto>(result.Branches, pbar);
            if (_verbose && result.Contracts.Count > 0)
                pbar.WriteLine($"Importing {result.Contracts.Count} Contracts");
            await UpdateModelAsync<Contract, ContractWriteDto>(result.Contracts, pbar);
            if (_verbose && result.Products.Count > 0)
                pbar.WriteLine($"Importing {result.Products.Count} Products");
            await UpdateModelAsync<Product, ProductWriteDto>(result.Products, pbar);
            if (_verbose && result.Transporters.Count > 0)
                pbar.WriteLine($"Importing {result.Transporters.Count} Transporters");
            await UpdateModelAsync<Transporter, TransporterWriteDto>(result.Transporters, pbar);
            if (meta) return;
            Stopwatch stopWatch = new Stopwatch();
            if (_verbose && result.TransportInstructions.Count > 0)
                pbar.WriteLine($"Importing {result.TransportInstructions.Count} TransportInstructions");
            if (_benchmark)
            {
                stopWatch.Start();
            }
            await UpdateModelAsync<TransportInstruction, TIWriteDto>(result.TransportInstructions, pbar);
            if (_benchmark)
            {
                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                pbar.WriteLine($"Transport Instruction RunTime " + elapsedTime);
            }
            if (_verbose && result.Dispatches.Count > 0)
                pbar.WriteLine($"Importing {result.Dispatches.Count} Dispatches");
            Stopwatch dispatchStopWatch = new Stopwatch();
            if (_benchmark)
            {
                dispatchStopWatch.Start();
            }
            await UpdateDispatchesAsync(result.Dispatches, pbar);
            if (_benchmark)
            {
                TimeSpan ts2 = dispatchStopWatch.Elapsed;
                string dispatchElapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts2.Hours, ts2.Minutes, ts2.Seconds,
                    ts2.Milliseconds / 10);
                Console.WriteLine("Dispatch RunTime " + dispatchElapsedTime);
            }
        }

        private async Task UpdateDispatchesAsync(List<DispatchWriteDto> results, ProgressBar pbar)
        {
            if (results == null || results.Count == 0) return;
            using var child = pbar.Spawn(results.Count, $"Syncing Dispatches {_startDate} - {_endDate}...", childOptions);
            for (int i = 0; i < results.Count; i++)
            {
                var dispatch = results[i];
                if (dispatch.Error != null)
                {
                    if (_verbose) pbar.WriteLine("Sync error: " + dispatch.Error);
                    child.Tick($"{i} of {results.Count}");
                    continue;
                }
                try
                {
                    Stopwatch dispatchStopWatch = new Stopwatch();
                    if (_benchmark)
                    {
                        dispatchStopWatch.Start();
                    }
                    var dbDispatch = _context.Dispatch.Find(dispatch.Id);
                    if (dbDispatch == null)
                    {
                        if (dispatch.Status != "Held" && dispatch.InitialWeight != null)
                            await CreateOrUpdateAsync<Weight, WeightWriteDto>(dispatch.InitialWeight, true);
                        if (!(dispatch.Booking.VehicleType == "Wagon" && dispatch.ReceivalWeight == null))
                            await CreateOrUpdateAsync<Weight, WeightWriteDto>(dispatch.ReceivalWeight, true);
                        await CreateOrUpdateAsync<Booking, BookingWriteDto>(dispatch.Booking, true);
                        await CreateOrUpdateAsync<Dispatch, DispatchWriteDto>(dispatch, true);
                        child.Tick($"{i} of {results.Count}");
                        continue;
                    }
                    if (dispatch.Status != "Held" && dispatch.InitialWeight != null)
                        await UpdateModelAsync<Weight, WeightWriteDto>(new List<WeightWriteDto> { dispatch.InitialWeight }, pbar);
                    if (!(dispatch.Booking.VehicleType == "Wagon" && dispatch.ReceivalWeight == null))
                        await UpdateModelAsync<Weight, WeightWriteDto>(new List<WeightWriteDto> { dispatch.ReceivalWeight }, pbar);
                    await UpdateModelAsync<Booking, BookingWriteDto>(new List<BookingWriteDto> { dispatch.Booking }, pbar);
                    dbDispatch.cId = dispatch.cId;
                    dbDispatch.UpdatedAt = dispatch.UpdatedAt;
                    dbDispatch.SyncVersion = dispatch.SyncVersion ?? 0;
                    dbDispatch.IsDeleted = dispatch.IsDeleted ?? false;
                    dbDispatch.DeletedBy = dispatch.DeletedBy;
                    _context.Dispatch.Update(dbDispatch);
                    _context.SaveChanges();
                    if (_benchmark)
                    {
                        TimeSpan ts2 = dispatchStopWatch.Elapsed;
                        string dispatchElapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts2.Hours, ts2.Minutes, ts2.Seconds,
                            ts2.Milliseconds / 10);
                        Console.WriteLine("Single RunTime " + dispatchElapsedTime);
                    }
                    child.Tick($"{i} of {results.Count}");
                }
                catch (Exception e)
                {
                    child.Tick($"{i} of {results.Count}");
                    if (_verbose)
                        pbar.WriteLine($"Dispatch error: {e}");
                    pbar.WriteLine("Failed to update model: " + JsonConvert.SerializeObject(dispatch));
                    return;
                }
                // TODO: do something
            }
        }

        private async Task UpdateModelAsync<Model, Dto>(List<Dto> results, ProgressBar pbar)
        where Model : Syncable
        where Dto : SyncableDto
        {
            if (results == null || results.Count == 0)
                return;
            using var child = pbar.Spawn(results.Count, $"Syncing {_startDate} - {_endDate}...", childOptions);
            for (int i = 0; i < results.Count; i++)
            {
                var item = results[i];
                if (item.Error != null)
                {
                    if (_verbose) Console.WriteLine("Sync error: " + item.Error);
                    child.Tick();
                    continue;
                }
                try
                {
                    Stopwatch dispatchStopWatch = new Stopwatch();
                    if (_benchmark)
                        dispatchStopWatch.Start();
                    var dbModel = _context.Set<Model>().Find(item.Id);
                    if (dbModel == null)
                    {
                        await CreateOrUpdateAsync<Model, Dto>(item, false);
                        child.Tick();
                        continue;
                    }
                    dbModel.cId = item.cId;
                    dbModel.UpdatedAt = item.UpdatedAt;
                    dbModel.SyncVersion = item.SyncVersion ?? 0;
                    dbModel.IsDeleted = item.IsDeleted ?? false;
                    dbModel.DeletedBy = item.DeletedBy;
                    _context.Set<Model>().Update(dbModel);
                    if (_benchmark)
                    {
                        TimeSpan ts2 = dispatchStopWatch.Elapsed;
                        string dispatchElapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts2.Hours, ts2.Minutes, ts2.Seconds,
                            ts2.Milliseconds / 10);
                        Console.WriteLine("Single RunTime " + dispatchElapsedTime);
                    }
                    child.Tick();
                }
                catch (Exception e)
                {
                    child.Tick();
                    pbar.WriteLine($"Error: {e}");
                    pbar.WriteLine("Failed to update model: " + JsonConvert.SerializeObject(item));
                }
                // TODO: do something
            }
            _context.SaveChanges();
        }
        private async Task<TEntity> CreateOrUpdateAsync<TEntity, Dto>(Dto item, bool saveChanges = false, bool recursed = false, bool fetched = false)
        where TEntity : Syncable
        where Dto : SyncableDto
        {
            var newItem = _mapper.Map<TEntity>(item);
            var model = _context.Set<TEntity>().AsEnumerable();
            if (!model.Any(c => c.cId == item.cId))
            {
                await CopyValuesAsync(newItem, newItem, recursed);
                _context.Set<TEntity>().Add(newItem);
            }
            if (model.Any(c => c.cId == item.cId))
            {
                var current = model.Where(c => c.cId == item.cId).First();
                await CopyValuesAsync(current, newItem, recursed);
                _context.Set<TEntity>().Update(current);
            }
            if (saveChanges)
                _context.SaveChanges();
            return newItem;
        }

        private async Task CopyValuesAsync<T>(T target, T source, bool recursed) where T : Syncable
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (prop.Name != "Id")
                {
                    value = await GetRelatedAsync(value, recursed);
                    prop.SetValue(target, value, null);
                }
            }
        }
        private async Task<object> GetRelatedAsync(object value, bool recursed)
        {
            switch (value)
            {
                case Country item:
                    return await GetOrFetchRelatedAsync<CountryWriteDto, Country>(item, "countries", recursed);

                case Office item:
                    return await GetOrFetchRelatedAsync<OfficeWriteDto, Office>(item, "offices", recursed);

                case Branch item:
                    return await GetOrFetchRelatedAsync<BranchWriteDto, Branch>(item, "branches", recursed);

                case Contract item:
                    return await GetOrFetchRelatedAsync<ContractWriteDto, Contract>(item, "contracts", recursed);

                case Product item:
                    return await GetOrFetchRelatedAsync<ProductWriteDto, Product>(item, "products", recursed);

                case Transporter item:
                    return await GetOrFetchRelatedAsync<TransporterWriteDto, Transporter>(item, "transporters", recursed);

                case TransportInstruction item:
                    return await GetOrFetchRelatedAsync<TIWriteDto, TransportInstruction>(item, "transport-instructions", recursed);

                case Booking item:
                    return await GetOrFetchRelatedAsync<BookingWriteDto, Booking>(item, "bookings", recursed);

                case Dispatch item:
                    return await GetOrFetchRelatedAsync<DispatchWriteDto, Dispatch>(item, "dispatches", recursed);

                case Weight item:
                    return await GetOrFetchRelatedAsync<WeightWriteDto, Weight>(item, "weights", recursed);

                case ApplicationUser item:
                    if (_context.Set<ApplicationUser>().Where(o => o.Email == item.Email).Any())
                        return _context.Set<ApplicationUser>().Where(o => o.Email == item.Email).First();
                    break;

                case Client item:
                    return await GetOrFetchRelatedAsync<ClientWriteDto, Client>(item, "clients", recursed);

                case Unit item:
                    return await GetOrFetchRelatedAsync<UnitWriteDto, Unit>(item, "units", recursed);

                default:
                    return value;
            }
            return value;
        }

        private async Task<TEntity> GetOrFetchRelatedAsync<Dto, TEntity>(TEntity item, string endpoint, bool recursed = false)
        where TEntity : Syncable
        where Dto : SyncableDto
        {
            var current = _context.Set<TEntity>().Where(o => o.cId == item.cId).Any();
            if (current)
                return _context.Set<TEntity>().Where(o => o.cId == item.cId).First();

            if (item.cId == 0)
                return null;

            var fetched = await _crmService.Get<Dto>(endpoint, DateTime.Now, 1, item.cId);
            await CreateOrUpdateAsync<TEntity, Dto>(fetched, true, recursed);
            var mapped = _mapper.Map<TEntity>(fetched);
            return mapped;
        }

        private long GetIdentifier(string type, long id)
        {
            return type switch
            {
                "Country" => _context.Country.Find(id).cId,
                "Office" => _context.Office.Find(id).cId,
                "Branch" => _context.Branch.Find(id).cId,
                "Contract" => _context.Contract.Find(id).cId,
                "Product" => _context.Product.Find(id).cId,
                "Transporter" => _context.Transporter.Find(id).cId,
                "TransportInstruction" => _context.TransportInstruction.Find(id).cId,
                "Booking" => _context.Booking.Find(id).cId,
                "Dispatch" => _context.Dispatch.Find(id).cId,
                "Weight" => _context.Weight.Find(id).cId,
                "Client" => _context.Client.Find(id).cId,
                "Unit" => _context.Unit.Find(id).cId,
                _ => id,
            };
        }

        private List<DispatchFullSyncDto> UpdatedDispatches() => _mapper.Map<List<DispatchFullSyncDto>>(_context.Dispatch
            .Include(x => x.Booking)
            .Include(x => x.Booking.TransportInstruction)
            .Include(x => x.InitialWeight)
            .Include(w => w.InitialWeight.TareUser)
            .Include(w => w.InitialWeight.GrossUser)
            .Include(x => x.ReceivalWeight)
            .Include(w => w.ReceivalWeight.TareUser)
            .Include(w => w.ReceivalWeight.GrossUser)
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<TransporterSyncDto> UpdatedTransporters() => _mapper.Map<List<TransporterSyncDto>>(_context.Transporter
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<TISyncDto> UpdatedTIs() => _mapper.Map<List<TISyncDto>>(_context.TransportInstruction
            .Include(t => t.Contract)
            .Include(t => t.Product)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<BranchSyncDto> UpdatedBranches() => _mapper.Map<List<BranchSyncDto>>(_context.Branch
            .Include(o => o.Office)
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<OfficeSyncDto> UpdatedOffices() => _mapper.Map<List<OfficeSyncDto>>(_context.Office
            .Include(o => o.Country)
            .Include(o => o.Unit)
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<UnitSyncDto> UpdatedUnits() => _mapper.Map<List<UnitSyncDto>>(_context.Unit
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<CountrySyncDto> UpdatedCountries() => _mapper.Map<List<CountrySyncDto>>(_context.Country
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<ContractSyncDto> UpdatedContracts() => _mapper.Map<List<ContractSyncDto>>(_context.Contract
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<ProductSyncDto> UpdatedProducts() => _mapper.Map<List<ProductSyncDto>>(_context.Product
            .Include(p => p.Unit)
            .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt <= _endDate)
            .ToList());

        private List<EventLogDto> EventLogs() => _mapper.Map<List<EventLogDto>>(_context.EventLog
                    .Include(e => e.User)
                    .Where(w => w.CreatedAt > _startDate && w.CreatedAt < _endDate)
                    .ToList()
                    .Select(item =>
                    {
                        try
                        {
                            var Id = GetIdentifier(item.Resource, item.ResourceId);
                            item.ResourceId = Id;
                            return item;
                        }
                        catch (Exception)
                        {
                            return item;
                        }
                    })
                    .ToList());

        private List<TIChangeDto> TIChanges() => _mapper.Map<List<TIChangeDto>>(_context.TIChange
                    .Include(e => e.User)
                    .Include(e => e.Booking)
                    .Include(e => e.OldValue)
                    .Include(e => e.NewValue)
                    .Where(w => w.UpdatedAt > _startDate && w.UpdatedAt < _endDate)
                    .ToList());

        private async Task UpdateLastUpdateMeta(Meta prevLastUpdate, DateTime updatedAt)
        {
            await Task.Delay(10);
            if (prevLastUpdate == null)
            {
                _lastUpdateMeta = new Meta
                {
                    name = "LastUpdate",
                    value = updatedAt.ToString("o")
                };
                _context.Add(_lastUpdateMeta);
            }
            if (prevLastUpdate != null)
            {
                prevLastUpdate.value = updatedAt.ToString("o");
                _context.Meta.Update(prevLastUpdate);
            }
            _context.SaveChanges();
        }
        public MatchResult ImportWagon(OT_WagonData wagon, ApplicationDbContext context, SyncHelper help, Office office, ApplicationUser user, TransportInstruction pending)
        {
            var booking = context.Booking.FirstOrDefault(b => b.wId == wagon.Wagon_Data_ID);
            Dispatch dispatch;
            Dispatch bookedDispatch = null;
            if (booking != null)
            {
                bookedDispatch = context.Dispatch.FirstOrDefault(d => d.Booking == booking && d.InitialWeight == null);
                if (bookedDispatch == null)
                {
                    return new MatchResult { Found = false, Direction = wagon.Train.Direction };
                }
                dispatch = help.FindInTransitDispatch(booking.NumberPlate, wagon.Train.Post_Date);
            }
            if (booking == null && wagon.Train.Direction == "UP")
            {
                dispatch = help.FindProcessedDispatch(wagon.Number(), wagon.Train.Post_Date) ?? help.FindHeldDispatch(wagon.Number(), wagon.Train.Post_Date);
            }
            else
                dispatch = help.FindInTransitDispatch(wagon.Number(), wagon.Train.Post_Date);

            if (wagon.Train.Direction == "DOWN" && bookedDispatch == null)
                help.PostMissingDispatch(wagon, office, user, pending);
            if (wagon.Train.Direction == "UP" && dispatch != null)
                help.UpdatetMissingTare(dispatch, wagon, office, user, pending);
            return new MatchResult { Direction = wagon.Train.Direction };
        }

        public bool LinkWagons(ApplicationDbContext context)
        {
            var help = new SyncHelper(context);
            try
            {
                context.Dispatch
                .Include(d => d.Booking)
                .Include(d => d.ReceivalWeight)
                .Where(d => d.Status == "Held")
                .Where(d => d.Booking.VehicleType == "Wagon")
                .ToList()
                .ForEach(pendingWagon =>
                {
                    var booking = pendingWagon.Booking;
                    Dispatch inTransit = null;
                    inTransit = help.FindInTransitDispatch(booking.NumberPlate, pendingWagon.ReceivalWeight.TareAt ?? DateTime.Now);
                    if (inTransit != null)
                    {
                        inTransit.ReceivalWeight = pendingWagon.ReceivalWeight;
                        if (pendingWagon.ReceivalWeight.Tare != 0)
                            inTransit.Status = "Processed";
                        pendingWagon.Status = "Discarded";
                        context.Dispatch.Update(inTransit);
                        context.Dispatch.Update(pendingWagon);
                        context.SaveChanges();
                    }
                });
                return true;
            }
            catch (Exception e)
            {
                if (_env.IsDevelopment()) Console.WriteLine(e);
                return false;
            }
        }

        public bool GetTrains(OnTractContext otContext, ApplicationDbContext context)
        {
            _status.UpdateStatus(new Status
            {
                Message = "Starting: Getting Train Details",
                Type = StatusType.INFO
            });
            SyncHelper help = new SyncHelper(context, otContext);
            var railFrom = _configuration.GetValue("Rail:From", "Nacala");
            var railTo = _configuration.GetValue("Rail:To", "Liwonde");
            var from = context.Office
                .Where(o => o.Name.Equals(railFrom))
                .Where(o => !o.IsDeleted)
                .FirstOrDefault();
            var office = context.Office
                .Where(o => o.Name.Equals(railTo))
                .Where(o => !o.IsDeleted)
                .FirstOrDefault();
            if (from == null || office == null) return false;
            List<OT_Train> trains = help.GetInboundTrains(3);
            trains.AddRange(help.GetOutboundTrains(3));
            var pending = help.GetPendingTI(from, office);
            var user = context.ApplicationUser.FirstOrDefault(u => u.Email == "system@wf.app");
            var res = trains.SelectMany(train => train.Wagons
                .Where(w => w.isValid())
                .Select(w => ImportWagon(w, context, help, office, user, pending))
            );
            var created = res.ToList().Count();
            var found = res.ToList().Count();
            _status.UpdateStatus(new Status
            {
                Message = $"Completed: Getting Train Details | Result: {true}",
                Type = StatusType.SUCCESS
            });

            return true;
        }

        public void LinkTempDispatches()
        {
            var manualReceivals = _context.Dispatch
                .Include(d => d.Booking)
                .Include(d => d.Booking.TransportInstruction)
                .Include(d => d.ReceivalWeight)
                .Where(d => d.Status == "Temp")
                .ToList();
            manualReceivals.ForEach(receival =>
            {
                var link = _context.Dispatch
                    .Include(d => d.Booking)
                    .Include(d => d.Booking.TransportInstruction)
                    .Where(d =>
                        d.Booking.NumberPlate == receival.Booking.NumberPlate
                        && d.Booking.TransportInstruction != null && receival.Booking.TransportInstruction != null
                        && d.Booking.TransportInstruction.Id == receival.Booking.TransportInstruction.Id
                        && d.Status == "Transit"
                        && d.Booking.CreatedAt < receival.Booking.CreatedAt
                    // TODO:
                    // && d.InitialWeight.TicketNumber == temp.Booking.TempTicketNumber
                    )
                    .OrderByDescending(d => d.Booking.CreatedAt)
                    .FirstOrDefault();
                if (link != null && receival.ReceivalWeight.Tare != 0)
                {
                    link.ReceivalWeight = receival.ReceivalWeight;
                    link.Status = receival.ReceivalWeight.Tare != 0 ? "Processed" : "Transit";
                    link.UpdatedAt = DateTime.UtcNow;
                    link.SyncVersion++;
                    _context.Dispatch.Update(link);
                    receival.Status = "Discarded";
                    receival.UpdatedAt = DateTime.UtcNow;
                    receival.SyncVersion++;
                    _context.Dispatch.Update(receival);
                }
                // else if (temp.ReceivalWeight.Tare != 0)
                // {
                //     temp.Status = "Held";
                //     temp.UpdatedAt = DateTime.UtcNow;
                //     temp.SyncVersion++;
                //     _context.Dispatch.Update(temp);
                // }
            });
            _context.SaveChanges();
        }
        private void DoSync(object state)
        {
            async void action()
            {
                await Sync();
                _status.Log = false;
                Interlocked.Exchange(ref running, 0);
                if (!_syncError)
                {
                    _timer.Change(TimerInterval, TimerInterval);
                }
                else
                {
                    _timer.Change(30000, 30000);
                }
            }
            Task.Run(action);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            void action()
            {
                _timer = new Timer(
                    DoSync,
                    null,
                    0,
                    TimerInterval
                );
            }
            Task.Run(action);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
