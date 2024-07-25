using TrendDataBackend.Models;

namespace TrendDataBackend.Repositories
{
    public interface IDeviceRepository
    {

      
        Task<List<DeviceDTO>> GetDeviceDataByTimeRangeAsync(string deviceId, long startTime, long endTime);
        Task<List<DeviceDTO>> GetDeviceDataByTimeRange(long startTime, long endTime);
       // Task<List<DeviceEntity>> GetDataBetweenTimesAsync(long startTime, long endTime);

    }
}
