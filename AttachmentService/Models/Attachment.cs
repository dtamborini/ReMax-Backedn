using System.Text.Json.Serialization;

namespace AttachmentService.Models
{
    public class Attachment : EntityBaseDomain
    {
        [JsonIgnore]
        public Guid BuildingGuid { get; set; }
        [JsonIgnore]
        public Guid? WorkOrderGuid { get; set; }
        [JsonIgnore]
        public Guid? WorkSheetGuid { get; set; }
        [JsonIgnore]
        public Guid? QuoteGuid { get; set; }
        [JsonIgnore]
        public Guid? RfqGuid { get; set; }
        [JsonIgnore]
        public Guid? TicketGuid { get; set; }
        [JsonIgnore]
        public Guid? ActivityGuid { get; set; }
        [JsonIgnore]
        public Guid? AccidentGuid { get; set; }
        [JsonIgnore]
        public Guid? NotificationGuid { get; set; }
    }

}