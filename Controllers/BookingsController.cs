using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeighForce.Data.Repositories;
using WeighForce.Dtos;
using AutoMapper;
using WeighForce.Models;
using WeighForce.Data;
using System;
using WeighForce.Filters;
using WeighForce.Wrappers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using WeighForce.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingsRepository _repo;
        private readonly IMapper _mapper;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly SyncService _syncService;
        private readonly IConfiguration _configuration;
        public BookingsController(IBookingsRepository context, IMapper mapper, IBackgroundTaskQueue backgroundTaskQueue, SyncService syncService, IConfiguration configuration)
        {
            _repo = context;
            _mapper = mapper;
            _backgroundTaskQueue = backgroundTaskQueue ?? throw new ArgumentNullException(nameof(backgroundTaskQueue));
            _syncService = syncService;
            _configuration = configuration;
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public ActionResult<BookingDto> GetClient(long id)
        {
            var item = _repo.GetBooking(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookingDto>(item));
        }
        [HttpGet("temp/{regNum}")]
        public ActionResult<bool> Check(string regNum, [FromQuery] string ticket)
        {
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var incoming = _repo.CheckIncoming(id, regNum, ticket);
            return Ok(!incoming);
        }
        [HttpPost]
        public ActionResult<BookingDto> AddBooking(BookingPostDto booking)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int officeId = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out officeId);
            var item = _repo.PostBooking(_mapper.Map<Booking>(booking), userId, officeId);
            if (item == null) return BadRequest(new { message = "Booking Failed" });
            return CreatedAtAction("NewBooking", new { id = item.Id }, _mapper.Map<BookingDto>(item));
        }
        [HttpPost("temp")]
        public ActionResult<BookingDto> AddTempBooking(BookingPostDto booking)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int officeId = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out officeId);
            var item = _repo.PostTempBooking(_mapper.Map<Booking>(booking), userId, officeId);
            return CreatedAtAction("NewBooking", new { id = item.Id }, _mapper.Map<BookingDto>(item));
        }
        [HttpGet("ticket/{ticket}")]
        public ActionResult<BookingDto> ticket(string ticket)
        {
            var len = ticket.Length;
            var isGeneratedTicket = ticket.Substring(len - 2, 2) == "-0";
            string ticketNumber;
            if (isGeneratedTicket)
            {
                ticketNumber = ticket.Substring(0, len - 1) + "1";
            }
            else
            {
                ticketNumber = ticket.Substring(0, len - 1) + "-1";
            }
            return Ok(new {
                isGeneratedTicket,
                ticketNumber, 
            });
        }
        [HttpPost("import/excel")]
        public async Task<ActionResult> ImportExcelAsync(IFormCollection files)
        {
            long size = files.Files.Sum(f => f.Length);
            foreach (var file in files.Files)
            {
                if (file.Length == 0 && !CheckIfExcelFile(file))
                    return BadRequest(new { message = "Invalid file extension" });
                string path = await WriteFile(file);
                if (path == null)
                    return BadRequest(new { message = "Upload Failed" });

                _backgroundTaskQueue.EnqueueTask(async (serviceScopeFactory, cancellationToken) =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    try
                    {
                        var repo = new OnTrackService(context, _mapper, _configuration);
                        await repo.ImportExcel(path, cancellationToken);
                        if (_configuration.GetValue<bool>("Sync:Trains", false))
                        {
                            var onTrackContext = scope.ServiceProvider.GetRequiredService<OnTractContext>();
                            _syncService.GetTrains(onTrackContext, context);
                        }
                        _syncService.LinkWagons(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, "Could not do Import OSR");
                    }
                });
            }
            return Ok(new { count = files.Files.Count, size });
        }
        private bool CheckIfExcelFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[^1];
            return extension == ".xlsx" || extension == ".xls";
        }
        private async Task<string> WriteFile(IFormFile file)
        {
            string fileName;
            try
            {
                var extension = "." + file.FileName.Split('.')[^1];
                fileName = DateTime.UtcNow.Ticks + extension;

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload",
                   fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return path;
            }
            catch (Exception)
            {
                //log error
                return null;
            }
        }

        [HttpPost("{id}/ti")]
        public ActionResult<BookingDto> UpdateTI(long id, TIUpdate ti)
        {
            var item = _repo.UpdateTI(id, ti.contractId, ti.productId);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookingDto>(item));
        }
        [HttpPut("{id}/ti")]
        public ActionResult<BookingDto> ChangeTI(long id, [FromBody] IDto ti)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var item = _repo.ChangeTI(id, ti.Id, userId);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookingDto>(item));
        }
        [HttpGet("osr")]
        public ActionResult<IEnumerable<OsrData>> GetAllOsrData([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To);
            var items = _repo.GetAllOsrData(validFilter);
            return Ok(new PagedResponse<IEnumerable<OsrData>, IEnumerable<OsrData>>(items, validFilter, _mapper));
        }
    }
}
