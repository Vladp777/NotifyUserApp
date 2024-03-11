using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using NotifyUserApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace NotifyUserApp.Services;

public class SasService : ISasService
{
    private readonly IOptions<BlobConfig> _blobConfig;
    private readonly ILogger<SasService> _log;

    public SasService(IOptions<BlobConfig> blobConfig,
        ILogger<SasService> log)
    {
        _blobConfig = blobConfig;
        _log = log;
    }

    public string CreateBlobSas(string blobName)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConfig.Value.ConnectionString);

        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_blobConfig.Value.ContainerName);

        BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

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
