namespace portfolio_api.Infrastructure.Services.Storage;

public interface IStorageService
{
    string GenerateContainerSasToken(string containerName, TimeSpan expiry);
    Task<byte[]> DownloadBlobAsBytesAsync(string blobUri, CancellationToken cancellationToken);
}
