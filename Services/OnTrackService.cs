using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using WeighForce.Data;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Services
{
    public class OnTrackService
    {
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private List<string> headers;

        public OnTrackService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        public int index(string title)
        {
            var i = headers.IndexOf(title.ToUpper());
            return i != -1 ? i + 1 : headers.Count + 1;
        }

        public dynamic getValue<T>(int row, string title, ExcelRange cell)
        {
            try
            {
                return cell[row, index(title)].GetValue<T>();
            }
            catch
            {
                return null;
            }
        }

        public List<OsrDetailsDTO> ExtractWagonData(FileInfo file)
        {
            var wagons = new List<OsrDetailsDTO>() { };
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Where(w => w.Name == "DISPATCHED BY RAIL").First();
                headers = new List<string>() { };
                int headerRow = 2;
                for (int col = 1; col < 36; col++)
                {
                    headers.Add(worksheet.Cells[headerRow, col].Value?.ToString() ?? "_");
                }
                var lastRow = worksheet.Cells.End.Row;
                var cell = worksheet.Cells;
                bool isNull = false;
                for (int i = 3; i < lastRow; i++)
                {
                    try
                    {
                        if (cell[i, 2].Value == null && isNull) break;
                        if (cell[i, 2].Value == null)
                        {
                            isNull = true;
                            continue;
                        }
                        wagons.Add(new OsrDetailsDTO
                        {
                            gdnId = getValue<long>(i, "GDN # (TC)", cell),
                            Date = getValue<DateTime>(i, "Date", cell),
                            Product = getValue<string>(i, "Product", cell),
                            Vessel = getValue<string>(i, "Vessel", cell),
                            VehicleNumber = getValue<string>(i, "Wagon #", cell),
                            Tare = getValue<double>(i, "Tare Wt", cell),
                            Gross = getValue<double>(i, "Gross Wt", cell),
                            CRMReleaseNo = getValue<string>(i, "CRM Release Nr", cell),
                            Warehouse = getValue<string>(i, "WH", cell),
                            TeamStats = getValue<string>(i, "Team Stats", cell),
                            Bl = getValue<string>(i, "Bl", cell),
                            GdnValis = getValue<string>(i, "GDN VALLIS", cell),
                            Type = "Wagon",
                            SealNumber = getValue<string>(i, "Seal Number", cell),
                            Bags = getValue<int>(i, "# OF BAGS", cell),
                            Unit = getValue<double>(i, "Unit/Bag", cell),
                            ProductWeight = getValue<double>(i, "Product Wt", cell),
                            RailedAtTrain = getValue<string>(i, "RAILED AT&TRAIN", cell),
                            ArrivalDate = getValue<DateTime>(i, "DATE OF ARRIVAL", cell),
                            BagsReceived = getValue<int>(i, "BAGS", cell),
                            Wet = getValue<double>(i, "Wet", cell),
                            QtyReceived = getValue<double>(i, "QUANTITY RECEIVED", cell),
                            Diff = getValue<double>(i, "SHORTAGE/   EXCESS", cell),
                        });
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            return wagons;
        }
        public List<OsrDetailsDTO> ExtractTruckData(FileInfo file)
        {
            var wagons = new List<OsrDetailsDTO>() { };
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Where(w => w.Name == "DISPATCHED BY ROAD").First();
                headers = new List<string>() { };
                int headerRow = 2;
                for (int col = 1; col < 36; col++)
                {
                    headers.Add(worksheet.Cells[headerRow, col].Value?.ToString() ?? "_");
                }
                var lastRow = worksheet.Cells.End.Row;
                var cell = worksheet.Cells;
                bool isNull = false;
                Console.WriteLine("Last Row {0}", lastRow);
                for (int i = 3; i < lastRow; i++)
                {
                    try
                    {
                        if (cell[i, 2].Value == null && isNull) break;
                        if (cell[i, 2].Value == null)
                        {
                            isNull = true;
                            continue;
                        }
                        string client = getValue<string>(i, "Client", cell);
                        if(client == null || client.ToUpper() != "MFC") continue;
                        wagons.Add(new OsrDetailsDTO
                        {
                            gdnId = getValue<long>(i, "GDN # (TC)", cell),
                            Date = getValue<DateTime>(i, "Date", cell),
                            Product = getValue<string>(i, "Product", cell),
                            Vessel = getValue<string>(i, "Vessel", cell),
                            VehicleNumber = getValue<string>(i, "Truck Registration", cell),
                            CRMReleaseNo = getValue<string>(i, "CRM Release Nr", cell),
                            Warehouse = getValue<string>(i, "WH", cell),
                            TeamStats = getValue<string>(i, "Team Stats", cell),
                            Bl = getValue<string>(i, "Bl", cell),
                            GdnValis = getValue<string>(i, "GDN VALLIS", cell),
                            Type = "Truck",
                            SealNumber = getValue<string>(i, "Seals", cell),
                            Bags = getValue<int>(i, "# OF BAGS", cell),
                            Unit = getValue<double>(i, "Unit/Bag", cell),
                            ProductWeight = getValue<double>(i, "Product Wt", cell),
                        });
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            return wagons;
        }

        public async Task ImportExcel(string filePath, CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting Import");
            var file = new FileInfo(filePath);
            var osr = ExtractWagonData(file);
            var trucks = ExtractTruckData(file);
            Console.WriteLine("Importing {0}", trucks.Count);
            Console.WriteLine("Importing {0}", osr.Count);
            
            osr.AddRange(trucks);
            await ImportWagons(osr, _context);
            Console.WriteLine("Import Complete");
        }

        public async Task ImportWagons(List<OsrDetailsDTO> osr, ApplicationDbContext _context)
        {
            Console.WriteLine("Importing " + osr.Count());
            var osrFrom = _configuration.GetValue<string>("Rail:From", "Nacala");
            var railTo = _configuration.GetValue<string>("Rail:To", "Liwonde");
            var roadTo = _configuration.GetValue<string>("Road:To", "Blantyre");
            var office = _context.Office.Where(o => o.Name.Equals(osrFrom)).FirstOrDefault();
            var user = _context.ApplicationUser.FirstOrDefault(u => u.Email == "system@wf.app");
            Office from = _context.Office.Where(c => c.Name.ToLower() == osrFrom.ToLower()).FirstOrDefault();
            Office to = _context.Office.Where(c => c.Name.ToLower() == railTo.ToLower()).FirstOrDefault();
            Office truckTo = _context.Office.Where(c => c.Name.ToLower() == roadTo.ToLower()).FirstOrDefault();
            Contract pendingContract = _context.Contract.Where(c => c.ContractNumber == "Pending").FirstOrDefault();
            Product pendingProduct = _context.Product.Where(c => c.Name == "Pending").FirstOrDefault();
            foreach (var vehicle in osr)
            {
                if (vehicle.gdnId == null) continue;

                var booked = _context.Booking.Where(b => b.gdnId == vehicle.gdnId).Any();
                if (booked)
                    continue;

                Contract contract = await GetContract(_context, pendingContract, vehicle);
                Product product = await GetProduct(_context, pendingProduct, vehicle);
                TransportInstruction ti = await GetTI(_context, from, vehicle.Type == "Wagon" ? to : truckTo, contract, product);
                if (vehicle.gdnId != null)
                {
                    CreateDispatch(_context, user, vehicle, ti);
                }
                SaveOSRData(_context, vehicle);
            }
            await _context.SaveChangesAsync();
        }
        public void SaveOSRData(ApplicationDbContext _context, OsrDetailsDTO wagon)
        {
            var exists = _context.OsrData.Where(o => o.gdnId == wagon.gdnId).FirstOrDefault() != null;
            if (exists) return;
            _context.OsrData.Add(_mapper.Map<OsrData>(wagon));
        }

        public Dispatch CreateDispatch(ApplicationDbContext _context, ApplicationUser user, OsrDetailsDTO vehicle, TransportInstruction ti)
        {
            return _context.Dispatch.Add(
                new Dispatch
                {
                    Booking = new Booking
                    {
                        gdnId = vehicle.gdnId,
                        VehicleType = vehicle.Type,
                        NumberPlate = vehicle.VehicleNumber,
                        CreatedAt = vehicle.Date,
                        TareWeight = 0,
                        TareAt = vehicle.Date,
                        TareUser = user,
                        TransportInstruction = ti,
                        UpdatedAt = DateTime.UtcNow
                    },
                    InitialWeight = new Weight
                    {
                        Tare = vehicle.Tare > 0 
                            ? (int)Math.Round(vehicle.Tare * 1000, 0) 
                            : vehicle.Type == "Wagon" ? 20000 : 1500,
                        TareAt = vehicle.Date,
                        Gross = vehicle.Gross > 0 
                            ? (int)Math.Round(vehicle.Gross * 1000, 0) 
                            : (int)Math.Round((decimal)(vehicle.ProductWeight * 1000), 0) + (vehicle.Type == "Wagon" ? 20000 : 1500),
                        GrossAt = vehicle.Date,
                        Office = ti.FromLocation,
                        TareUser = user,
                        GrossUser = user,
                        UpdatedAt = DateTime.UtcNow
                    },
                    Status = "Transit",
                    UpdatedAt = DateTime.UtcNow
                }
            ).Entity;
        }

        public async Task<TransportInstruction> GetTI(ApplicationDbContext _context, Office from, Office to, Contract contract, Product product)
        {
            TransportInstruction ti = _context.TransportInstruction
                .Where(t => t.Contract == contract)
                .Where(t => t.Product == product)
                .Where(t => t.FromLocation == from)
                .Where(t => t.ToLocation == to)
                .FirstOrDefault();
            if (ti == null) //TODO: && contract != null && product != null
            {
                // TODO: On Missing details, 
                // prefill form with data, 
                // on submit, search for similar/create new
                ti = _context.TransportInstruction.Add(new TransportInstruction
                {
                    Contract = contract,
                    Product = product,
                    FromLocation = from,
                    ToLocation = to,
                }).Entity;
                await _context.SaveChangesAsync();
            }
            return ti;
        }

        public async Task<Product> GetProduct(ApplicationDbContext _context, Product pendingProduct, OsrDetailsDTO wagon)
        {
            Product product = wagon.Product != null ? _context.Product.Where(c => c.Name == wagon.Product).FirstOrDefault() : pendingProduct;
            if (product == null && wagon.Product != null)
            {
                product = _context.Product.Add(new Product
                {
                    Name = wagon.Product,
                    UpdatedAt = DateTime.UtcNow
                }).Entity;
                await _context.SaveChangesAsync();
            }

            return product;
        }

        public async Task<Contract> GetContract(ApplicationDbContext _context, Contract pendingContract, OsrDetailsDTO wagon)
        {
            Contract contract = wagon.CRMReleaseNo != null ? _context.Contract.Where(c => c.ContractNumber == wagon.CRMReleaseNo).FirstOrDefault() : pendingContract;
            if (contract == null && wagon.CRMReleaseNo != null)
            {
                contract = _context.Contract.Add(new Contract
                {
                    ContractNumber = wagon.CRMReleaseNo,
                    Vessel = wagon.Vessel,
                    UpdatedAt = DateTime.UtcNow
                }).Entity;
                await _context.SaveChangesAsync();
            }

            return contract;
        }
    }
}