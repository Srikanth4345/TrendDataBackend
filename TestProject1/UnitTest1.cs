//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
////using TrendDataBackend.Controllers;
//using TrendDataBackend.Models;
//using TrendDataBackend.Repositories;

//namespace TrendDataBackend.Tests.Controllers
//{
//    [TestClass]
//    public class DeviceControllerTests
//    {
//        private Mock<IDeviceRepository> _mockDeviceRepository;
//        private Mock<IConfiguration> _mockConfiguration;
//        private DeviceController _controller;
//        public void Initialize()
//        {
//            _mockDeviceRepository = new Mock<IDeviceRepository>();
//            _mockConfiguration = new Mock<IConfiguration>();

//            // Create instance of DeviceController with mocks
//            _controller = new DeviceController(_mockDeviceRepository.Object, _mockConfiguration.Object);
//        }

//[TestMethod]
//public async Task PostDeviceData_ValidRequest_ReturnsOkResult()
//{
//    // Arrange
//    var deviceId = "device123";
//    var dateRange = new TimeRangeModel { StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow };
//    var deviceData = new List<DeviceData> { new DeviceData { DeviceId = deviceId, Timestamp = DateTime.UtcNow } };
//    _deviceRepositoryMock.Setup(repo => repo.GetDeviceDataByTimeRangeAsync(deviceId, It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(deviceData);

//    // Act        [TestInitialize]

//    var result = await _controller.PostDeviceData(deviceId, dateRange) as OkObjectResult;

//    // Assert
//    Assert.IsNotNull(result);
//    Assert.AreEqual(200, result.StatusCode);
//    Assert.AreEqual(deviceData, result.Value);
//}

//[TestMethod]
//public async Task PostDeviceData_InvalidDateRange_ReturnsBadRequestResult()
//{
//    // Arrange
//    var deviceId = "63d0d1be-3df7-45f4-9b1a-73126e960190";
//    var startDateString = "2024-07-23T15:57:16.379Z";
//    var endDateString = "2024-07-23T16:05:16.379Z";

//    var dateRange = new TimeRangeModel
//    {
//        StartDate = DateTime.Parse(startDateString),
//        EndDate = DateTime.Parse(endDateString)
//    };

//    // Act
//    var result = await _controller.PostDeviceData(deviceId, dateRange) as BadRequestObjectResult;

//    // Assert
//    Assert.IsNotNull(result); // Ensure result is not null
//    Assert.AreEqual(400, result.StatusCode);
//    Assert.AreEqual("Invalid date range parameters.", result.Value);
//}

//[TestMethod]
//public async Task PostDeviceData_InternalServerError_ReturnsStatusCode500()
//{
//    // Arrange
//    var startDateString = "2024-07-23T15:57:16.379Z";
//    var endDateString = "2024-07-23T16:05:16.379Z";

//    var dateRange = new TimeRangeModel
//    {
//        StartDate = DateTime.Parse(startDateString),
//        EndDate = DateTime.Parse(endDateString)
//    };
//    _deviceRepositoryMock.Setup(repo => repo.GetDataBetweenDatesFromBody(dateRange) as BadRequestObjectResult;

//    // Act
//    var result = await _controller.GetDataBetweenDatesFromBody(dateRange) as StatusCodeResult;

//    // Assert
//    Assert.IsNotNull(result);
//    Assert.AreEqual(500, result.StatusCode);
//}

//[TestMethod]
//public async Task GetDataBetweenDates_ValidRequest_ReturnsOkResult()
//{
//    // Arrange
//    var startDateUtc = DateTime.UtcNow.AddDays(-1);
//    var endDateUtc = DateTime.UtcNow;
//    var deviceData = new List<DeviceData> { new DeviceData { DeviceId = "device123", Timestamp = DateTime.UtcNow } };
//    _deviceRepositoryMock.Setup(repo => repo.GetDataBetweenTimesAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(deviceData);

//    // Act
//    var result = await _controller.GetDataBetweenDates(startDateUtc, endDateUtc) as OkObjectResult;

//    // Assert
//    Assert.IsNotNull(result);
//    Assert.AreEqual(200, result.StatusCode);
//    Assert.AreEqual(deviceData, result.Value);
//}

//[TestMethod]
//public async Task GetDataBetweenDates_InternalServerError_ReturnsStatusCode500()
//{
//    // Arrange
//    var startDateUtc = DateTime.UtcNow.AddDays(-1);
//    var endDateUtc = DateTime.UtcNow;
//    _deviceRepositoryMock.Setup(repo => repo.GetDataBetweenTimesAsync(It.IsAny<long>(), It.IsAny<long>())).Throws(new Exception("Internal server error"));

//    // Act
//    var result = await _controller.GetDataBetweenDatesFromBody(startDateUtc, endDateUtc) as StatusCodeResult;

//    // Assert
//    Assert.IsNotNull(result);
//    Assert.AreEqual(500, result.StatusCode);
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendDataBackend.Controllers;
using TrendDataBackend.Models;
using TrendDataBackend.Repositories;

[TestClass]
public class DeviceControllerTests
{
    private readonly Mock<IDeviceRepository> _mockDeviceRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly DeviceController _controller;

    public DeviceControllerTests()
    {
        _mockDeviceRepository = new Mock<IDeviceRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        _controller = new DeviceController(_mockDeviceRepository.Object, _mockConfiguration.Object);

    }

    [TestMethod]
    public async Task PostDeviceData_ValidDateRange_ReturnsOk()
    {
        // Arrange
        var deviceId = "63d0d1be-3df7-45f4-9b1a-73126e960190";
        var dateRange = new TimeRangeModel
        {
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow
        };

        var deviceData = new List<DeviceDTO>(); // Populate with mock data as needed
        _mockDeviceRepository.Setup(repo => repo.GetDeviceDataByTimeRangeAsync(deviceId, It.IsAny<long>(), It.IsAny<long>()))
                             .ReturnsAsync(deviceData);

        // Act
        var result = await _controller.PostDeviceData(deviceId, dateRange);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreSame(deviceData, okResult.Value); // Adjust assertion based on your expected behavior
    }

    //[TestMethod]
    //public async Task PostDeviceData_InvalidDateRange_ReturnsBadRequestResult()
    //{
    //    // Arrange
    //    var deviceId = "testDeviceId";
    //    var dateRange = new TimeRangeModel
    //    {
    //        // Invalid date range (end date before start date)
    //        StartDate = DateTime.UtcNow,
    //        EndDate = DateTime.UtcNow.AddHours(-1)
    //    };

    //    // Act
    //    var result = await _controller.PostDeviceData(deviceId, dateRange) as BadRequestObjectResult;

    //    // Assert
    //    Assert.IsNotNull(result);
    //    Assert.AreEqual(400, result.StatusCode);
    //    Assert.AreEqual("Invalid date range parameters.", result.Value);
    //}

    //[TestMethod]
    //public async Task GetDataBetweenDatesFromBody_ValidDateRange_ReturnsOkObjectResult()
    //{
    //    // Arrange
    //    var dateRange = new TimeRangeModel
    //    {
    //        StartDate = DateTime.UtcNow.AddHours(-1),
    //        EndDate = DateTime.UtcNow
    //    };

    //    // Mock setup for GetDeviceDataByTimeRange
    //    _mockDeviceRepository.Setup(repo => repo.GetDeviceDataByTimeRange(It.IsAny<long>(), It.IsAny<long>()))
    //                         .ReturnsAsync(new List<DeviceDTO>());

    //    // Act
    //    var result = await _controller.GetDataBetweenDatesFromBody(dateRange) as OkObjectResult;

    //    // Assert
    //    Assert.IsNotNull(result);
    //    Assert.AreEqual(200, result.StatusCode);
    //    Assert.IsNotNull(result.Value); // Assuming it returns some data
    //}

    //[TestMethod]
    //public async Task GetDataBetweenDatesFromBody_InvalidDateRange_ReturnsInternalServerError()
    //{
    //    // Arrange
    //    var dateRange = new TimeRangeModel
    //    {
    //        // Invalid date range (end date before start date)
    //        StartDate = DateTime.UtcNow,
    //        EndDate = DateTime.UtcNow.AddHours(-1)
    //    };

    //    // Act
    //    var result = await _controller.GetDataBetweenDatesFromBody(dateRange) as StatusCodeResult;

        // Assert
    //    Assert.IsNotNull(result);
    //    Assert.AreEqual(500, result.StatusCode);
    //}
}
