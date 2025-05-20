using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace portfolio_api.Infrastructure.Storage;

public interface IAzureStorageService
{
    string GenerateBlobSasToken(string containerName, string blobName, TimeSpan expiry);
    Task<byte[]> DownloadBlobAsBytesAsync(string blobUri);
}

public class AzureStorageService(ILogger<AzureStorageService> logger, IOptions<AzureStorageOptions> options) : IAzureStorageService
{
    private readonly ILogger<AzureStorageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly AzureStorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    public string GenerateBlobSasToken(string containerName, string blobName, TimeSpan expiry)
    {
        try
        {
            var credential = new StorageSharedKeyCredential(_options.AccountName, _options.AccountKey);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(15)),
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

            var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();
            return sasToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating SAS token for blob: {BlobName}", blobName);
            throw;
        }
    }

    public async Task<byte[]> DownloadBlobAsBytesAsync(string blobUri)
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
            await blobClient.DownloadToAsync(memoryStream);
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
