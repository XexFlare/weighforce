using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data;
using WeighForce.Models;

namespace WeighForce.Services
{
    public class SyncHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly OnTractContext _onTrack;

        public SyncHelper(ApplicationDbContext context, OnTractContext onTrack = null)
        {
            _context = context;
            _onTrack = onTrack;
        }

        public Dispatch GetLastTrain()
        {
            return _context.Dispatch
                .Include(d => d.Booking)
                .Where(d => d.Booking != null && d.Booking.VehicleType == "Wagon")
                .Where(d => d.Booking.wId != null)
                .OrderBy(d => d.Booking.CreatedAt)
                .LastOrDefault();
        }

        public List<OT_Train> GetInboundTrains(int days, bool past = false)
        {
            Dispatch last = GetLastTrain();
            List<OT_Train> trains;
            try
            {
                if (!past && last != null)
                {
                    trains = _onTrack.Trains
                        .Include(t => t.Wagons)
                        .ThenInclude(w => w.Tag)
                        .Where(t => t.Post_Date > last.Booking.CreatedAt)
                        .Where(s => s.Direction == "DOWN")
                        .Where(s => s.Site.Site_ID == 4)
                        .ToList();
                }
                else
                {
                    var startDate = DateTime.Today.AddDays(-days);
                    trains = _onTrack.Trains
                        .Where(t => t.Post_Date >= startDate)
                        .Where(s => s.Site.Site_ID == 4)
                        .Where(s => s.Direction == "DOWN")
                        .Include(t => t.Wagons
                        )
                        .ThenInclude(w => w.Tag)
                        .ToList();
                }
                return trains;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public IEnumerable<OT_Train> GetOutboundTrains(int days, bool past = false)
        {
            Dispatch last = null; // GetLastTrain();
            IEnumerable<OT_Train> trains;
            try
            {
                if (!past && last != null)
                {
                    trains = _onTrack.Trains
                        .Include(t => t.Wagons)
                        .ThenInclude(w => w.Tag)
                        .Where(t => t.Post_Date > last.Booking.CreatedAt)
                        .Where(s => s.Direction == "UP")
                        .Where(s => s.Site.Site_ID == 4)
                        .ToList();
                }
                else
                {
                    var startDate = DateTime.Today.AddDays(-days);
                    trains = _onTrack.Trains
                        .Where(t => t.Post_Date >= startDate)
                        .Where(s => s.Site.Site_ID == 4)
                        .Where(s => s.Direction == "UP")
                        .Include(t => t.Wagons
                        )
                        .ThenInclude(w => w.Tag)
                        .ToList();
                }
                return trains;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public TransportInstruction GetPendingTI(Office from, Office to)
        {
            var pending = _context.TransportInstruction
                .Where(o => o.Product.Name.Equals("Pending") && o.FromLocation.Id == from.Id && o.ToLocation.Id == to.Id)
                .Where(o => !o.IsDeleted)
                .FirstOrDefault();
            var pendingContract = _context.Contract.Where(c => c.ContractNumber == "Pending").FirstOrDefault();
            var pendingProduct = _context.Product.Where(c => c.Name == "Pending").FirstOrDefault();
            if (pending == null)
            {
                pending = _context.TransportInstruction.Add(
                    new TransportInstruction
                    {
                        Product = pendingProduct ?? new Product
                        {
                            Name = "Pending",
                            UpdatedAt = DateTime.UtcNow
                        },
                        Contract = pendingContract ?? new Contract
                        {
                            ContractNumber = "Pending",
                            UpdatedAt = DateTime.UtcNow
                        },
                        FromLocation = from,
                        ToLocation = to,
                        UpdatedAt = DateTime.UtcNow
                    }
                ).Entity;
                _context.SaveChanges();
            }
            return pending;
        }

        public Booking FindBookedWagon(string wagonNumber, DateTime arrivalDate, int skip = 0)
        {
            return _context.Booking
                .Where(b => b.NumberPlate != null)
                .Where(b => b.NumberPlate == wagonNumber || "0" + b.NumberPlate == wagonNumber)
                .Where(b => b.CreatedAt >= arrivalDate.AddDays(-14))
                .Where(b => b.CreatedAt < arrivalDate)
                .Skip(skip)
                .FirstOrDefault();
        }

        public Dispatch GetDispatch(string wagonNumber, DateTime arrivalDate, string status, int skip = 0)
        {
            Booking booking = FindBookedWagon(wagonNumber, arrivalDate, skip);
            if (booking == null) return null;
            return _context.Dispatch
                .Include(d => d.Booking)
                .Include(d => d.ReceivalWeight)
                .Where(d => d.Status == status && d.Booking.Id == booking.Id)
                .FirstOrDefault();
        }
        public Dispatch FindHeldDispatch(string wagonNumber, DateTime arrivalDate)
        {
            Dispatch dispatch;
            int i = 0;

            do
            {
                dispatch = GetDispatch(wagonNumber, arrivalDate, "Held", i++);
            } while (dispatch != null && dispatch.Status != "Held" && i < 10);

            return dispatch;
        }
        public Dispatch FindProcessedDispatch(string numberPlate, DateTime arrivalDate)
        {
            Dispatch dispatch;
            int i = 0;

            do
            {
                dispatch = GetDispatch(numberPlate, arrivalDate, "Processed", i++);
            } while (dispatch != null && dispatch.Status != "Processed" && i < 10);

            return dispatch;
        }
        public Dispatch FindInTransitDispatch(string numberPlate, DateTime arrivalDate)
        {
            Dispatch dispatch;
            int i = 0;

            do
            {
                dispatch = GetDispatch(numberPlate, arrivalDate, "Transit", i++);
            } while (dispatch != null && dispatch.Status != "Transit" && i < 10);

            return dispatch;
        }
        public void CompleteDispatch(Dispatch dispatch, OT_WagonData wagon, OT_Train train, Office office, ApplicationUser user)
        {
            dispatch.Status = "Processed";
            dispatch.Booking.wId = wagon.Wagon_Data_ID;
            if (dispatch.ReceivalWeight == null)
                dispatch.ReceivalWeight = new Weight
                {
                    Tare = (int)Math.Round(wagon.Tare_Mass * 1000, 0),
                    TareAt = train.Post_Date,
                    Gross = (int)Math.Round(wagon.Mass * 1000, 0),
                    GrossAt = train.Post_Date,
                    Office = office,
                    TareUser = user,
                    GrossUser = user,
                    UpdatedAt = DateTime.UtcNow
                };
            else
            {
                dispatch.ReceivalWeight.Gross = (int)Math.Round(wagon.Mass * 1000, 0);
                dispatch.ReceivalWeight.GrossAt = train.Post_Date;
                dispatch.ReceivalWeight.GrossUser = user;
                dispatch.ReceivalWeight.UpdatedAt = DateTime.UtcNow;
            }
            _context.Dispatch.Update(dispatch);
            _context.SaveChanges();
        }
        public void UpdateTare(Dispatch dispatch, OT_WagonData wagon, OT_Train train, Office office, ApplicationUser user)
        {
            dispatch.Booking.TareWeight = (int)Math.Round(wagon.Mass * 1000, 0);
            dispatch.Booking.UpdatedAt = DateTime.UtcNow;
            if (dispatch.ReceivalWeight == null)
                dispatch.ReceivalWeight = new Weight
                {
                    Office = office,
                    TareUser = user,
                    GrossUser = user
                };
            dispatch.ReceivalWeight.Tare = (int)Math.Round(wagon.Tare_Mass * 1000, 0);
            dispatch.ReceivalWeight.TareAt = train.Post_Date;
            dispatch.ReceivalWeight.UpdatedAt = DateTime.UtcNow;
            _context.Dispatch.Update(dispatch);
            _context.SaveChanges();
        }

        public void UpdatetMissingTare(Dispatch dispatch, OT_WagonData wagon, Office office, ApplicationUser user, TransportInstruction pending)
        {
            dispatch.Booking.TareWeight = (int)Math.Round(wagon.Mass * 1000, 0);
            dispatch.Booking.TareAt = wagon.Train.Post_Date;
            dispatch.ReceivalWeight.Tare = (int)Math.Round(wagon.Tare_Mass * 1000, 0);
            dispatch.ReceivalWeight.TareAt = wagon.Train.Post_Date;
            dispatch.ReceivalWeight.UpdatedAt = DateTime.UtcNow;
            _context.Dispatch.Update(dispatch);
            _context.SaveChanges();
        }
        public void PostMissingDispatch(OT_WagonData wagon, Office office, ApplicationUser user, TransportInstruction pending)
        {
            _context.Dispatch.Add(
                new Dispatch
                {
                    Booking = new Booking
                    {
                        VehicleType = wagon.Vehicle_Type == "W" ? "Wagon" : "Loco",
                        wId = wagon.Wagon_Data_ID,
                        NumberPlate = wagon.Number(),
                        TrailerNumber = wagon.Train.Message_ID.ToString(),
                        TransportInstruction = pending,
                        CreatedAt = wagon.Train.Post_Date,
                        TareWeight = 0,
                        TareUser = user,
                        TareAt = wagon.Train.Post_Date,
                        DeliveryNoteNumber = $"{wagon.Speed}",
                        PhoneNumber = $"{wagon.Wagon_No}",
                        UpdatedAt = DateTime.UtcNow
                    },
                    Status = "Held",
                    ReceivalWeight = new Weight
                    {
                        Tare = (int)Math.Round(wagon.Tare_Mass * 1000, 0),
                        TareAt = wagon.Train.Post_Date,
                        Gross = (int)Math.Round(wagon.Mass * 1000, 0),
                        GrossAt = wagon.Train.Post_Date,
                        Office = office,
                        TareUser = user,
                        GrossUser = user,
                        UpdatedAt = DateTime.UtcNow
                    },
                    UpdatedAt = DateTime.UtcNow
                }
            );
            _context.SaveChanges();
        }
        public void UpdateWagonMeta(OT_WagonData wagon, Booking booking, Dispatch bookedDispatch)
        {
            booking.VehicleType = wagon.Vehicle_Type == "W" ? "Wagon" : "Loco";
            booking.TrailerNumber = wagon.Train.Message_ID.ToString();
            booking.DeliveryNoteNumber = $"{wagon.Speed}";
            booking.PhoneNumber = $"{wagon.Wagon_No}";
            booking.UpdatedAt = DateTime.UtcNow;
            booking.SyncVersion = booking.SyncVersion + 1;
            bookedDispatch.UpdatedAt = DateTime.UtcNow;
            bookedDispatch.SyncVersion = bookedDispatch.SyncVersion + 1;
            _context.Booking.Update(booking);
            _context.Dispatch.Update(bookedDispatch);
        }
    }
    public class Sys
    {
        public static void log(dynamic message, int level = 0)
        {
            string spaces = "";
            for (int i = 0; i < level * 2; i++)
            {
                spaces += " ";
            }
            Console.WriteLine(spaces + "" + message);
        }
    }
}