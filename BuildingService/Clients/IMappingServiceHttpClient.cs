namespace BuildingService.Clients
{
    public interface IMappingServiceHttpClient
    {
        Task<Guid?> GetMappingGuidByIdAsync(Guid mappingId);
    }
}