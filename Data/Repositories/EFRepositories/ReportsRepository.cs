using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WeighForce.Data.Repositories;
using WeighForce.Filters;
using WeighForce.Models;
using WeighForce.Services;

namespace WeighForce.Data.EF
{
  public class Report
  {
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public List<ReportLocation> Locations { get; set; }
  }
  public class TransporterMismatch
  {
    public Office Location;
    public string NumberPlate;
    public string Transporter;
    public string Previous;
    public string DoneBy;
    public string TicketNumber;
  }
  public class ReportLocation
  {
    public string Name { get; set; }
    public int Total { get; set; }
    public List<ProductSummary> Summary { get; set; }
    public List<Dispatch> Dispatches { get; set; }
  }
  public class ProductSummary
  {

    public string Product { get; set; }
    public int Amount { get; set; }
    public List<DispatchSummary> Dispatches { get; set; }
  }
  public class DispatchSummary
  {
    public long Id { get; set; }
    public String Driver { get; set; }
    public String NumberPlate { get; set; }
    public int? Dispatched { get; set; }
    public int? Received { get; set; }
    public int? Diff { get; set; }
    public DateTime? DispatchedOn { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public String DispatchedBy { get; set; }
    public String ReceivedBy { get; set; }
  }

  public class ReportsRepository : IReportsRepository
  {
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;

    public ReportsRepository(ApplicationDbContext context, EmailService emailService)
    {
      _context = context;
      _emailService = emailService;
    }

    public List<ReportLocation> AnnualDispatches(string userId, ReportFilter filter)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .FirstOrDefault();
      if (user == null) return null;
      var locations = filter.OfficeId != null ? user.OfficeUsers.Where(l => l.OfficeId == filter.OfficeId).Select(l => l.Office) : user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var endDate = filter.Date ?? DateTime.Today;
      var startDate = endDate.AddYears(-1);
      foreach (var location in locations)
      {
        var dispatches = _context.Dispatch
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.InitialWeight)
            .Where(d => d.InitialWeight.Gross != 0)
            .Where(d => d.InitialWeight.GrossAt >= startDate)
            .Where(d => d.InitialWeight.GrossAt <= endDate)
            .Where(d => d.Booking.TransportInstruction.FromLocation.Id == location.Id)
            .ToList();
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => d.InitialWeight.Gross - d.InitialWeight.Tare) ?? 0
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary,
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }
    public List<ReportLocation> AnnualReceivals(string userId, ReportFilter filter)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .FirstOrDefault();
      if (user == null) return null;
      var locations = filter.OfficeId != null ? user.OfficeUsers.Where(l => l.OfficeId == filter.OfficeId).Select(l => l.Office) : user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var endDate = filter.Date ?? DateTime.Today;
      var startDate = endDate.AddYears(-1);
      foreach (var location in locations)
      {
        var dispatches = _context.Dispatch
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.ReceivalWeight)
            .Where(d => d.ReceivalWeight.Tare != 0)
            .Where(d => d.ReceivalWeight.GrossAt >= startDate)
            .Where(d => d.ReceivalWeight.GrossAt <= endDate)
            .Where(d => d.Booking.TransportInstruction.ToLocation.Id == location.Id)
            .ToList();
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => d.ReceivalWeight.Gross - d.ReceivalWeight.Tare) ?? 0
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary,
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }

    public List<ReportLocation> DailyDispatches(string userId, ReportFilter filter)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .FirstOrDefault();
      if (user == null) return null;
      var locations = filter.OfficeId != null ? user.OfficeUsers.Where(l => l.OfficeId == filter.OfficeId).Select(l => l.Office) : user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = startDate.AddDays(1);
      foreach (var location in locations)
      {
        System.Console.WriteLine(location.Name);
        var dispatches = _context.Dispatch
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.InitialWeight)
            .Where(d => d.InitialWeight.Gross != 0)
            .Where(d => d.InitialWeight.GrossAt >= startDate)
            .Where(d => d.InitialWeight.GrossAt <= endDate)
            .Where(d => d.Booking.TransportInstruction.FromLocation.Id == location.Id)
            .ToList();
        System.Console.WriteLine(dispatches.Count);
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped?.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => d.InitialWeight.Gross - d.InitialWeight.Tare) ?? 0
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }
    public List<ReportLocation> DailyReceivals(string userId, ReportFilter filter)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .FirstOrDefault();
      if (user == null) return null;
      var locations = filter.OfficeId != null ? user.OfficeUsers.Where(l => l.OfficeId == filter.OfficeId).Select(l => l.Office) : user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = startDate.AddDays(1);
      foreach (var location in locations)
      {
        var dispatches = _context.Dispatch
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.ReceivalWeight)
            .Where(d => d.ReceivalWeight.Tare != 0)
            .Where(d => d.ReceivalWeight.GrossAt >= startDate)
            .Where(d => d.ReceivalWeight.GrossAt <= endDate)
            .Where(d => d.Booking.TransportInstruction.ToLocation.Id == location.Id)
            .ToList();
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped?.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => d.ReceivalWeight.Gross - d.ReceivalWeight.Tare) ?? 0
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }
    public List<ReportLocation> DailyDispatchesEmail(string userId)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .FirstOrDefault();
      if (user == null) return null;
      var locations = user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var date = DateTime.Today.AddDays(-1);
      foreach (var location in locations)
      {
        var dispatches = _context.Dispatch
            .Include(d => d.Booking)
            .Include(d => d.Booking.TransportInstruction.Contract)
            .Include(d => d.Booking.TransportInstruction.ToLocation)
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.InitialWeight)
            .Where(d => d.InitialWeight.Gross != 0)
            .Where(d => d.InitialWeight.GrossAt >= date)
            .Where(d => d.Booking.TransportInstruction.FromLocation.Id == location.Id)
            .ToList();
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped?.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => d.InitialWeight.Gross - d.InitialWeight.Tare) ?? 0
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary,
          Dispatches = dispatches
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }
    public List<TransporterMismatch> SendTransporterMismatchReport()
    {
      DateTime start = DateTime.Now.AddDays(-10);
      var bookings = _context.Booking
              .Include(b => b.Transporter)
              .Where(b => b.CreatedAt > start)
              .Where(b => b.VehicleType == "Truck")
              .ToList();
      var mismatched = bookings.Select(booking =>
      {
        var previousList = _context.Booking
                  .Include(b => b.Transporter)
                  .Where(b => b.NumberPlate == booking.NumberPlate && b.CreatedAt < booking.CreatedAt)
                  .OrderByDescending(b => b.CreatedAt);
        if (previousList.Count() == 0)
        {
          return new
          {
            Mismatch = false,
            Booking = booking,
            Previous = booking,
          };
        }
        var previous = previousList.First();
        if (previous != null && previous.Transporter.Name?.Trim().ToLower() != booking.Transporter.Name?.Trim().ToLower())
          return new
          {
            Mismatch = true,
            Booking = booking,
            Previous = previous,
          };
        return new
        {
          Mismatch = false,
          Booking = booking,
          Previous = previous,
        };
      }).Where(x => x.Mismatch);
      var loaded = mismatched.Select(l =>
      {
        var Location = l.Booking.TempTicketNumber != null ? _context.Booking.Include(b => b.TransportInstruction.ToLocation)
                  .FirstOrDefault(x => x.Id == l.Booking.Id)
                  .TransportInstruction.ToLocation
              : _context.Booking.Include(b => b.TransportInstruction.FromLocation)
                  .FirstOrDefault(x => x.Id == l.Booking.Id)
                  .TransportInstruction.FromLocation;
        var weight = l.Booking.TempTicketNumber != null ?
                  _context.Dispatch
                      .Include(d => d.ReceivalWeight)
                      .Include(d => d.ReceivalWeight.GrossUser)
                      .FirstOrDefault(x => x.Booking.Id == l.Booking.Id)?.ReceivalWeight :
                  _context.Dispatch
                      .Include(d => d.InitialWeight)
                      .Include(d => d.InitialWeight.TareUser)
                      .FirstOrDefault(x => x.Booking.Id == l.Booking.Id)?.InitialWeight;
        return new TransporterMismatch
        {
          Location = Location,
          NumberPlate = l.Booking.NumberPlate,
          Transporter = l.Booking.OtherTransporter ?? l.Booking.Transporter.Name,
          Previous = l.Previous.Transporter.Name,
          DoneBy = l.Booking.TempTicketNumber != null ? weight?.GrossUser?.Name : weight?.TareUser?.Name,
          TicketNumber = weight?.TicketNumber
        };
      }).ToList();
      var mailTo = MailingList("Logistics");
      mailTo.ForEach(async rcpt =>
      {
        await _emailService.SendTransporterReport(rcpt.User.Email, loaded.Where(b => b.Location.Id == rcpt.Office.Id).ToList());
      });
      return loaded;
    }

    public List<UserMail> MailingList(string alert)
        => _context.UserMail.Include(x => x.User)
            .Where(a => a.Alert == alert).ToList();


    public async Task SendReportsAsync()
    {
      var users = _context.ApplicationUser.ToList();
      var filter = new ReportFilter
      {
        Date = DateTime.Today.AddDays(-7)
      };
      foreach (var user in users)
      {
        var alerts = _context.UserMail.Where(u => u.User.Id == user.Id).Select(a => a.Alert).ToList();
        Console.WriteLine("{0} : {1}", user.Email, alerts.Count);
        if (alerts.Any(a => a == "Weekly Summary"))
        {

          var report = new Report
          {
            Locations = WeeklyDispatches(user.Id, filter),
            StartDate = DateTime.Today.AddDays(-7).ToShortDateString(),
            EndDate = DateTime.Today.ToShortDateString()
          };
          await _emailService.SendWeeklyReport(user.Email, report);
        }
        if (alerts.Any(a => a == "Daily Summary"))
        {
          var report = new Report
          {
            Locations = DailyDispatchesEmail(user.Id),
            StartDate = DateTime.Today.ToShortDateString()
          };
          await _emailService.SendDailyReport(user.Email, report);
        }
      }
    }
    public async Task SendTrainsReport()
    {
      var mailingList = _context.MailingList.Where(x => x.Alert == AlertType.TRAIN).ToList();
      if(mailingList.Count == 0) return;

      var trains = TrainsToSend();
      var addresses = mailingList.Select(item => new Address(item.Email, item.Name)).ToList();
      await _emailService.SendTrainsReport(addresses, trains);
      foreach (var train in trains)
      {
        train.ToEmail = false;
        _context.Dispatch.Update(train);
      }
      _context.SaveChanges();
    }

    public List<Dispatch> TrainsToSend()
    {
      return _context.Dispatch
        .Include(d => d.Booking.TransportInstruction.FromLocation)
        .Include(d => d.Booking.TransportInstruction.ToLocation)
        .Include(d => d.Booking.TransportInstruction.Contract)
        .Include(d => d.Booking.TransportInstruction.Product)
        .Include(d => d.InitialWeight)
        .Include(d => d.ReceivalWeight)
        .Where(d => d.ToEmail)
        .ToList();
    }
    public List<ReportLocation> WeeklyDispatches(string userId, ReportFilter filter)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .First();
      var locations = filter.OfficeId != null ? user.OfficeUsers.Where(l => l.OfficeId == filter.OfficeId).Select(l => l.Office) : user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = startDate.AddDays(7);
      foreach (var location in locations)
      {
        var dispatches = _context.Dispatch
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.InitialWeight)
            .Where(d => d.InitialWeight.Gross != 0)
            .Where(d => d.InitialWeight.GrossAt >= startDate)
            .Where(d => d.InitialWeight.GrossAt <= endDate)
            .Where(d => d.Booking.TransportInstruction.FromLocation.Id == location.Id)
            .ToList();
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => d.InitialWeight.Gross - d.InitialWeight.Tare) ?? 0
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary,
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }
    public List<ReportLocation> WeeklyReceivals(string userId, ReportFilter filter)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .First();
      var locations = filter.OfficeId != null ? user.OfficeUsers.Where(l => l.OfficeId == filter.OfficeId).Select(l => l.Office) : user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = startDate.AddDays(7);
      foreach (var location in locations)
      {
        var dispatches = _context.Dispatch
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.ReceivalWeight)
            .Where(d => d.ReceivalWeight.Tare != 0)
            .Where(d => d.ReceivalWeight.GrossAt >= startDate)
            .Where(d => d.ReceivalWeight.GrossAt <= endDate)
            .Where(d => d.Booking.TransportInstruction.ToLocation.Id == location.Id)
            .ToList();
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => d.ReceivalWeight.Gross - d.ReceivalWeight.Tare) ?? 0
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary,
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }
    public List<ReportLocation> ShortageReport(string userId, ReportFilter filter)
    {
      var user = _context.ApplicationUser
          .Include(u => u.OfficeUsers)
          .ThenInclude(u => u.Office)
          .Where(u => u.Id == userId)
          .First();
      var locations = filter.OfficeId != null ? user.OfficeUsers.Where(l => l.OfficeId == filter.OfficeId).Select(l => l.Office) : user.OfficeUsers.Select(l => l.Office);
      var locationsSummary = new List<ReportLocation>();
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = filter.ToDate?.AddDays(1) ?? DateTime.Now;
      foreach (var location in locations)
      {
        var dispatches = _context.Dispatch
            .Include(d => d.Booking.TransportInstruction.Product)
            .Include(d => d.ReceivalWeight)
            .Include(d => d.ReceivalWeight.GrossUser)
            .Include(d => d.InitialWeight)
            .Include(d => d.InitialWeight.GrossUser)
            .Where(d => d.InitialWeight.Gross != 0)
            .Where(d => d.ReceivalWeight.Tare != 0)
            .Where(d => d.ReceivalWeight.TareAt >= startDate)
            .Where(d => d.ReceivalWeight.TareAt < endDate)
            .Where(d => d.Booking.TransportInstruction.ToLocation.Id == location.Id)
            .Where(d => (d.ReceivalWeight.Gross - d.ReceivalWeight.Tare) - (d.InitialWeight.Gross - d.InitialWeight.Tare) < 0)
            .ToList();
        var grouped = dispatches.GroupBy(d => d.Booking.TransportInstruction.Product.Name).ToList();
        var summary = grouped.Select(g => new ProductSummary
        {
          Product = g.Key,
          Amount = g.Sum(d => (d.ReceivalWeight.Gross - d.ReceivalWeight.Tare) - (d.InitialWeight.Gross - d.InitialWeight.Tare)) * -1 ?? 0,
          Dispatches = g.Select(d => new
          DispatchSummary
          {
            Id = d.Id,
            Driver = d.Booking.DriverName,
            NumberPlate = d.Booking.NumberPlate,
            Dispatched = d.InitialWeight.Gross - d.InitialWeight.Tare,
            Received = d.ReceivalWeight.Gross - d.ReceivalWeight.Tare,
            Diff = (d.ReceivalWeight.Gross - d.ReceivalWeight.Tare) - (d.InitialWeight.Gross - d.InitialWeight.Tare),
            DispatchedOn = d.InitialWeight.GrossAt,
            ReceivedOn = d.ReceivalWeight.GrossAt,
            DispatchedBy = d.InitialWeight.GrossUser.Name,
            ReceivedBy = d.ReceivalWeight.GrossUser.Name,
          }).ToList(),
        }).ToList();
        locationsSummary.Add(new ReportLocation
        {
          Name = location.Name,
          Total = dispatches.Count,
          Summary = summary,
        });
      }
      return locationsSummary.OrderByDescending(r => r.Total).ToList();
    }

    public string DispatchedReport(ReportFilter filter)
    {
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = filter.ToDate ?? startDate.AddDays(7);
      var dispatches = _context.Dispatch
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Where(d => d.InitialWeight.Gross != 0)
          .Where(d => d.InitialWeight.GrossAt >= startDate)
          .Where(d => d.InitialWeight.GrossAt <= endDate)
          .Where(d => d.Booking.TransportInstruction.FromLocation.Id == filter.OfficeId)
          .ToList();
      var title = Guid.NewGuid().ToString("n").Substring(0, 10);
      var file = new FileInfo(@"./wwwroot/reports/dispatched_report_" + title + ".xlsx");
      using (var package = new ExcelPackage(file))
      {
        var sheet = package.Workbook.Worksheets.Add("Dispatches");
        sheet.SetValue(1, 1, "Date");
        sheet.SetValue(1, 2, "W.B. Ticket #");
        sheet.SetValue(1, 3, "Transport Instruction #");
        sheet.SetValue(1, 4, "Contract #");
        sheet.SetValue(1, 5, "LPO #");
        sheet.SetValue(1, 6, "Location From");
        sheet.SetValue(1, 7, "Location To");
        sheet.SetValue(1, 8, "Product");
        sheet.SetValue(1, 9, "Description");
        sheet.SetValue(1, 10, "Truck Reg #");
        sheet.SetValue(1, 11, "Trailer Reg #");
        sheet.SetValue(1, 12, "Tare Weight");
        sheet.SetValue(1, 13, "Tare By");
        sheet.SetValue(1, 14, "Gross Weight");
        sheet.SetValue(1, 15, "Gross By");
        sheet.SetValue(1, 16, "Net Weight");
        sheet.SetValue(1, 17, "Bag Count");
        sheet.SetValue(1, 18, "Days in Transit");
        sheet.SetValue(1, 19, "Status");
        sheet.SetValue(1, 20, "Drivers Name");
        sheet.SetValue(1, 21, "License");
        sheet.SetValue(1, 22, "Transporter Company");

        for (int i = 0; i < dispatches.Count; i++)
        {
          var d = dispatches[i];
          var daysInTransit = d.ReceivalWeight?.GrossAt != null && d.InitialWeight?.GrossAt != null
              ? d.ReceivalWeight.GrossAt - d.InitialWeight.GrossAt
              : d.InitialWeight?.GrossAt != null
                  ? DateTime.Today - d.InitialWeight.GrossAt
                  : TimeSpan.Zero;
          sheet.SetValue(i + 2, 1, d.InitialWeight.GrossAt.ToString());
          sheet.SetValue(i + 2, 2, d.InitialWeight.TicketNumber);
          sheet.SetValue(i + 2, 3, d.Booking.TransportInstruction.KineticRef ?? d.Booking.TransportInstruction.Id.ToString() + " (*)");
          sheet.SetValue(i + 2, 4, d.Booking.TransportInstruction.Contract.ContractNumber);
          sheet.SetValue(i + 2, 5, d.Booking.TransportInstruction.Contract.ContractNumber);
          sheet.SetValue(i + 2, 6, d.Booking.TransportInstruction.FromLocation.Name);
          sheet.SetValue(i + 2, 7, d.Booking.TransportInstruction.ToLocation.Name);
          sheet.SetValue(i + 2, 8, d.Booking.TransportInstruction.Product.Name);
          sheet.SetValue(i + 2, 9, "");
          sheet.SetValue(i + 2, 10, d.Booking.NumberPlate);
          sheet.SetValue(i + 2, 11, d.Booking.TrailerNumber);
          sheet.SetValue(i + 2, 12, d.InitialWeight.Tare);
          sheet.SetValue(i + 2, 13, d.InitialWeight.TareUser.Name);
          sheet.SetValue(i + 2, 14, d.InitialWeight.Gross);
          sheet.SetValue(i + 2, 15, d.InitialWeight.GrossUser.Name);
          sheet.SetValue(i + 2, 16, d.InitialWeight.Gross - d.InitialWeight.Tare);
          sheet.SetValue(i + 2, 17, d.InitialWeight.Bags);
          sheet.SetValue(i + 2, 18, daysInTransit?.Days);
          sheet.SetValue(i + 2, 19, d.Status);
          sheet.SetValue(i + 2, 20, d.Booking.DriverName);
          sheet.SetValue(i + 2, 21, d.Booking.PassportNumber);
          sheet.SetValue(i + 2, 22, d.Booking.Transporter?.Name ?? d.Booking.OtherTransporter);
        }
        package.Save();
      }
      return "reports/" + file.Name;
    }

    public string ReceivedReport(ReportFilter filter)
    {
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = filter.ToDate ?? startDate.AddDays(7);
      var dispatches = _context.Dispatch
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser)
          .Where(d => d.ReceivalWeight.Gross != 0)
          .Where(d => d.ReceivalWeight.GrossAt >= startDate)
          .Where(d => d.ReceivalWeight.GrossAt <= endDate)
          .Where(d => d.Booking.TransportInstruction.ToLocation.Id == filter.OfficeId)
          .ToList();

      var title = Guid.NewGuid().ToString("n").Substring(0, 10);
      var file = new FileInfo(@"./wwwroot/reports/receival_report_" + title + ".xlsx");
      using (var package = new ExcelPackage(file))
      {
        var sheet = package.Workbook.Worksheets.Add("Receivals");
        sheet.SetValue(1, 1, "Date");
        sheet.SetValue(1, 2, "W.B. Ticket #");
        sheet.SetValue(1, 3, "Transport Instruction #");
        sheet.SetValue(1, 4, "Contract #");
        sheet.SetValue(1, 5, "LPO #");
        sheet.SetValue(1, 6, "Location From");
        sheet.SetValue(1, 7, "Location To");
        sheet.SetValue(1, 8, "Product");
        sheet.SetValue(1, 9, "Description");
        sheet.SetValue(1, 10, "Truck Reg #");
        sheet.SetValue(1, 11, "Trailer Reg #");
        sheet.SetValue(1, 12, "Tare Weight");
        sheet.SetValue(1, 13, "Tare By");
        sheet.SetValue(1, 14, "Gross Weight");
        sheet.SetValue(1, 15, "Gross By");
        sheet.SetValue(1, 16, "Net Weight");
        sheet.SetValue(1, 17, "Bag Count");
        sheet.SetValue(1, 18, "Weight Difference");
        sheet.SetValue(1, 19, "Dispatched Weight");
        sheet.SetValue(1, 20, "Dispatched Bag Count");
        sheet.SetValue(1, 21, "Days in Transit");
        sheet.SetValue(1, 22, "Status");
        sheet.SetValue(1, 23, "Drivers Name");
        sheet.SetValue(1, 24, "License");
        sheet.SetValue(1, 25, "Transporter Company");
        for (int i = 0; i < dispatches.Count; i++)
        {
          var d = dispatches[i];
          var daysInTransit = d.ReceivalWeight?.GrossAt != null && d.InitialWeight?.GrossAt != null
              ? d.ReceivalWeight.GrossAt - d.InitialWeight.GrossAt
              : d.InitialWeight?.GrossAt != null
                  ? DateTime.Today - d.InitialWeight.GrossAt
                  : TimeSpan.Zero;
          sheet.SetValue(i + 2, 1, d.ReceivalWeight.GrossAt.ToString());
          sheet.SetValue(i + 2, 2, d.ReceivalWeight.TicketNumber);
          sheet.SetValue(i + 2, 3, d.Booking.TransportInstruction.KineticRef ?? d.Booking.TransportInstruction.Id.ToString() + " (*)");
          sheet.SetValue(i + 2, 4, d.Booking.TransportInstruction.Contract.ContractNumber);
          sheet.SetValue(i + 2, 5, d.Booking.TransportInstruction.Contract.ContractNumber);
          sheet.SetValue(i + 2, 6, d.Booking.TransportInstruction.FromLocation.Name);
          sheet.SetValue(i + 2, 7, d.Booking.TransportInstruction.ToLocation.Name);
          sheet.SetValue(i + 2, 8, d.Booking.TransportInstruction.Product.Name);
          sheet.SetValue(i + 2, 9, "");
          sheet.SetValue(i + 2, 10, d.Booking.NumberPlate);
          sheet.SetValue(i + 2, 11, d.Booking.TrailerNumber);
          sheet.SetValue(i + 2, 12, d.ReceivalWeight.Tare);
          sheet.SetValue(i + 2, 13, d.ReceivalWeight.TareUser?.Name ?? "");
          sheet.SetValue(i + 2, 14, d.ReceivalWeight.Gross);
          sheet.SetValue(i + 2, 15, d.ReceivalWeight.GrossUser?.Name ?? "");
          sheet.SetValue(i + 2, 16, d.ReceivalWeight.Gross - d.ReceivalWeight.Tare);
          sheet.SetValue(i + 2, 17, d.ReceivalWeight.Bags);
          sheet.SetValue(i + 2, 18, (d.ReceivalWeight.Gross - d.ReceivalWeight.Tare) - (d.InitialWeight.Gross - d.InitialWeight.Tare));
          sheet.SetValue(i + 2, 19, d.InitialWeight.Gross - d.InitialWeight.Tare);
          sheet.SetValue(i + 2, 20, d.InitialWeight.Bags);
          sheet.SetValue(i + 2, 21, daysInTransit?.Days);
          sheet.SetValue(i + 2, 22, d.Status == "Temp" ? "Received" : d.Status);
          sheet.SetValue(i + 2, 23, d.Booking.DriverName);
          sheet.SetValue(i + 2, 24, d.Booking.PassportNumber);
          sheet.SetValue(i + 2, 25, d.Booking.Transporter?.Name ?? d.Booking.OtherTransporter);
        }
        package.Save();
      }
      return "reports/" + file.Name;
    }

    public string PendingReport(ReportFilter filter)
    {
      var startDate = filter.Date ?? DateTime.Today;
      var endDate = filter.ToDate ?? startDate.AddDays(7);
      var dispatches = _context.Dispatch
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Where(d => d.Status == "Transit")
          .Where(d => d.InitialWeight.Gross != 0)
          .Where(d => d.InitialWeight.GrossAt >= startDate)
          .Where(d => d.InitialWeight.GrossAt <= endDate)
          .Where(d => d.Booking.TransportInstruction.ToLocation.Id == filter.OfficeId)
          .ToList();

      var title = Guid.NewGuid().ToString("n").Substring(0, 10);
      var file = new FileInfo(@"./wwwroot/reports/pending_report_" + title + ".xlsx");
      using (var package = new ExcelPackage(file))
      {
        var sheet = package.Workbook.Worksheets.Add("Dispatches");
        sheet.SetValue(1, 1, "Date");
        sheet.SetValue(1, 2, "W.B. Ticket #");
        sheet.SetValue(1, 3, "Transport Instruction #");
        sheet.SetValue(1, 4, "Contract #");
        sheet.SetValue(1, 5, "LPO #");
        sheet.SetValue(1, 6, "Location From");
        sheet.SetValue(1, 7, "Location To");
        sheet.SetValue(1, 8, "Product");
        sheet.SetValue(1, 9, "Description");
        sheet.SetValue(1, 10, "Truck Reg #");
        sheet.SetValue(1, 11, "Trailer Reg #");
        sheet.SetValue(1, 12, "Tare Weight");
        sheet.SetValue(1, 13, "Tare By");
        sheet.SetValue(1, 14, "Gross Weight");
        sheet.SetValue(1, 15, "Gross By");
        sheet.SetValue(1, 16, "Net Weight");
        sheet.SetValue(1, 17, "Bag Count");
        sheet.SetValue(1, 18, "Days in Transit");
        sheet.SetValue(1, 19, "Status");
        sheet.SetValue(1, 20, "Drivers Name");
        sheet.SetValue(1, 21, "License");
        sheet.SetValue(1, 22, "Transporter Company");

        for (int i = 0; i < dispatches.Count; i++)
        {
          var d = dispatches[i];
          var daysInTransit = d.ReceivalWeight?.GrossAt != null && d.InitialWeight?.GrossAt != null
              ? d.ReceivalWeight.GrossAt - d.InitialWeight.GrossAt
              : d.InitialWeight?.GrossAt != null
                  ? DateTime.Today - d.InitialWeight.GrossAt
                  : TimeSpan.Zero;
          sheet.SetValue(i + 2, 1, d.InitialWeight.GrossAt.ToString());
          sheet.SetValue(i + 2, 2, d.InitialWeight.TicketNumber);
          sheet.SetValue(i + 2, 3, d.Booking.TransportInstruction.KineticRef ?? d.Booking.TransportInstruction.Id.ToString() + " (*)");
          sheet.SetValue(i + 2, 4, d.Booking.TransportInstruction.Contract.ContractNumber);
          sheet.SetValue(i + 2, 5, d.Booking.TransportInstruction.Contract.ContractNumber);
          sheet.SetValue(i + 2, 6, d.Booking.TransportInstruction.FromLocation.Name);
          sheet.SetValue(i + 2, 7, d.Booking.TransportInstruction.ToLocation.Name);
          sheet.SetValue(i + 2, 8, d.Booking.TransportInstruction.Product.Name);
          sheet.SetValue(i + 2, 9, "");
          sheet.SetValue(i + 2, 10, d.Booking.NumberPlate);
          sheet.SetValue(i + 2, 11, d.Booking.TrailerNumber);
          sheet.SetValue(i + 2, 12, d.InitialWeight.Tare);
          sheet.SetValue(i + 2, 13, d.InitialWeight.TareUser.Name);
          sheet.SetValue(i + 2, 14, d.InitialWeight.Gross);
          sheet.SetValue(i + 2, 15, d.InitialWeight.GrossUser.Name);
          sheet.SetValue(i + 2, 16, d.InitialWeight.Gross - d.InitialWeight.Tare);
          sheet.SetValue(i + 2, 17, d.InitialWeight.Bags);
          sheet.SetValue(i + 2, 17, daysInTransit?.Days);
          sheet.SetValue(i + 2, 19, d.Status);
          sheet.SetValue(i + 2, 20, d.Booking.DriverName);
          sheet.SetValue(i + 2, 21, d.Booking.PassportNumber);
          sheet.SetValue(i + 2, 22, d.Booking.Transporter?.Name ?? d.Booking.OtherTransporter);
        }
        package.Save();
      }
      return "reports/" + file.Name;
    }
  }
}