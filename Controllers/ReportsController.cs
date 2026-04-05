using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WeighForce.Data.Repositories;
using WeighForce.Filters;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsRepository _repo;

        public ReportsController(IReportsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("daily-dispatches")]
        public IActionResult DailyDispatchReport([FromQuery] ReportFilter filter)
            => Ok(_repo.DailyDispatches(User.FindFirst(ClaimTypes.NameIdentifier).Value, filter));

        [HttpGet("weekly-dispatches")]
        public IActionResult WeeklyDispatchReport([FromQuery] ReportFilter filter)
            => Ok(_repo.WeeklyDispatches(User.FindFirst(ClaimTypes.NameIdentifier).Value, filter));

        [HttpGet("annual-dispatches")]
        public IActionResult AnnualDispatchReport([FromQuery] ReportFilter filter)
          => Ok(_repo.AnnualDispatches(User.FindFirst(ClaimTypes.NameIdentifier).Value, filter));
        
        [HttpGet("daily-receivals")]
        public IActionResult DailyReceivalReport([FromQuery] ReportFilter filter)
            => Ok(_repo.DailyReceivals(User.FindFirst(ClaimTypes.NameIdentifier).Value, filter));

        [HttpGet("weekly-receivals")]
        public IActionResult WeeklyReceivalReport([FromQuery] ReportFilter filter)
            => Ok(_repo.WeeklyReceivals(User.FindFirst(ClaimTypes.NameIdentifier).Value, filter));

        [HttpGet("annual-receivals")]
        public IActionResult AnnualReceivalReport([FromQuery] ReportFilter filter)
          => Ok(_repo.AnnualReceivals(User.FindFirst(ClaimTypes.NameIdentifier).Value, filter));
        
        [HttpGet("shortages")]
        public IActionResult ShortageReport([FromQuery] ReportFilter filter)
          => Ok(_repo.ShortageReport(User.FindFirst(ClaimTypes.NameIdentifier).Value, filter));

        [HttpGet("dispatched")]
        public IActionResult DispatchedReport([FromQuery] ReportFilter filter)
            => Ok(new { url = _repo.DispatchedReport(filter) });
        [HttpGet("received")]
        public IActionResult ReceivedReport([FromQuery] ReportFilter filter)
            => Ok(new { url = _repo.ReceivedReport(filter) });
        [HttpGet("pending")]
        public IActionResult PendingReport([FromQuery] ReportFilter filter)
            => Ok(new { url = _repo.PendingReport(filter) });

        [HttpGet("send")]
        public async Task<IActionResult> SendReportsAsync()
        {
            await _repo.SendReportsAsync();
            return Ok();
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendTrainsReportsAsync()
        {
            await _repo.SendTrainsReport();
            return Ok();
        }
        [HttpGet("mismatch")]
        public IActionResult SendMismatchReport()
        {
            _repo.SendTransporterMismatchReport();
            return Ok();
        }
    }
}