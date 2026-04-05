using System.Collections.Generic;
using WeighForce.Filters;
using WeighForce.Models;
using WeighForce.Wrappers;

namespace WeighForce.Data.Repositories
{
    public interface IBookingsRepository
    {
        IEnumerable<Booking> GetAllBookings(long OfficeId, PaginationFilter filter);
        PagedQuery<IEnumerable<OsrData>> GetAllOsrData(PaginationFilter filter);
        IEnumerable<Booking> GetPendingBookings(long OfficeId, PaginationFilter filter);
        Booking GetBooking(long id);
        bool CheckIncoming(long locationId, string numberPlate, string ticketNumber);
        Booking PostBooking(Booking booking, string userId, long officeId);
        Booking PostTempBooking(Booking booking, string userId, long officeId);
        Booking SetNumberPlate(long id, string numberPlate);
        Booking UpdateTI(long id, long contractId, long productId);
        Booking ChangeTI(long id, long tiId, string userId);
    }
}