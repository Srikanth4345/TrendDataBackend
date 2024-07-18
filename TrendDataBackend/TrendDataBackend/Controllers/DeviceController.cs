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
    public async Task<IActionResult> GetDataBetweenDates([FromQuery] DateTime startDateUtc, [FromQuery] DateTime endDateUtc)
    {
        try
        {
            // Convert UTC dates to Unix timestamps
            long startTime = (long)(startDateUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
            long endTime = (long)(endDateUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;

            var data = await _deviceRepository.GetDataBetweenTimesAsync(startTime, endTime);

            // Convert DeviceEntity to Device model
            var devices = data.Select(entity => new Device
            {
                DeviceId = entity.DeviceId,
                TagId = entity.TagId,
                DeviceTimeStamp = entity.DeviceTimeStamp,
                Status = entity.Status,
                DeviceProfileId = entity.DeviceProfileId
            }).ToList();

            if (devices.Count == 0)
            {
                return NotFound("No devices found");
            }

            return Ok(devices);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


}