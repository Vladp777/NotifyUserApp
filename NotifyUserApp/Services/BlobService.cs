using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using NotifyUserApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace NotifyUserApp.Services;

public class BlobService : IBlobService
{
    private readonly IOptions<BlobConfig> _blobConfig;
    private readonly ILogger<BlobService> _log;

    public BlobService(IOptions<BlobConfig> blobConfig,
        ILogger<BlobService> log)
    {
        _blobConfig = blobConfig;
        _log = log;
    }
    private BlobClient GetBlobClient(string blobName)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConfig.Value.ConnectionString);

        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_blobConfig.Value.ContainerName);

        return blobContainerClient.GetBlobClient(blobName);
    }
    public string GetEmailFromMetadata(string blobName)
    {
        var blobClient = GetBlobClient(blobName);

        var properties = blobClient.GetProperties();
        var metadataDic = properties.Value.Metadata;
        return metadataDic["Email"];
    }

    public string CreateBlobSas(string blobName)
    {
        
        var blobClient = GetBlobClient(blobName);

        BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _blobConfig.Value.ContainerName,
            BlobName = blobName,
            ExpiresOn = DateTime.UtcNow.AddHours(1)
        };
        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = blobSasBuilder.ToSasQueryParameters(
            new StorageSharedKeyCredential(_blobConfig.Value.StorageAccountName, _blobConfig.Value.AccountKey)).ToString();

        var blobSasUri = $"{blobClient.Uri}?{sasToken}";

        _log.LogInformation("SAS created");

        return blobSasUri;
    }
}
