using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Common.Interfaces
{
    public interface IModStorage
    {
        string GetUploadLink(ModVersion version);
        string GetDownloadLink(ModVersion version);
    }
}