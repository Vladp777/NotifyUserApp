namespace NotifyUserApp.Services
{
    public interface ISasService
    {
        string CreateBlobSas(string blobName);
    }
}