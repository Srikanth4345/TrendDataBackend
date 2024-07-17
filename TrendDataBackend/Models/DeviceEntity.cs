using Microsoft.Azure.Cosmos.Table;

public class DeviceEntity : TableEntity
{
    
    public string DeviceId { get; set; }
    public long TagId { get; set; }
    public string DeviceProfileId { get; set; }
    public long DeviceTimeStamp { get; set; }

    public bool Status { get; set; }

    public DeviceEntity()
    {
        // Required for Azure Table Storage
    }

    public DeviceEntity(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }
}
