namespace AttachmentService.Models
{
    public class AttachmentTree
    {
        public Guid BuildingId { get; set; }
        public List<AttachmentTreeNode> Nodes { get; set; } = new List<AttachmentTreeNode>();
    }

    public class AttachmentTreeNode
    {
        public Guid AttachmentGuid { get; set; }
        public string Name { get; set; }
        public List<AttachmentTreeNode> Children { get; set; } = new List<AttachmentTreeNode>();
    }
}
