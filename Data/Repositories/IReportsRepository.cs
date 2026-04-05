using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeighForce.Data.EF;
using WeighForce.Filters;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface IReportsRepository
    {
        List<ReportLocation> DailyDispatches(string userId, ReportFilter filter);
        List<ReportLocation> DailyReceivals(string userId, ReportFilter filter);
        List<ReportLocation> DailyDispatchesEmail(string userId);
        List<ReportLocation> WeeklyDispatches(string userId, ReportFilter filter);
        List<ReportLocation> AnnualDispatches(string userId, ReportFilter filter);
        List<ReportLocation> WeeklyReceivals(string userId, ReportFilter filter);
        List<ReportLocation> AnnualReceivals(string userId, ReportFilter filter);
        List<ReportLocation> ShortageReport(string userId, ReportFilter filter);
        string DispatchedReport(ReportFilter filter);
        string ReceivedReport(ReportFilter filter);
        string PendingReport(ReportFilter filter);
        Task SendReportsAsync();
        Task SendTrainsReport();
        List<TransporterMismatch> SendTransporterMismatchReport();
    }
}