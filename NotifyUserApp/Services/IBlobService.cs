using System.Threading.Tasks;

namespace NotifyUserApp.Services
{
    public interface IBlobService
    {
        string CreateBlobSas(string blobName);
        string GetEmailFromMetadata(string blobName);
    }
}