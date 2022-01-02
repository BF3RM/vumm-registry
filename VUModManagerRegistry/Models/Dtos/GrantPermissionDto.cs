namespace VUModManagerRegistry.Models.Dtos
{
    public record GrantPermissionDto
    {
        public string Username { get; init; }
        public ModPermission Permission { get; init; }
        public string Tag { get; init; }
    }
}