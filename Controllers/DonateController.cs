using Elderly_Canteen.Data.Dtos.Login;
using Elderly_Canteen.Data.Dtos.Donate;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Dtos.AuthenticationDto;
using Elderly_Canteen.Services.Implements;

namespace Elderly_Canteen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonateController : ControllerBase
    {
        private readonly IDonateService _donateService;

        public DonateController(IDonateService donateService)
        {
            _donateService = donateService;
        }

        [HttpGet("getDonationList")]
        public async Task<IActionResult> GetList()
        {
            var response = await _donateService.GetDonateListAsync();
            return Ok(response);
        }

        [HttpPost("submitDonation")]
        public async Task<IActionResult> SubmitDonation([FromBody] DonateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var response = await _donateService.SubmitDonationAsync(request);
            return Ok(response);
        }
    }
}
