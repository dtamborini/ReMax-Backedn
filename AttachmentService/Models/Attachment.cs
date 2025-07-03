using System.Text.Json.Serialization;

namespace AttachmentService.Models
{
    public class Attachment : EntityBaseDomain
    {
        [JsonIgnore]
        public Guid? AssetGuid { get; init; }
        [JsonIgnore]
        public Guid? UserGuid { get; init; }
        [JsonIgnore]
        public Guid BuildingGuid { get; init; }
        [JsonIgnore]
        public Guid? WorkOrderGuid { get; init; }
        [JsonIgnore]
        public Guid? WorkSheetGuid { get; init; }
        [JsonIgnore]
        public Guid? QuoteGuid { get; init; }
        [JsonIgnore]
        public Guid? RfqGuid { get; init; }
        [JsonIgnore]
        public Guid? TicketGuid { get; init; }
        [JsonIgnore]
        public Guid? ActivityGuid { get; init; }
        [JsonIgnore]
        public Guid? AccidentGuid { get; init; }
        [JsonIgnore]
        public Guid? NotificationGuid { get; init; }
    }

}