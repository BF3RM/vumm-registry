namespace VUModManagerRegistry.Models.Dtos
{
    public record RevokePermissionDto
    {
        public string Username { get; init; }
        public string Tag { get; init; }
    }
}