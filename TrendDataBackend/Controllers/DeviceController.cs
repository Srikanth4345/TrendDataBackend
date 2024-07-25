using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendDataBackend.Models;
using TrendDataBackend.Repositories;
namespace TrendDataBackend.Controllers
{

    [Route("api/v1/")]
    [ApiController]

    public class DeviceController : ControllerBase
    {

        private readonly CloudTable _table;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IConfiguration _configuration;

        public DeviceController(IDeviceRepository deviceRepository, IConfiguration configuration)
        {
            _deviceRepository = deviceRepository;
            _configuration = configuration;
        }

        [HttpPost("{deviceId}/timeseries")]
        public async Task<IActionResult> PostDeviceData([FromRoute] string deviceId, [FromBody] TimeRangeModel dateRange)
        {
            try
            {
                if (dateRange == null || dateRange.StartDate == default || dateRange.EndDate == default)
                {
                    return BadRequest("Invalid date range parameters.");
                }
                TimeSpan difference = dateRange.EndDate - dateRange.StartDate;
                int differenceInDays = (int)difference.TotalDays;

                // Check if difference is greater than 30 days
                if (differenceInDays > 30)
                {
                    return BadRequest("Difference between start date and end date should not exceed 30 days.");
                }
                long startTime = new DateTimeOffset(dateRange.StartDate.ToUniversalTime()).ToUnixTimeSeconds();
                long endTime = new DateTimeOffset(dateRange.EndDate.ToUniversalTime()).ToUnixTimeSeconds();
                var deviceData  = await _deviceRepository.GetDeviceDataByTimeRangeAsync(deviceId, startTime, endTime);
                
                return Ok(deviceData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("devices")]
        public async Task<IActionResult> GetDataBetweenDatesFromBody([FromBody] TimeRangeModel dateRange)
        {
            try
            {
                TimeSpan difference = dateRange.EndDate - dateRange.StartDate;
                int differenceInDays = (int)difference.TotalDays;

               
                if (differenceInDays > 30)
                {
                    return BadRequest("Difference between start date and end date should not exceed 30 days.");
                }
                long startTime = (long)(dateRange.StartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
                long endTime = (long)(dateRange.EndDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
                var devices = await _deviceRepository.GetDeviceDataByTimeRange(startTime, endTime);
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }

}