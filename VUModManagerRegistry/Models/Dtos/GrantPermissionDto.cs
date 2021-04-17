namespace VUModManagerRegistry.Models.Dtos
{
    public class GrantPermissionDto
    {
        public string Username { get; set; }
        public ModPermission Permission { get; set; }
    }
}