using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendDataBackend.Controllers;
using TrendDataBackend.Models;
using TrendDataBackend.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DeviceDataTests
{
    [TestClass]
    public class DeviceControllerTests
    {
        private Mock<IDeviceRepository> _deviceRepositoryMock;
        private Mock<IConfiguration> _configurationMock;
        private DeviceController _controller;

        [TestInitialize]
        public void Setup()
        {
            _deviceRepositoryMock = new Mock<IDeviceRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new DeviceController(_deviceRepositoryMock.Object, _configurationMock.Object);
        }

        [TestMethod]
        public async Task PostDeviceData_ReturnsBadRequest_WhenDateRangeIsInvalid()
        {
            // Arrange
            string deviceId = "device123";
            TimeRangeModel invalidDateRange = null;

            // Act
            var result = await _controller.PostDeviceData(deviceId, invalidDateRange);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task PostDeviceData_ReturnsBadRequest_WhenDifferenceExceeds30Days()
        {
            // Arrange
            string deviceId = "device123";
            TimeRangeModel dateRange = new TimeRangeModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(31)
            };

            // Act
            var result = await _controller.PostDeviceData(deviceId, dateRange);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task PostDeviceData_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            string deviceId = "device123";
            TimeRangeModel dateRange = new TimeRangeModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            _deviceRepositoryMock
                .Setup(repo => repo.GetDeviceDataByTimeRangeAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new List<DeviceDTO>());

            // Act
            var result = await _controller.PostDeviceData(deviceId, dateRange);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(List<DeviceDTO>));
        }

        [TestMethod]
        public async Task GetDataBetweenDatesFromBody_ReturnsBadRequest_WhenDifferenceExceeds30Days()
        {
            // Arrange
            TimeRangeModel dateRange = new TimeRangeModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(31)
            };

            // Act
            var result = await _controller.GetDataBetweenDatesFromBody(dateRange);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetDataBetweenDatesFromBody_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            TimeRangeModel dateRange = new TimeRangeModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            _deviceRepositoryMock
                .Setup(repo => repo.GetDeviceDataByTimeRange(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new List<DeviceDTO>());

            // Act
            var result = await _controller.GetDataBetweenDatesFromBody(dateRange);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(List<DeviceDTO>));
        }
    }
}
