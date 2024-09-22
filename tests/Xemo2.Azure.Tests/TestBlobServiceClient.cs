using Azure.Storage;
using Azure.Storage.Blobs;
using Tonga;
using Xemo.Azure.Tests;

namespace Xemo2.AzureTests;

/// <summary>
/// Creates a blob container and deletes it on disposal.
/// </summary>
public sealed class TestBlobServiceClient : IScalar<BlobServiceClient>
{
    private readonly Lazy<BlobServiceClient> service;
    
    /// <summary>
    /// Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobServiceClient()
    {
        this.service = 
            new Lazy<BlobServiceClient>(() =>
                new BlobServiceClient(
                    new Uri(new Secret("blobStorageUri").AsString()),
                    new StorageSharedKeyCredential(
                        new Secret("storageAccountName").AsString(),
                        new Secret("storageAccountSecret").AsString()
                    )
                )
        );
    }

    public BlobServiceClient Value() => this.service.Value;
}