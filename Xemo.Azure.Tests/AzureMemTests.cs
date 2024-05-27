using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Xunit;

namespace Xemo.Azure.Tests;

public sealed class AzureMemTests
{
    [Fact]
    public void Connects()
    {
        var serviceClient =
            new TableServiceClient(
                new Uri(new Secret("storageUri").AsString()),
                new TableSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            );

        Pageable<TableItem> queryTableResults = serviceClient.Query();

        // Iterate the <see cref="Pageable"> in order to access queried tables.
        foreach (TableItem table in queryTableResults)
        {
            Console.WriteLine(table.Name);
        }
    }
}