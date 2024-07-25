// Inside DeviceRepositoryTests class

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using Moq;
using TrendDataBackend.Models;

[TestClass]
public class DeviceRepositoryTests
{
    private Mock<CloudTable> _mockTable;
    private DeviceRepository _deviceRepository;

    [TestInitialize]
    public void Initialize()
    {
        // Mock AzureStorageOptions
        var mockOptions = new Mock<IOptions<AzureStorageOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new AzureStorageOptions
        {
            ConnectionString = "fake-connection-string",
            TableName = "fake-table-name"
        });

        // Mock CloudTable
        _mockTable = new Mock<CloudTable>(new Uri("http://localhost"), null);
        _mockTable.Setup(t => t.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<DeviceEntity>>(), It.IsAny<TableContinuationToken>()))
                  .ReturnsAsync(new TableQuerySegment<DeviceEntity>(new List<DeviceEntity>(), null));

        // Mock CloudTableClient and setup CloudTable
        var mockTableClient = new Mock<CloudTableClient>(new Uri("http://localhost"), null);
        mockTableClient.Setup(c => c.GetTableReference("fake-table-name")).Returns(_mockTable.Object);

        // Create DeviceRepository instance
        _deviceRepository = new DeviceRepository(mockOptions.Object);
    }

   
    [TestMethod]
    public async Task GetAllDevicesAsync_EmptyTable_ReturnsEmptyList()
    {
        // Arrange
        _mockTable.Setup(t => t.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<DeviceEntity>>(), null))
                  .ReturnsAsync(new TableQuerySegment<DeviceEntity>(new List<DeviceEntity>(), null));

        // Act
        var result = await _deviceRepository.GetAllDevicesAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }


    [TestMethod]
    public async Task GetDeviceByIdAsync_ReturnsEmptyList()
    {
        // Arrange
        string deviceId = "test-device-id";
        var expectedDevices = new List<DeviceEntity>();
        _mockTable.Setup(t => t.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<DeviceEntity>>(), null))
                  .ReturnsAsync(new TableQuerySegment<DeviceEntity>(new List<DeviceEntity>(), null));

        // Act
        var result = await _deviceRepository.GetDeviceByIdAsync(deviceId);

        // Assert
        CollectionAssert.AreEqual(expectedDevices, result);
    }

    [TestMethod]
    public async Task GetDataBetweenTimesAsync_ReturnsEmptyList()
    {
        // Arrange
        long startTime = 0;
        long endTime = 100;
        var expectedData = new List<DeviceEntity>();
        _mockTable.Setup(t => t.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<DeviceEntity>>(), null))
                  .ReturnsAsync(new TableQuerySegment<DeviceEntity>(new List<>));

        // Act
        var result = await _deviceRepository.GetDataBetweenTimesAsync(startTime, endTime);

        // Assert
        CollectionAssert.AreEqual(expectedData, result);
    }
}
