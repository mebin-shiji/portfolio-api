namespace portfolio_api.Features.Upload.CreateSasToken
{
    public sealed record CreateSasTokenCommand
    (
        string BlobName,
        string? ContainerName
    );
}
