using System.IO;
using System.Threading.Tasks;

namespace VUModManagerRegistry.Common.Interfaces
{
    public interface IModStorage
    {
        Task StoreArchive(string modName, string modVersion, Stream stream);
        Task DeleteArchive(string modName, string modVersion);
        string GetArchiveDownloadLink(string modName, string modVersion);
    }
}