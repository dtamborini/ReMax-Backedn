using WorkSheetService.Enums;

namespace WorkSheetService.Models
{
    public class EntityUniqueIdentifier
    {
        public required UniqueIdentifierType Type { get; set; } = UniqueIdentifierType.QR;
        public required string Value { get; set; }
    }
}
