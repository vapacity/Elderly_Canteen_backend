using Elderly_Canteen.Data.Dtos.WeekMenu;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Controllers
{
    [Route("api/menu")]
    [ApiController]
    //[AuthorizeRole("admin")]
    public class WeekMenuController : ControllerBase
    {
        private readonly IWeekMenuService _wmService;

        public WeekMenuController(IWeekMenuService wmService)
        {
            _wmService = wmService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddMenu(WmRequestDto request)
        {
            if (request == null)
            {
                return BadRequest();
            }
            var response =await _wmService.AddWM(request);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }
    }
}
