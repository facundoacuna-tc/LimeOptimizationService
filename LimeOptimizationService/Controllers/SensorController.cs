using System.Security.Claims;
using LOS.Common.Entities;
using LOS.Common.Requests;
using LOS.Services;
using LOS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LimeOptimizationService.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public SensorController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [Authorize]        
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<SensorValue>>> GetAll()
        {
            var result = await _sensorService.GetAllValues();
            return Ok(result);
        }


        [Authorize]
        [HttpGet("GetAllByFilter")]
        public async Task<ActionResult<IEnumerable<SensorValue>>> GetAllByFilter([FromBody] SensorFilterRequest request)
        {
            var result = await _sensorService.GetAllValuesByFilter(request.Sensor, request.SensorType, request.Since, request.To);
            return Ok(result);
        }




        //--------------

        //[Authorize]
        //[HttpGet("GetAll")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<ActionResult<IEnumerable<Sensor>>> GetAll()
        //{
        //    var result = await _sensorService.GetAll();
        //    return Ok(result);
        //}



    }
}
