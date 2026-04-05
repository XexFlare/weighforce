using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WeighForce.Data.Repositories;
using WeighForce.Filters;
using WeighForce.Models;
using WeighForce.Wrappers;

namespace WeighForce.Data.EF
{
    public class BookingsRepository : IBookingsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BookingsRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IEnumerable<Booking> GetAllBookings(long OfficeId, PaginationFilter filter)
        {
            return _context.Booking
                .Include(b => b.TareUser)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToList();
        }
        public IEnumerable<Booking> GetPendingBookings(long OfficeId, PaginationFilter filter)
        {
            return _context.Booking
                .Where(b =>
                    b.TareWeight == 0
                    && b.TransportInstruction.FromLocation.Id == OfficeId
                    && b.CreatedAt >= filter.From
                    && b.CreatedAt <= filter.To.AddDays(1))
                .Include(b => b.TareUser)
                .Include(b => b.Transporter)
                .OrderByDescending(b => b.CreatedAt)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToList();
        }

        public Booking GetBooking(long id)
        {
            return _context.Booking
                .Include(b => b.TareUser)
                .FirstOrDefault(d => d.Id == id);
        }

        public Booking PostBooking(Booking booking, string userId, long officeId)
        {
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;
            var hasDefault = _context.Meta.FirstOrDefault(x => x.name == "PrefContract:" + officeId);
            if(hasDefault?.value != null) {
                var contract = _context.Contract.FirstOrDefault(x => x.Id == long.Parse(hasDefault.value));
                var from = _context.Office.Include(x => x.Unit).FirstOrDefault(x => x.Id == officeId);
                var to = _context.Office.Find(booking.TransportInstruction.ToLocation.Id);
                var product = _context.Product.Find(booking.TransportInstruction.Product.Id);
                booking.TransportInstruction = new TransportInstruction {
                    FromLocation = from,
                    ToLocation = to,
                    Contract = contract,
                    Product = product,
                    OneTime = true
                };
                var exists = _context.TransportInstruction
                    .Include(t => t.FromLocation)
                    .Include(t => t.ToLocation)
                    .Include(t => t.Contract)
                    .Include(t => t.Product)
                    .Where(x => x.FromLocation.Id == booking.TransportInstruction.FromLocation.Id)
                    .Where(x => x.ToLocation.Id == booking.TransportInstruction.ToLocation.Id)
                    .Where(x => x.Contract.Id == booking.TransportInstruction.Contract.Id)
                    .Where(x => x.Product.Id == booking.TransportInstruction.Product.Id)
                    .FirstOrDefault();
                if(exists != null) booking.TransportInstruction = exists;
            } else {
                booking.TransportInstruction = _context.TransportInstruction
                    .Include(t => t.FromLocation)
                    .FirstOrDefault(t => t.Id == booking.TransportInstruction.Id);
            }
            if (booking.Branch?.Id != null)
                booking.Branch = _context.Branch.Find(booking.Branch.Id);
                booking.Transporter = booking.Transporter.Id != 0 ? _context.Transporter.Find(booking.Transporter.Id) : null;
            if (booking.TransportInstruction == null)
                return null;
                booking.VehicleType = "Truck";
                var ticket = GenerateTicketNumber(booking.TransportInstruction.FromLocation.TicketPrefix);
            var dispatch = new Dispatch
            {
                Booking = booking,
                InitialWeight = new Weight
                {
                    Tare = booking.TareWeight,
                    TareUser = booking.TareUser,
                    TareAt = booking.TareAt,
                    Printed = false,
                    TicketNumber = ticket + "-0",
                    UpdatedAt = DateTime.UtcNow
                },
                ReceivalWeight = new Weight
                {
                    Printed = false,
                    TicketNumber = ticket + "-1",
                    UpdatedAt = DateTime.UtcNow
                },
                Status = "Booking",
                UpdatedAt = DateTime.UtcNow
            };
            var entity = _context.Dispatch.Add(dispatch).Entity;
            _context.SaveChanges();
            var user = _context.ApplicationUser.Find(userId);
            _context.EventLog.Add(new EventLog
            {
                User = user,
                Resource = "Booking",
                ResourceId = entity.Booking.Id,
                Message = $"Booking was created for {entity.Booking.NumberPlate}",
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
            return booking;
        }
        public string GenerateTicketNumber(long prefix)
        {
            prefix = prefix == 0 ? 99000000 : prefix;
            var lastTicket = _context.Meta.SingleOrDefault(m => m.name == "LastTicket");
            string ticketNumber;
            if (lastTicket == null)
            {
                var next = 1;
                ticketNumber = (prefix + next).ToString();
                _context.Meta.Add(new Meta { name = "LastTicket", value = next.ToString() });
            }
            else
            {
                var next = long.Parse(lastTicket.value) + 1;
                ticketNumber = (prefix + next).ToString();
                lastTicket.value = next.ToString();
                _context.Meta.Update(lastTicket);
            }
            ticketNumber = ticketNumber.Substring(0, 3) + "-" + ticketNumber.Substring(2);
            _context.SaveChanges();
            return ticketNumber;
        }
        public Booking PostTempBooking(Booking booking, string userId, long officeId)
        {
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;
            var hasDefault = _context.Meta.FirstOrDefault(x => x.name == "PrefContract:" + officeId);
            if(hasDefault?.value != null) {
                var contract = _context.Contract.FirstOrDefault(x => x.Id == long.Parse(hasDefault.value));
                var to = _context.Office.Include(x => x.Unit).FirstOrDefault(x => x.Id == officeId);
                var from = _context.Office.Find(booking.TransportInstruction.FromLocation.Id);
                var product = _context.Product.Find(booking.TransportInstruction.Product.Id);
                booking.TransportInstruction = new TransportInstruction {
                    FromLocation = from,
                    ToLocation = to,
                    Contract = contract,
                    Product = product,
                    OneTime = true
                };
                var exists = _context.TransportInstruction
                    .Include(t => t.FromLocation)
                    .Include(t => t.ToLocation)
                    .Include(t => t.Contract)
                    .Include(t => t.Product)
                    .Where(x => x.FromLocation.Id == booking.TransportInstruction.FromLocation.Id)
                    .Where(x => x.ToLocation.Id == booking.TransportInstruction.ToLocation.Id)
                    .Where(x => x.Contract.Id == booking.TransportInstruction.Contract.Id)
                    .Where(x => x.Product.Id == booking.TransportInstruction.Product.Id)
                    .FirstOrDefault();
                if(exists != null) booking.TransportInstruction = exists;
            } else {
                booking.TransportInstruction = _context.TransportInstruction
                    .Include(t => t.FromLocation)
                    .Include(t => t.ToLocation)
                    .FirstOrDefault(t => t.Id == booking.TransportInstruction.Id);
            }
            if (booking.Branch?.Id != null)
                booking.Branch = _context.Branch.Find(booking.Branch.Id);
            if(booking.Transporter.Id != 0)
                booking.Transporter = _context.Transporter.Find(booking.Transporter.Id);
            else booking.Transporter = null;
            booking.VehicleType = "Truck";
            var ticket = booking.TempTicketNumber ?? "";
            string ticketNumber = "";
            var len = ticket.Length;
            var isSystemGenerated = len > 10 && ticket.Substring(len - 2, 2) == "-0";
            if(isSystemGenerated){
                ticketNumber= ticket.Substring(0, len - 1) + "1";
            } else {
                ticketNumber = GenerateTicketNumber(booking.TransportInstruction.ToLocation.TicketPrefix) + "-1";
            }
            var dispatch = new Dispatch
            {
                Booking = booking,
                InitialWeight = new Weight
                {
                    Tare = booking.TareWeight,
                    TareUser = booking.TareUser,
                    TareAt = booking.TareAt,
                    Printed = false,
                    UpdatedAt = DateTime.UtcNow
                },
                ReceivalWeight = new Weight
                {
                    Printed = false,
                    TicketNumber = ticketNumber,
                    UpdatedAt = DateTime.UtcNow
                },
                Status = "Temp",
                UpdatedAt = DateTime.UtcNow
            };
            var entity = _context.Dispatch.Add(dispatch).Entity;
            _context.SaveChanges();
            var user = _context.ApplicationUser.Find(userId);
            _context.EventLog.Add(new EventLog
            {
                User = user,
                Resource = "Booking",
                ResourceId = entity.Booking.Id,
                Message = $"Manual Receival Booking was created for {entity.Booking.NumberPlate}",
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
            return booking;
        }

        public Booking UpdateTI(long id, long contractId, long productId)
        {
            Booking booking = _context.Booking
                .Include(b => b.TransportInstruction)
                .Include(b => b.TransportInstruction.FromLocation)
                .Include(b => b.TransportInstruction.ToLocation)
                .Include(b => b.TransportInstruction.Product)
                .Include(b => b.TransportInstruction.Contract)
                .FirstOrDefault(b => b.Id == id);
            TransportInstruction ti = _context.TransportInstruction
                .Include(t => t.Contract)
                .Include(t => t.Product)
                .Where(t => t.Contract.Id == contractId)
                .Where(t => t.Product.Id == productId)
                .FirstOrDefault();
            if (ti == null)
            {
                var office = _context.Office.Where(o => o.Name.Equals("Nacala")).FirstOrDefault();
                var user = _context.ApplicationUser.FirstOrDefault(u => u.Email == "system@wf.app");
                Office from = _context.Office.Where(c => c.Name == "Nacala").FirstOrDefault();
                Office to = _context.Office.Where(c => c.Name == "Liwonde").FirstOrDefault();
                Contract contract = _context.Contract.Find(contractId);
                Product product = _context.Product.Find(productId);
                if (contract == null || product == null)
                {
                    return null;
                }
                ti = _context.TransportInstruction.Add(new TransportInstruction
                {
                    Contract = contract,
                    Product = product,
                    FromLocation = from,
                    ToLocation = to,
                }).Entity;
            }
            booking.TransportInstruction = ti;
            _context.Booking.Update(booking);
            _context.SaveChanges();
            return booking;
        }

        public Booking SetNumberPlate(long id, string numberPlate)
        {
            Booking Booking = _context.Booking.Find(id);
            Booking.NumberPlate = numberPlate;
            _context.Booking.Update(Booking);
            return Booking;
        }

        public PagedQuery<IEnumerable<OsrData>> GetAllOsrData(PaginationFilter filter)
        {
            var query = _context.OsrData;
            // .Where(b => b.Date >= filter.From && b.Date <= filter.To.AddDays(1))
            return new PagedQuery<IEnumerable<OsrData>>
            {
                TotalRecords = query.Count(),
                TotalPages = ((int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0))),
                Records = query
                    .OrderByDescending(d => d.Date)
                    .Skip((filter.Page - 1) * filter.Size)
                    .Take(filter.Size)
                    .ToList()
            };
        }

        public bool CheckIncoming(long locationId, string numberPlate, string ticketNumber)
        {
            var createdDate = DateTime.UtcNow.AddDays(-3);
            return _context.Dispatch
                .Include(d => d.Booking)
                .Include(d => d.Booking.TransportInstruction)
                .Include(d => d.Booking.TransportInstruction.ToLocation)
                .Any(
                    d => d.Status == "Transit"
                    && d.Booking.NumberPlate.Replace(" ", "") == numberPlate.Replace(" ", "")
                    && d.Booking.TransportInstruction.ToLocation.Id == locationId
                    && d.Booking.CreatedAt >= createdDate
                );
        }

        public Booking ChangeTI(long id, long tiId, string userId)
        {
            var ti = _context.TransportInstruction
                .Include(t => t.Product)
                .Include(t => t.Contract)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .FirstOrDefault(t => t.Id == tiId);
            var booking = _context.Booking
                .Include(b => b.TransportInstruction)
                .Include(b => b.TransportInstruction.Contract)
                .Include(b => b.TransportInstruction.Product)
                .Include(b => b.TIChanges)
                .FirstOrDefault(b => b.Id == id);
            if (ti == null || booking == null) return null;
            var user = _context.ApplicationUser.Find(userId);
            _context.EventLog.Add(new EventLog
            {
                User = user,
                Resource = "Booking",
                ResourceId = booking.Id,
                Message = $"Transport Instruction was updated",
                CreatedAt = DateTime.UtcNow
            });
            booking.TIChanges.Add(new TIChange
            {
                User = user,
                OldValue = booking.TransportInstruction,
                NewValue = ti,
                UpdatedAt = DateTime.UtcNow
            });
            booking.TransportInstruction = ti;
            _context.Booking.Update(booking);
            _context.SaveChanges();
            return booking;
        }
    }
}