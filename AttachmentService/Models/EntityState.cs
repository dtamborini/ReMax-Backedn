using AttachmentService.Enums;

namespace AttachmentService.Models
{
    public class EntityState
    {
        public required DateType Value { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
    }
}
