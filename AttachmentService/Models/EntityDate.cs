using AttachmentService.Enums;

namespace AttachmentService.Models
{
    public class EntityDate
    {
        public required DateType DateType { get; init; }
        public required EntityParticipation User { get; set; }
    }
}
