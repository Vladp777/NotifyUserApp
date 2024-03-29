using System;
using System.IO;
using System.Linq;
using NotifyUserApp.Models;
using NotifyUserApp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;

namespace NotifyUserApp
{
    [StorageAccount("AzureBlobStorage")]
    public class NotifyUserFunction
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotifyUserFunction> _log;
        private readonly IBlobService _blobService;
        public NotifyUserFunction(IBlobService sasService, 
            IEmailService emailService,
            ILogger<NotifyUserFunction> log)
        {
            _blobService = sasService;
            _emailService = emailService;
            _log = log;
        }

        [FunctionName("NotifyUserFunction")]
        public void Run([BlobTrigger("fileuploader/{name}")]Stream myBlob, string name)
        {
            _log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var email = _blobService.GetEmailFromMetadata(name);
            if (email == null)
            {
                _log.LogError("Email is invalid");
                return;
            }

            var sasUri = _blobService.CreateBlobSas(name);

            var fileName = name.Split(';').Last();

            var message = GetMassage(fileName, myBlob.Length /1000.0, sasUri);

            var emailMessage = new EmailModel
            {
                EmailTo = email,
                Subject = "Notification of Successful File Upload",
                Message = message
            };

            _emailService.SendEmail(emailMessage);
        }

        private string GetMassage(string fileName, double size, string uri)
        {
            return "We are pleased to inform you that your file has been successfully uploaded to our server.<br>" +
                    $"Details of the uploaded file:<br>File Name: {fileName}<br>File Size: {size} KB<br>Date and Time of Upload: {DateTime.Now}<br>" +
                    $"Link for file: {uri}" +
                    $"<br/>Please note that the link will expire at 1 hour.";
        }
    }
}
