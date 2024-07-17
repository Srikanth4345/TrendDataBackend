using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendDataBackend.Models;
using TrendDataBackend.Repositories;

[Route("api/devices")]
[ApiController]
public class DeviceController : ControllerBase
{
    
    private readonly CloudTable _table;
    private readonly IDeviceRepository _deviceRepository;
    private readonly string _baseUrl;

    public DeviceController(IDeviceRepository deviceRepository, IConfiguration configuration)
    {
        _deviceRepository = deviceRepository;
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
    }


    [HttpGet]
    public async Task<ActionResult<List<Device>>> GetAllDevices()
    {
        try
        {
            var devices = await _deviceRepository.GetAllDevicesAsync();
            return Ok(devices);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{deviceId}")]
    public async Task<ActionResult<Device>> GetDeviceById(string deviceId)
    {
        try
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(device);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
   
    [HttpGet("data/between")]
    public async Task<IActionResult> GetDataBetweenDates([FromQuery] long startDate, [FromQuery] long endDate)
    {
        try
        {
           

            var data = await _deviceRepository.GetDataBetweenTimesAsync(startDate, endDate);
            if (!data.Any())
            {
                return Ok("No devices found");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
