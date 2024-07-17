using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;

namespace TrendDataBackend.Models
{
    public class Device
    {
        public string DeviceId { get; set; }
        public long TagId { get; set; }

        public string DeviceProfileId { get; set; }
        public long DeviceTimeStamp { get; set; }

        public bool Status { get; set; }
       
    }
}
