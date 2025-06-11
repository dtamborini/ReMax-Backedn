namespace UserService.Clients
{
    public interface IMappingServiceHttpClient
    {
        Task<Guid?> GetMappingGuidByIdAsync(Guid mappingId);
    }
}