using TrendDataBackend.Models;

namespace TrendDataBackend.Repositories
{
    public interface IDeviceRepository
    {
        Task<List<Device>> GetAllDevicesAsync();
        Task<List<DeviceEntity>> GetDeviceByIdAsync( string deviceId);
        Task<List<DeviceEntity>> GetDataBetweenTimesAsync(long startTime, long endTime);
    }
}
