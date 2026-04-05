using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WeighForce.Data.Repositories;
using WeighForce.Filters;
using WeighForce.Models;
using WeighForce.Wrappers;

namespace WeighForce.Data.EF
{
  static class DispatchStatus
  {
    public const string BOOKING = "Booking";
    public const string DISPATCHING = "Dispatching";
    public const string DISCARDED = "Discarded";
    public const string TRANSIT = "Transit";
    public const string HELD = "Held";
    public const string TEMP = "Temp";
    public const string PROCESSED = "Processed";
  }
  public class DispatchesRepository : IDispatchesRepository
  {
    private readonly ApplicationDbContext _context;

    public static Expression<Func<Dispatch, bool>> IsActive()
    {
      return d => !d.IsDeleted && d.Status != DispatchStatus.DISCARDED;
    }
    public static Expression<Func<Dispatch, bool>> InPeriod(DateTime From, DateTime To)
    {
      return d => d.Booking.CreatedAt >= From && d.Booking.CreatedAt <= To.AddDays(1);
    }
    public static Expression<Func<Dispatch, bool>> IsManual()
    {
      return d => d.Status == DispatchStatus.TEMP;
    }
    public static Expression<Func<Dispatch, bool>> IsNotManual()
    {
      return d => d.Status != DispatchStatus.TEMP;
    }
    public static Expression<Func<Dispatch, bool>> IsPending(long OfficeId)
    {
      return d => (d.Booking.VehicleType == "Truck" && d.Booking.TransportInstruction.FromLocation.Id == OfficeId && d.Status == DispatchStatus.DISPATCHING) ||
          (d.Booking.TransportInstruction.FromLocation.Id == OfficeId && d.Booking.VehicleType == "Wagon" && d.Status == DispatchStatus.HELD) ||
          (d.Booking.TransportInstruction.FromLocation.Id == OfficeId && d.Status == DispatchStatus.BOOKING) ||
          (d.Booking.TransportInstruction.ToLocation.Id == OfficeId && d.Booking.VehicleType == "Wagon" && d.Status == DispatchStatus.HELD) ||
          (d.Booking.TransportInstruction.ToLocation.Id == OfficeId && d.Booking.VehicleType == "Loco") ||
          (d.Booking.TransportInstruction.ToLocation.Id == OfficeId && d.Status != DispatchStatus.PROCESSED && d.Status != DispatchStatus.HELD);
    }
    public DispatchesRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public Dispatch GetDispatch(long id)
    {
      return _context.Dispatch
          .Include(d => d.Booking)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Product)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Contract)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.User)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser)
          .FirstOrDefault(d => d.Id == id);
    }

    public PagedQuery<IEnumerable<DispatchFirstWeightDto>> GetPendingDispatches(long OfficeId, PaginationFilter filter, IMapper mapper)
    {
      var query = _context.Dispatch
          .Where(IsActive())
          .Where(IsNotManual())
          .Where(InPeriod(filter.From, filter.To))
          .Where(d => filter.Type == "Wagon" ? 
                d.Booking.VehicleType.Contains(filter.Type) || d.Booking.VehicleType.Contains("Loco")
                : d.Booking.VehicleType.Contains(filter.Type))
          .Where(IsPending(OfficeId))
          .Include(d => d.Booking)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Product)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Contract)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.User)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.Branch)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      var records = query
        .OrderByDescending(d => d.Booking.CreatedAt)
        .Skip((filter.Page - 1) * filter.Size)
        .Take(filter.Size)
        .ToList();
      IEnumerable<DispatchFirstWeightDto> patched = records
      .Select(dispatch =>
      {
        if (dispatch.Booking.VehicleType != "Wagon")
          return mapper.Map<DispatchFirstWeightDto>(dispatch);
        var firstMass = _context.Dispatch
          .Include(d => d.ReceivalWeight)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser)
          .Where(d => d.Booking.NumberPlate == dispatch.Booking.NumberPlate)
          .FirstOrDefault()?.ReceivalWeight;
        var mapped = mapper.Map<DispatchFirstWeightDto>(dispatch);
        mapped.FirstWeight = firstMass;
        return mapped;
      }).ToList();
      return new PagedQuery<IEnumerable<DispatchFirstWeightDto>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = patched
      };
    }
    public PagedQuery<IEnumerable<Dispatch>> GetPendingWagons(long OfficeId, PaginationFilter filter, IMapper mapper)
    {
      var query = _context.Dispatch
          .Where(InPeriod(filter.From, filter.To))
          .Where(d => d.Status == DispatchStatus.HELD)
          .Where(d => filter.Type == "Wagon" ?
                d.Booking.VehicleType.Contains(filter.Type) || d.Booking.VehicleType.Contains("Loco")
                : d.Booking.VehicleType.Contains(filter.Type))
          .Where(d => d.Booking.TransportInstruction.Product.Name == "Pending")
          .Where(d => d.Booking.TransportInstruction.ToLocation.Id == OfficeId)
          .Include(d => d.Booking)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Product)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Contract)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.User)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.Branch)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      var records = query
          .OrderByDescending(d => d.Booking.CreatedAt)
          .Skip((filter.Page - 1) * filter.Size)
          .Take(filter.Size)
          .ToList();
      records = records
          .OrderByDescending(d => d.Booking.CreatedAt)
          .ThenByDescending(x => x.Booking.PhoneNumber != null &&  x.Booking.PhoneNumber != "" ? int.Parse(x.Booking.PhoneNumber) : 0)
          .ToList();

      records.Reverse();
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = records
      };
    }
    public PagedQuery<IEnumerable<Dispatch>> GetAllWagons(long OfficeId, PaginationFilter filter, IMapper mapper)
    {
      var query = _context.Dispatch
          .Where(InPeriod(filter.From, filter.To))
          .Where(d => d.Status == DispatchStatus.HELD)
          .Where(d => filter.Type == "Wagon" ?
                d.Booking.VehicleType.Contains(filter.Type) || d.Booking.VehicleType.Contains("Loco")
                : d.Booking.VehicleType.Contains(filter.Type))
          .Where(d => d.Booking.TransportInstruction.ToLocation.Id == OfficeId)
          .Include(d => d.Booking)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Product)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Contract)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.User)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.Branch)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      var records = query
          .OrderByDescending(d => d.Booking.CreatedAt)
          .Skip((filter.Page - 1) * filter.Size)
          .Take(filter.Size)
          .ToList();
      records = records
          .OrderByDescending(d => d.Booking.CreatedAt)
          .ThenByDescending(x => x.Booking.PhoneNumber != null &&  x.Booking.PhoneNumber != "" ? int.Parse(x.Booking.PhoneNumber) : 0)
          .ToList();

      records.Reverse();
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = records
      };
    }
    public PagedQuery<IEnumerable<Dispatch>> GetTempDispatches(long OfficeId, PaginationFilter filter)
    {
      var query = _context.Dispatch
          .Where(IsActive())
          .Where(IsManual())
          .Where(InPeriod(filter.From, filter.To))
          .Where(d => d.Booking.TransportInstruction.ToLocation.Id == OfficeId)
          .Include(d => d.Booking)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = query
          .OrderByDescending(d => d.Booking.CreatedAt)
          .Skip((filter.Page - 1) * filter.Size)
          .Take(filter.Size)
          .ToList()
      };
    }

    public PagedQuery<IEnumerable<Dispatch>> GetDispatchesDuring(long OfficeId, long id)
    {
      var dispatch = _context.Dispatch.Where(d => d.Id == id)
          .Include(d => d.Booking)
          .FirstOrDefault();
      if (dispatch == null)
      {
        return null;
      }
      var query = _context.Dispatch
          .Where(d =>
              d.Booking.VehicleType == "Wagon" && d.Booking.TransportInstruction.ToLocation.Id == OfficeId && d.Status == DispatchStatus.TRANSIT &&
              d.Booking.CreatedAt > dispatch.Booking.CreatedAt.AddDays(-10) && d.Booking.CreatedAt < dispatch.Booking.CreatedAt
          )
          .Include(d => d.Booking)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.InitialWeight);
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = query.Count(),
        TotalPages = 1,
        Records = query
      .OrderByDescending(d => d.Booking.CreatedAt)
      .ToList()
      };
    }

    public PagedQuery<IEnumerable<Dispatch>> GetClientDispatches(long OfficeId, PaginationFilter filter)
    {
      var query = _context.Dispatch
          .Where(d =>
              d.Booking.TransportInstruction.ToLocation.IsClient &&
              d.Booking.TransportInstruction.FromLocation.Id == OfficeId && d.Status != DispatchStatus.PROCESSED
          && d.Booking.CreatedAt >= filter.From
              && d.Booking.CreatedAt <= filter.To.AddDays(1)
          )
          .Include(d => d.Booking)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = query
      .OrderByDescending(d => d.Booking.CreatedAt)
      .Skip((filter.Page - 1) * filter.Size)
      .Take(filter.Size)
      .ToList()
      };
    }

    public PagedQuery<IEnumerable<DispatchFirstWeightDto>> GetProcessedDispatches(long OfficeId, PaginationFilter filter, IMapper mapper)
    {
      var query = _context.Dispatch
          .Where(d =>
              ((d.Booking.TransportInstruction.ToLocation.Id == OfficeId && d.Status == DispatchStatus.PROCESSED)
              || (d.Booking.TransportInstruction.FromLocation.Id == OfficeId && (d.Status == DispatchStatus.PROCESSED || d.Status == DispatchStatus.TRANSIT)))
              && d.Booking.CreatedAt >= filter.From
              && d.Booking.CreatedAt <= filter.To.AddDays(1)
              && (filter.Type == "Wagon" ? 
                d.Booking.VehicleType.Contains(filter.Type) || d.Booking.VehicleType.Contains("Loco")
                : d.Booking.VehicleType.Contains(filter.Type))
          )
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Product)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.OldValue.Contract)
          .Include(d => d.Booking.TIChanges)
              .ThenInclude(b => b.User)
          .Include(d => d.Booking.Branch)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      var records = query
        .OrderByDescending(d => d.InitialWeight.GrossAt)
        .Skip((filter.Page - 1) * filter.Size)
        .Take(filter.Size)
        .ToList();
      IEnumerable<DispatchFirstWeightDto> patched = records
        .Select(dispatch =>
        {
          if (dispatch.ReceivalWeight?.Gross == null || dispatch.ReceivalWeight.Gross == 0)
            return mapper.Map<DispatchFirstWeightDto>(dispatch);

          var firstMass = _context.Dispatch
            .Include(d => d.ReceivalWeight)
            .Include(d => d.ReceivalWeight.GrossUser)
            .Include(d => d.ReceivalWeight.TareUser)
            .Where(d => d.Booking.NumberPlate == dispatch.Booking.NumberPlate)
            .FirstOrDefault()?.ReceivalWeight;
          var mapped = mapper.Map<DispatchFirstWeightDto>(dispatch);
          mapped.FirstWeight = firstMass;
          return mapped;
        }).ToList();
      return new PagedQuery<IEnumerable<DispatchFirstWeightDto>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = patched
      };
    }
    public PagedQuery<IEnumerable<Dispatch>> GetToSend(IMapper mapper)
    {
      var records = _context.Dispatch
          .Where(d => d.ToEmail)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .OrderByDescending(d => d.Booking.TrailerNumber)
          .ToList();
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = records.Count(),
        TotalPages = 1,
        Records = records
      };
    }
    public PagedQuery<IEnumerable<Dispatch>> GetOverweights(long OfficeId, PaginationFilter filter)
    {
      var query = _context.Dispatch
          .Where(d =>
              (d.Booking.TransportInstruction.ToLocation.Id == OfficeId || d.Booking.TransportInstruction.FromLocation.Id == OfficeId)
              && d.Status == DispatchStatus.PROCESSED
              && (d.InitialWeight.Gross < d.ReceivalWeight.Gross)
          && d.Booking.CreatedAt >= filter.From
              && d.Booking.CreatedAt <= filter.To.AddDays(1)
          )
          .Include(d => d.Booking)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = query
      .OrderByDescending(d => d.Booking.CreatedAt)
      .Skip((filter.Page - 1) * filter.Size)
      .Take(filter.Size)
      .ToList()
      };
    }
    public PagedQuery<IEnumerable<Dispatch>> GetUnderweights(long OfficeId, PaginationFilter filter)
    {
      var query = _context.Dispatch
          .Where(d =>
              (d.Booking.TransportInstruction.ToLocation.Id == OfficeId || d.Booking.TransportInstruction.FromLocation.Id == OfficeId)
              && d.Status == DispatchStatus.PROCESSED
              && (d.InitialWeight.Gross - d.InitialWeight.Tare - (d.ReceivalWeight.Gross - d.InitialWeight.Tare) > 5000)
          && d.Booking.CreatedAt >= filter.From
              && d.Booking.CreatedAt <= filter.To.AddDays(1)
          )
          .Include(d => d.Booking)
          .Include(d => d.Booking.Transporter)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser);
      return new PagedQuery<IEnumerable<Dispatch>>
      {
        TotalRecords = query.Count(),
        TotalPages = (int)Math.Ceiling((query.Count() + 0.0) / (filter.Size + 0.0)),
        Records = query
      .OrderByDescending(d => d.Booking.CreatedAt)
      .Skip((filter.Page - 1) * filter.Size)
      .Take(filter.Size)
      .ToList()
      };
    }

    public Dispatch PostClientWeight(long id, ScaleWeight tare, ScaleWeight gross, string userId)
    {
      var user = _context.ApplicationUser.FirstOrDefault(d => d.Id == userId);
      var dispatch = this.GetDispatch(id);
      if (dispatch == null)
      {
        return null;
      }
      if (user == null)
      {
        return null;
      }
      dispatch.SetReceivalGross(gross, user);
      dispatch.SetReceivalTare(tare, user);
      Update(dispatch);
      return dispatch;
    }

    public Dispatch PostInitialWeight(long id, ScaleWeight tare, ScaleWeight gross, string userId)
    {
      var user = _context.ApplicationUser.FirstOrDefault(d => d.Id == userId);
      var dispatch = GetDispatch(id);
      if (dispatch == null)
      {
        return null;
      }
      if (user == null)
      {
        return null;
      }
      if(dispatch.InitialWeight?.Tare > 0 || dispatch.InitialWeight?.Gross > 0)
      {
        return null;
      }
      dispatch.SetInitialTare(tare, user);
      dispatch.SetInitialGross(gross, user, false);
      dispatch.Status = DispatchStatus.PROCESSED;
      dispatch.ToEmail = true;
      Update(dispatch);
      return dispatch;
    }

    public Dispatch PostWeight(long id, ScaleWeight details, string userId)
    {
      var user = _context.ApplicationUser
      .Include(u => u.OfficeUsers)
      .FirstOrDefault(d => d.Id == userId);
      var dispatch = GetDispatch(id);
      if (dispatch == null)
      {
        return null;
      }
      _context.EventLog.Add(new EventLog
      {
        User = user,
        Resource = "Dispatch",
        ResourceId = dispatch.Id,
        Message = $"Truck Weighed: {dispatch.Booking.NumberPlate} - {details.Amount} kgs",
        CreatedAt = DateTime.UtcNow
      });
      if (user.OfficeUsers.Exists(o => o.OfficeId == dispatch.Booking.TransportInstruction.FromLocation.Id) && dispatch.Status != DispatchStatus.TEMP)
      {
        if (dispatch.InitialWeight == null || dispatch.InitialWeight.Tare == null || dispatch.InitialWeight.Tare == 0)
        {
          dispatch.SetInitialTare(details, user);
          Update(dispatch);
          return dispatch;
        }
        if (dispatch.InitialWeight.Gross == null || dispatch.InitialWeight.Gross == 0)
        {
          dispatch.SetInitialGross(details, user);
          Update(dispatch);
          return dispatch;
        }
      }
      if (user.OfficeUsers.Exists(o => o.OfficeId == dispatch.Booking.TransportInstruction.ToLocation.Id))
      {
        if (dispatch.ReceivalWeight == null || dispatch.ReceivalWeight.Gross == null || dispatch.ReceivalWeight.Gross == 0)
        {
          Console.WriteLine("Receival Gross");
          dispatch.SetReceivalGross(details, user);
          Update(dispatch);
          return dispatch;
        }
        if (dispatch.ReceivalWeight.Tare == null || dispatch.ReceivalWeight.Tare == 0)
        {
          Console.WriteLine("Receival Tare");
          dispatch.SetReceivalTare(details, user);
          Update(dispatch);
          return dispatch;
        }
      }
      return dispatch;
    }
    private void Update(Dispatch dispatch)
    {
      dispatch.UpdatedAt = DateTime.UtcNow;
      dispatch.SyncVersion++;
      _context.Dispatch.Update(dispatch);
      _context.SaveChanges();
    }

    public Dispatch AddDispatch(Dispatch dispatch)
    {
      dispatch.Booking = _context.Booking.Include(b => b.TareUser).FirstOrDefault(d => d.Id == dispatch.Booking.Id);
      dispatch.InitialWeight = _context.Weight.Add(new Weight
      {
        Tare = dispatch.Booking.TareWeight,
        TareUser = dispatch.Booking.TareUser,
        TareAt = dispatch.Booking.TareAt
      }).Entity;
      dispatch.ReceivalWeight = _context.Weight.Add(new Weight { }).Entity;
      _context.Dispatch.Add(dispatch);
      _context.SaveChanges();
      return dispatch;
    }

    public Dashboard GetDashboard(long OfficeId, int days, DateTime from, DateTime to)
    {
      if (from > DateTime.UnixEpoch)
      {
        var nacalaDate = DateTime.Today.AddDays(-7).ToLocalTime();
        var dispatches = _context.Dispatch
            .Where(d => DateTime.Compare((DateTime)d.InitialWeight.TareAt, from) >= 0 || DateTime.Compare((DateTime)d.ReceivalWeight.GrossAt, to) >= 0
            || (d.Booking.TransportInstruction.FromLocation.Name == "Nacala" && DateTime.Compare((DateTime)d.InitialWeight.TareAt, nacalaDate) >= 0))
        .Include(d => d.Booking)
        .Include(d => d.Booking.TransportInstruction)
        .Include(d => d.Booking.TransportInstruction.FromLocation)
        .Include(d => d.Booking.TransportInstruction.ToLocation)
        .Include(d => d.InitialWeight)
        .Include(d => d.ReceivalWeight)
        .ToList();
        var TopDispatches = dispatches
        .Where(d => (d.InitialWeight?.TareAt != null && ((DateTime)d.InitialWeight.TareAt).ToLocalTime() > from) || d.Booking?.TransportInstruction?.FromLocation?.Name == "Nacala")
        .GroupBy(d => d.Booking?.TransportInstruction?.FromLocation?.Name)
        .OrderByDescending(d => d.Count())
        .Take(5)
        .Select(g => new ChartInfo
        {
          Name = g.Key ?? "Other",
          Value = g.Count()
        }).ToList();

        var TopReceivals = dispatches
            .Where(d => d.ReceivalWeight?.GrossAt != null && ((DateTime)d.ReceivalWeight.GrossAt).ToLocalTime() > from)
            .GroupBy(d => d.Booking?.TransportInstruction?.ToLocation?.Name)
            .OrderByDescending(d => d.Count())
            .Take(5)
            .Select(g => new ChartInfo
            {
              Name = g.Key ?? "Other",
              Value = g.Count()
            }).ToList();
        return new Dashboard
        {
          TopDispatches = TopDispatches,
          TopReceivals = TopReceivals,
        };
      }
      else
      {
        var date = DateTime.Today.AddDays(-days).ToLocalTime();
        var nacalaDate = DateTime.Today.AddDays(-1).ToLocalTime();
        var today = DateTime.Today.ToLocalTime();
        var dispatches = _context.Dispatch
            .Where(d => DateTime.Compare((DateTime)d.InitialWeight.TareAt, date) >= 0 || DateTime.Compare((DateTime)d.ReceivalWeight.GrossAt, date) >= 0
            || (d.Booking.TransportInstruction.FromLocation.Name == "Nacala" && DateTime.Compare((DateTime)d.InitialWeight.TareAt, nacalaDate) >= 0))
        .Include(d => d.Booking)
        .Include(d => d.Booking.TransportInstruction)
        .Include(d => d.Booking.TransportInstruction.FromLocation)
        .Include(d => d.Booking.TransportInstruction.ToLocation)
        .Include(d => d.InitialWeight)
        .Include(d => d.ReceivalWeight)
        .ToList();
        var TopDispatches = dispatches
        .Where(d => (d.InitialWeight?.TareAt != null && DateTime.Compare(((DateTime)d.InitialWeight.TareAt).ToLocalTime(), today) >= 0)
            || (d.Booking?.TransportInstruction?.FromLocation?.Name == "Nacala" && d.InitialWeight?.TareAt != null && DateTime.Compare(((DateTime)d.InitialWeight.TareAt).ToLocalTime(), nacalaDate) >= 0))
        .GroupBy(d => d.Booking?.TransportInstruction?.FromLocation?.Name)
        .OrderByDescending(d => d.Count())
        .Take(5)
        .Select(g => new ChartInfo
        {
          Name = g.Key ?? "Other",
          Value = g.Count()
        }).ToList();

        var TopReceivals = dispatches
            .Where(d => d.ReceivalWeight?.GrossAt != null && DateTime.Compare(((DateTime)d.ReceivalWeight.GrossAt).ToLocalTime(), today) >= 0)
            .GroupBy(d => d.Booking?.TransportInstruction?.ToLocation?.Name)
            .OrderByDescending(d => d.Count())
            .Take(5)
            .Select(g => new ChartInfo
            {
              Name = g.Key ?? "Other",
              Value = g.Count()
            }).ToList();
        var dailyDispatchData = dispatches
            .Where(d => d.Booking?.TransportInstruction?.FromLocation?.Id == OfficeId && d.InitialWeight?.TareAt != null)
            .OrderBy(d => d.InitialWeight.TareAt)
            .Select((d) => new
            {
              Day = ((DateTime)d.InitialWeight.TareAt).ToLocalTime().Day + "/" + ((DateTime)d.InitialWeight.TareAt).ToLocalTime().Month,
              ((DateTime)d.InitialWeight.TareAt).ToLocalTime().Date,
              Weight = d.InitialWeight?.Gross > 0 ? d.InitialWeight.Gross - d.InitialWeight.Tare : 0
            })
            .GroupBy(d => d.Day);

        var dailyReceivalData = dispatches
            .Where(d => d.Booking?.TransportInstruction?.ToLocation?.Id == OfficeId && d.ReceivalWeight?.GrossAt != null)
            .OrderBy(d => d.ReceivalWeight.GrossAt)
            .Select(d => new
            {
              Day = ((DateTime)d.ReceivalWeight.GrossAt).ToLocalTime().Day + "/" + ((DateTime)d.ReceivalWeight.GrossAt).ToLocalTime().Month,
              ((DateTime)d.ReceivalWeight.GrossAt).ToLocalTime().Date,
              Weight = d.ReceivalWeight?.Tare > 0 ? d.ReceivalWeight.Gross - d.ReceivalWeight.Tare : d.InitialWeight?.Tare > 0 ? d.ReceivalWeight.Gross - d.InitialWeight.Tare : 0
            })
            .GroupBy(d => d.Day);

        var DailyDispatches = new List<ChartInfo>() { };
        var DailyReceivals = new List<ChartInfo>() { };
        var DailyDispatchedTons = new List<ChartInfo>() { };
        var DailyReceivedTons = new List<ChartInfo>() { };
        for (int i = days - 1; i >= 0; i--)
        {
          var dated = DateTime.Today.AddDays(-i);
          var dispatched = new ChartInfo
          {
            Name = dated.Day + "/" + dated.Month,
            Value = dailyDispatchData.Where(d => DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i)) >= 0
             && DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i + 1)) < 0).FirstOrDefault()?.Count() ?? 0,
            Sub = dailyDispatchData.Where(d => DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i)) >= 0
             && DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i + 1)) < 0).FirstOrDefault()?.Sum(d => d.Weight) / 1000 ?? 0
          };

          var received = new ChartInfo
          {
            Name = dated.Day + "/" + dated.Month,
            Value = dailyReceivalData.Where(d => DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i)) >= 0
             && DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i + 1)) < 0).FirstOrDefault()?.Count() ?? 0,
            Sub = dailyReceivalData.Where(d => DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i)) >= 0
             && DateTime.Compare(d.FirstOrDefault()?.Date ?? DateTime.UnixEpoch, DateTime.Today.AddDays(-i + 1)) < 0).FirstOrDefault()?.Sum(d => d.Weight) / 1000 ?? 0
          };
          DailyDispatches.Add(dispatched);
          DailyReceivals.Add(received);
        }
        return new Dashboard
        {
          DailyDispatches = DailyDispatches,
          DailyReceivals = DailyReceivals,
          TopDispatches = TopDispatches,
          TopReceivals = TopReceivals,
        };
      }
    }

    public DispatchReport GetDispatchReport()
    {
      var date = DateTime.Today.AddDays(-2);
      var dispatches = _context.Dispatch
          .Where(d => DateTime.Compare((DateTime)d.InitialWeight.TareAt, date) >= 0 || DateTime.Compare((DateTime)d.ReceivalWeight.GrossAt, date) >= 0)
          .Include(d => d.Booking)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .ToList();
      var locations = new List<string>() { };

      var summary = dispatches
          .Select(d => new
          {
            To = d.Booking?.TransportInstruction?.ToLocation?.Name ?? "",
            From = d.Booking?.TransportInstruction?.FromLocation?.Name ?? "",
            Received = d.ReceivalWeight?.GrossAt != null ? ((DateTime)d.ReceivalWeight.GrossAt).ToLocalTime().Day + "/" + ((DateTime)d.ReceivalWeight.GrossAt).ToLocalTime().Month : "",
            Dispatched = d.InitialWeight?.TareAt != null ? ((DateTime)d.InitialWeight.TareAt).ToLocalTime().Day + "/" + ((DateTime)d.InitialWeight.TareAt).ToLocalTime().Month : "",
          }
      ).ToList();
      summary.ForEach(d =>
      {
        if (!locations.Any(s => s == d.From))
          locations.Add(d.From);
        if (!locations.Any(s => s == d.To))
          locations.Add(d.To);
      });
      var allDispatched = summary
          .Where(d => d.Dispatched != "")
          .GroupBy(d => d.Dispatched);
      var AllReceived = summary
          .Where(d => d.Received != "")
          .GroupBy(d => d.Received);

      var locatedDispatches = allDispatched.SelectMany(g =>
          g.GroupBy(dg => dg.From)
              .Select(dg => new
              {
                Label = dg.Key,
                Date = g.Key,
                Count = dg.Count()
              })
          );
      var locatedReceivals = AllReceived.SelectMany(g =>
          g.GroupBy(dg => dg.To)
              .Select(dg => new
              {
                Label = dg.Key,
                Date = g.Key,
                Count = dg.Count()
              })
          );
      var labels = new List<string>() { };
      var received = new List<int>() { };
      var dispatched = new List<int>() { };
      var goodLocations = new List<string>() { };
      for (int i = -2; i <= 0; i++)
      {
        var d = DateTime.Today.AddDays(i).Day + "/" + DateTime.Today.AddDays(i).Month;
        locations.ForEach(loc =>
        {
          var receivedCount = locatedReceivals.FirstOrDefault(r => r.Date == d && r.Label == loc)?.Count ?? 0;
          var dispatchedCount = locatedDispatches.FirstOrDefault(r => r.Date == d && r.Label == loc)?.Count ?? 0;
          if (!(receivedCount == 0 && dispatchedCount == 0) && !goodLocations.Any(l => l == loc))
          {
            goodLocations.Add(loc);
          }
        });
      }
      for (int i = -2; i <= 0; i++)
      {
        var d = DateTime.Today.AddDays(i).Day + "/" + DateTime.Today.AddDays(i).Month;
        goodLocations.ForEach(loc =>
        {
          var receivedCount = locatedReceivals.FirstOrDefault(r => r.Date == d && r.Label == loc)?.Count ?? 0;
          var dispatchedCount = locatedDispatches.FirstOrDefault(r => r.Date == d && r.Label == loc)?.Count ?? 0;
          labels.Add(loc + " " + d);
          received.Add(receivedCount);
          dispatched.Add(dispatchedCount);
        });
      }
      return new DispatchReport
      {
        Labels = labels,
        Dispatched = dispatched,
        Received = received
      };
    }

    public IEnumerable<DayInfo> GetChart(long OfficeId, int days, string type)
    {
      var date = DateTime.UtcNow.AddDays(-days);
      var Days = new List<DayInfo>();
      if (type == "Dispatched")
      {
        var received = _context.Dispatch
         .Where(d => d.Booking.TransportInstruction.FromLocation.Id == OfficeId
            && DateTime.Compare((DateTime)d.InitialWeight.GrossAt, date) >= 0)
        .Include(d => d.InitialWeight)
        .ToList();
        for (int i = days - 1; i >= 0; i--)
        {
          var dated = received.Where(d =>
           DateTime.Compare((DateTime)d.InitialWeight.GrossAt, DateTime.Today.AddDays(-i)) >= 0
           && DateTime.Compare((DateTime)d.InitialWeight.GrossAt, DateTime.Today.AddDays(-i + 1)) < 0)
           .Count();
          var day = new DayInfo
          {
            Day = DateTime.Today.AddDays(-i).ToShortDateString().Substring(0, 5),
            Trucks = dated
          };
          Days.Add(day);
        }
      }
      else
      {
        var received = _context.Dispatch
            .Where(d => d.Booking.TransportInstruction.ToLocation.Id == OfficeId
            && DateTime.Compare((DateTime)d.ReceivalWeight.TareAt, date) >= 0)
            .Include(d => d.ReceivalWeight)
            .ToList();
        for (int i = days - 1; i >= 0; i--)
        {
          var dated = received.Where(d =>
           DateTime.Compare((DateTime)d.ReceivalWeight.TareAt, DateTime.Today.AddDays(-i)) >= 0
           && DateTime.Compare((DateTime)d.ReceivalWeight.TareAt, DateTime.Today.AddDays(-i + 1)) < 0)
           .Count();
          var day = new DayInfo
          {
            Day = DateTime.Today.AddDays(-i).ToShortDateString().Substring(0, 5),
            Trucks = dated
          };
          Days.Add(day);
        }
      }
      return Days;
    }

    public PagedQuery<IEnumerable<Dispatch>> GetSuspendedDispatches(long OfficeId, PaginationFilter filter)
    {
      throw new NotImplementedException();
    }

    public Dispatch DiscardDispatch(long id)
    {
      Dispatch discarded = _context.Dispatch
          .Include(d => d.Booking)
          .Include(d => d.Booking.TransportInstruction)
          .Include(d => d.InitialWeight)
          .Include(d => d.ReceivalWeight)
          .Include(d => d.InitialWeight.GrossUser)
          .Include(d => d.InitialWeight.TareUser)
          .Include(d => d.ReceivalWeight.GrossUser)
          .Include(d => d.ReceivalWeight.TareUser)
          .Where(d => d.Id == id)
          .FirstOrDefault();
      if (discarded == null)
      {
        return null;
      }
      discarded.Status = "Discarded";
      Update(discarded);
      _context.SaveChanges();
      return discarded;
    }
    public Dispatch ReassignDispatch(long id, long product, string numberPlate)
    {
      Dispatch dispatch = _context.Dispatch
          .Include(d => d.Booking)
          .Include(d => d.Booking.TransportInstruction.FromLocation)
          .Include(d => d.Booking.TransportInstruction.ToLocation)
          .Include(d => d.Booking.TransportInstruction.Contract)
          .Include(d => d.Booking.TransportInstruction.Product)
          .Include(d => d.ReceivalWeight)
          .Where(d => d.Id == id)
          .FirstOrDefault();
      var ti = _context.TransportInstruction
        .Include(d => d.FromLocation)
        .Include(d => d.ToLocation)
        .Include(d => d.Contract)
        .Include(d => d.Product)
        .Where(d => d.Product.Id == product
          && d.FromLocation.Id == dispatch.Booking.TransportInstruction.FromLocation.Id
          && d.Contract.Id == dispatch.Booking.TransportInstruction.Contract.Id
          && d.ToLocation.Id == dispatch.Booking.TransportInstruction.ToLocation.Id
        ).FirstOrDefault();

      if (dispatch == null) return null;
      // var pendingProduct = _context.Product.Where(c => c.Name == "Pending").FirstOrDefault();
      // if (dispatch.Booking.TransportInstruction.Product.Id != pendingProduct.Id) return dispatch;
      if (ti == null)
      {
        Product selectedProduct = _context.Product.Where(p => p.Id == product).FirstOrDefault();
        if (selectedProduct == null) return dispatch;
        Console.WriteLine("abs");
        Console.WriteLine(JsonConvert.SerializeObject(selectedProduct));
        var newTI = new TransportInstruction
        {
          FromLocation = dispatch.Booking.TransportInstruction.FromLocation,
          Contract = dispatch.Booking.TransportInstruction.Contract,
          ToLocation = dispatch.Booking.TransportInstruction.ToLocation,
          Product = selectedProduct
        };
        _context.TransportInstruction.Add(newTI);
        _context.SaveChanges();
        Console.WriteLine(JsonConvert.SerializeObject(newTI));

        ti = _context.TransportInstruction
          .Include(d => d.FromLocation)
          .Include(d => d.ToLocation)
          .Include(d => d.Contract)
          .Include(d => d.Product)
          .Where(t =>
            t.FromLocation.Id == newTI.FromLocation.Id
            && t.ToLocation.Id == newTI.ToLocation.Id
            && t.Contract.Id == newTI.Contract.Id
            && t.Product.Id == newTI.Product.Id
          ).FirstOrDefault();
      }
      dispatch.Booking.TransportInstruction = ti;
      dispatch.Booking.NumberPlate = numberPlate;
      Console.WriteLine(JsonConvert.SerializeObject(ti));
      Update(dispatch);
      _context.SaveChanges();
      return dispatch;
    }

    public bool Print(long id)
    {
      var dispatch = _context.Dispatch
      .Include(d => d.InitialWeight)
      .Include(d => d.ReceivalWeight)
      .FirstOrDefault(d => d.Id == id);
      if (dispatch != null)
      {
        dispatch.Print();
        _context.SaveChanges();
        return true;
      }
      return false;
    }

    public IEnumerable<Dispatch> Search(long OfficeId, string query)
    {
      return _context.Dispatch
        .Include(d => d.Booking)
        .Include(d => d.Booking.TIChanges)
            .ThenInclude(b => b.OldValue.Product)
        .Include(d => d.Booking.TIChanges)
            .ThenInclude(b => b.OldValue.Contract)
        .Include(d => d.Booking.TIChanges)
            .ThenInclude(b => b.User)
        .Include(d => d.Booking.Branch)
        .Include(d => d.Booking.Transporter)
        .Include(d => d.Booking.TransportInstruction)
        .Include(d => d.Booking.TransportInstruction.FromLocation)
        .Include(d => d.Booking.TransportInstruction.FromLocation.Country)
        .Include(d => d.Booking.TransportInstruction.ToLocation)
        .Include(d => d.Booking.TransportInstruction.ToLocation.Country)
        .Include(d => d.Booking.TransportInstruction.Product)
        .Include(d => d.Booking.TransportInstruction.Contract)
        .Include(d => d.InitialWeight)
        .Include(d => d.ReceivalWeight)
        .Include(d => d.InitialWeight.GrossUser)
        .Include(d => d.InitialWeight.TareUser)
        .Include(d => d.ReceivalWeight.GrossUser)
        .Include(d => d.ReceivalWeight.TareUser)
        .Where(d => d.Status != DispatchStatus.DISCARDED)
        .Where(d =>
            (d.Booking.TransportInstruction.ToLocation.Id == OfficeId
            || d.Booking.TransportInstruction.FromLocation.Id == OfficeId)
            && (d.Booking.NumberPlate.ToLower().Contains(query.ToLower())
            || d.InitialWeight.TicketNumber.Contains(query)
            || d.ReceivalWeight.TicketNumber.Contains(query))
        ).OrderByDescending(d => d.Booking.CreatedAt)
        .ToList();
    }
  }
}