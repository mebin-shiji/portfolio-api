using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace portfolio_api.Infrastructure.Services.Storage;

public class AzureStorageService(ILogger<AzureStorageService> logger, IOptions<StorageOptions> options) : IStorageService
{
    private readonly ILogger<AzureStorageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly StorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    public string GenerateContainerSasToken(string containerName, TimeSpan expiry)
    {
        try
        {
            var credential = new StorageSharedKeyCredential(_options.AccountName, _options.AccountKey);
            var sasBuilder = new BlobSasBuilder(BlobContainerSasPermissions.Write | BlobContainerSasPermissions.Create | BlobContainerSasPermissions.Add, DateTimeOffset.UtcNow.Add(expiry))
            {
                BlobContainerName = containerName,
                Resource = "c", // 'c' for container
                StartsOn = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(5))
            };

            var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();
            return sasToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating container SAS token for container: {ContainerName}", containerName);
            throw;
        }
    }

    public async Task<byte[]> DownloadBlobAsBytesAsync(string blobUri, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(blobUri, UriKind.Absolute, out var uri))
        {
            _logger.LogError("Provided URI is not valid.");
            throw new ArgumentException("Provided blob URI is not valid.");
        }
        var blobClient = new BlobClient(uri);

        try
        {
            using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure Storage error occurred when downloading blob. ErrorCode: {ErrorCode}", ex.ErrorCode);
            throw;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "I/O error occurred when downloading blob.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred when downloading blob.");
            throw;
        }
    }
}
