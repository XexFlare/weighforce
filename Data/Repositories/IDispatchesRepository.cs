using System;
using System.Collections.Generic;
using AutoMapper;
using WeighForce.Filters;
using WeighForce.Models;
using WeighForce.Wrappers;

namespace WeighForce.Data.Repositories
{
    public interface IDispatchesRepository
    {
        PagedQuery<IEnumerable<Dispatch>> GetClientDispatches(long OfficeId, PaginationFilter filter);
        IEnumerable<DayInfo> GetChart(long OfficeId, int days, string type);
        Dashboard GetDashboard(long OfficeId, int days, DateTime from, DateTime to);
        DispatchReport GetDispatchReport();
        PagedQuery<IEnumerable<DispatchFirstWeightDto>> GetPendingDispatches(long OfficeId, PaginationFilter filter, IMapper mapper);
        PagedQuery<IEnumerable<Dispatch>> GetAllWagons(long OfficeId, PaginationFilter filter, IMapper mapper);
        PagedQuery<IEnumerable<Dispatch>> GetPendingWagons(long OfficeId, PaginationFilter filter, IMapper mapper);
        PagedQuery<IEnumerable<Dispatch>> GetTempDispatches(long OfficeId, PaginationFilter filter);
        PagedQuery<IEnumerable<Dispatch>> GetDispatchesDuring(long OfficeId, long id);
        PagedQuery<IEnumerable<Dispatch>> GetSuspendedDispatches(long OfficeId, PaginationFilter filter);
        PagedQuery<IEnumerable<DispatchFirstWeightDto>> GetProcessedDispatches(long OfficeId, PaginationFilter filter, IMapper mapper);
        PagedQuery<IEnumerable<Dispatch>> GetOverweights(long OfficeId, PaginationFilter filter);
        PagedQuery<IEnumerable<Dispatch>> GetUnderweights(long OfficeId, PaginationFilter filter);
        IEnumerable<Dispatch> Search(long OfficeId, string query);
        Dispatch GetDispatch(long id);
        bool Print(long id);
        Dispatch DiscardDispatch(long id);
        Dispatch PostWeight(long id, ScaleWeight details, string userId);
        Dispatch ReassignDispatch(long id, long product, string numberPlate);
        Dispatch PostClientWeight(long id, ScaleWeight tare, ScaleWeight gross, string userId);
        Dispatch PostInitialWeight(long id, ScaleWeight tare, ScaleWeight gross, string userId);
        Dispatch AddDispatch(Dispatch dispatch);
        PagedQuery<IEnumerable<Dispatch>> GetToSend(IMapper mapper);
    }
}