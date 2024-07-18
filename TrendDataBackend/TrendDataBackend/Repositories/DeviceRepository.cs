using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendDataBackend.Models;
using TrendDataBackend.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly CloudTable _table;
    private readonly string rowKey = "1721200427_baa037fb-4dc3-4b5d-8eff-71569799cf82";
    public DeviceRepository(IOptions<AzureStorageOptions> options)
    {
        AzureStorageOptions storageOptions = options.Value;

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageOptions.ConnectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _table = tableClient.GetTableReference(storageOptions.TableName);
    }

    public async Task<List<Device>> GetAllDevicesAsync()
    {
        TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>();
        var devices = new List<Device>();

        TableContinuationToken token = null;
        do
        {
            TableQuerySegment<DeviceEntity> resultSegment = await _table.ExecuteQuerySegmentedAsync(query, token);
            token = resultSegment.ContinuationToken;

            foreach (var entity in resultSegment.Results)
            {
                devices.Add(new Device
                {
                    
                    DeviceId = entity.DeviceId,
                    TagId = entity.TagId,
                    DeviceTimeStamp = entity.DeviceTimeStamp,
                    Status = entity.Status,
                    DeviceProfileId = entity.DeviceProfileId,
                 
                    // Map other properties as needed
                });
            }
        } while (token != null);

        return devices;
    }


    public async Task<List<DeviceEntity>> GetDeviceByIdAsync(string deviceId)
    {
        string partitionKey = deviceId;

        TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>()
         .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

        List<DeviceEntity> devices = new List<DeviceEntity>();
        TableContinuationToken continuationToken = null;

       
        do
        {
            TableQuerySegment<DeviceEntity> querySegment = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
            devices.AddRange(querySegment.Results);

            continuationToken = querySegment.ContinuationToken;
        } while (continuationToken != null);

        return devices;
    }
    public async Task<List<DeviceEntity>> GetDataBetweenTimesAsync(long startTime, long endTime)
    {
        var query = new TableQuery<DeviceEntity>().Where(
        TableQuery.CombineFilters(
        TableQuery.GenerateFilterConditionForLong("DeviceTimeStamp", QueryComparisons.GreaterThanOrEqual, startTime),
        TableOperators.And,
        TableQuery.GenerateFilterConditionForLong("DeviceTimeStamp", QueryComparisons.LessThanOrEqual, endTime)));

        var result = new List<DeviceEntity>();
        TableContinuationToken token = null;

        do
        {
            var queryResult = await _table.ExecuteQuerySegmentedAsync(query, token);
            result.AddRange(queryResult.Results);
            token = queryResult.ContinuationToken;
        } while (token != null);

        return result;
    }


}

