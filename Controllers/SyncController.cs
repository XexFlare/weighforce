using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeighForce.Services;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly SyncService _syncService;

        public SyncController(SyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost]
        public IActionResult StartSyncAsync()
        {
            _ = _syncService.Sync();
            return Ok();
        }
    }
}