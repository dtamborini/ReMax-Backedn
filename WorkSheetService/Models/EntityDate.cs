using WorkSheetService.Enums;

namespace WorkSheetService.Models
{
    public class EntityDate
    {
        public required DateType dateType { get; set; }
        public required EntityParticipation user { get; set; }
    }
}
