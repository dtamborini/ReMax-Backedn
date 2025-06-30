using AssetService.Enums;

namespace AssetService.Models
{
    public class EntityUniqueIdentifier
    {
        public required UniqueIdentifierType Type { get; set; } = UniqueIdentifierType.QR;
        public required string Value { get; set; }
    }
}
