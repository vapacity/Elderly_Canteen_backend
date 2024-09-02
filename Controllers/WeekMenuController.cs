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

        [HttpDelete("remove")]
        public async Task<IActionResult> DeleteMenu(WmRequestDto request)
        {
            if (request == null)
            {
                return BadRequest();
            }
            var response = await _wmService.RemoveWM(request);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> GetWeek(DateTime date)
        {
            if (date == null)
            {
                return BadRequest();
            }
            var response = await _wmService.GetWeekMenuByDateAsync(date);
            if (response == null)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPut("uploadDiscount")]
        public async Task<IActionResult> UploadDiscount(DiscountRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }
            var response = await _wmService.UploadDiscount(dto);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPut("batch-discount")]
        public async Task<IActionResult> BatchDiscount(BatchRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }
            var response = await _wmService.BatchDiscount(dto);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet("getDiscount")]
        public async Task<IActionResult> GetAllDiscount(DateTime week)
        {
            if (week == null)
            {
                return BadRequest();
            }
            var response = await _wmService.GetAllDiscount(week);
            if (response == null)
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
