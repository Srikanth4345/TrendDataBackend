using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendDataBackend.Models;
using TrendDataBackend.Repositories;

namespace TrendDataBackend.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly CloudTable _table;
        private readonly IMapper _mapper;

        public DeviceRepository(IOptions<AzureStorageOptions> options, IMapper mapper)
        {
            AzureStorageOptions storageOptions = options.Value;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageOptions.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _table = tableClient.GetTableReference(storageOptions.TableName);

            _mapper = mapper;
        }



        public async Task<List<DeviceDTO>> GetDeviceDataByTimeRangeAsync(string deviceId, long startTime, long endTime)
        {
            string partitionFilter = TableQuery.GenerateFilterCondition("DeviceId", QueryComparisons.Equal, deviceId);
            string startFilter = TableQuery.GenerateFilterConditionForLong("DeviceTimeStamp", QueryComparisons.GreaterThanOrEqual, startTime);
            string endFilter = TableQuery.GenerateFilterConditionForLong("DeviceTimeStamp", QueryComparisons.LessThanOrEqual, endTime);

            string combinedFilter = TableQuery.CombineFilters(
                partitionFilter,
                TableOperators.And,
                TableQuery.CombineFilters(startFilter, TableOperators.And, endFilter)
            );

            var query = new TableQuery<DeviceEntity>().Where(combinedFilter);

            var results = new List<DeviceDTO>();
            TableContinuationToken token = null;

            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));

            do
            {
                var segment = await _table.ExecuteQuerySegmentedAsync(query, token);
                token = segment.ContinuationToken;


                var devices = segment.Results.Select(entity =>
                {
                    var deviceDTO = mapper.Map<DeviceDTO>(entity);
                    deviceDTO.DeviceTimeStamp = DateTimeOffset.FromUnixTimeSeconds(entity.DeviceTimeStamp).UtcDateTime;
                    return deviceDTO;
                });

                results.AddRange(devices);

            }
            while (token != null);

            return results;
        }

        public async Task<List<DeviceDTO>> GetDeviceDataByTimeRange(long startDate, long endDate)
        {
            var query = new TableQuery<DeviceEntity>().Where(
                   TableQuery.CombineFilters(
                   TableQuery.GenerateFilterConditionForLong("DeviceTimeStamp", QueryComparisons.GreaterThanOrEqual, startDate),
                   TableOperators.And,
                   TableQuery.GenerateFilterConditionForLong("DeviceTimeStamp", QueryComparisons.LessThanOrEqual, endDate)));

            var result = new List<DeviceDTO>();
            TableContinuationToken token = null;
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));

            do
            {
                var segment = await _table.ExecuteQuerySegmentedAsync(query, token);
                token = segment.ContinuationToken;
                var devices = segment.Results.Select(entity =>
                {
                    var deviceDTO = mapper.Map<DeviceDTO>(entity);
                    deviceDTO.DeviceTimeStamp = DateTimeOffset.FromUnixTimeSeconds(entity.DeviceTimeStamp).UtcDateTime;
                    return deviceDTO;
                });
                result.AddRange(devices);

            } while (token != null);

            return result;
        }

    }
}

