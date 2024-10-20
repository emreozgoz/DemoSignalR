using API.HubServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HubServiceController(HubService hubService) : ControllerBase
    {
        [HttpGet("GetMembers/{groupName}")]
        public IActionResult GetMembers(string groupName)
        {
            return Ok(hubService.GetMembers(groupName));
        }
        [HttpGet("availableGroups")]
        public IActionResult GetAvailableGroups()
        {
            return Ok(hubService.GetAvailableGroups());
        }
        [HttpGet("GetUserGroupName/{connectionId}")]
        public IActionResult GetUserGroupName(string connectionId)
        {
            return Ok(hubService.GetUserGroupName(connectionId));
        }
    }
}
