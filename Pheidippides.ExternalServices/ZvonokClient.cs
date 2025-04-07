using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Pheidippides.ExternalServices;

public class ZvonokClient
{
    private const string FLASH_CALL_CAMPAIGN_ID = "761802590";
    private static readonly string Prefix = "db654378fe019622" + string.Empty + "f293034f4078be43";

    public async Task FlashCall(string phoneNumber, CancellationToken cancellationToken)
    {
        using var httpClient = BuildClient();

        var query = new Dictionary<string, string?>
        {
            ["public_key"] = string.Empty + Prefix + string.Empty,
            ["phone"] = phoneNumber,
            ["campaign_id"] = FLASH_CALL_CAMPAIGN_ID
        };

        var uri = QueryHelpers.AddQueryString("/manager/cabapi_external/api/v1/phones/flashcall/", query);

        using var response = await httpClient.PostAsync(uri, null, cancellationToken);
    }

    public async Task<ushort> GetFlashCallCode(string phoneNumber, CancellationToken cancellationToken)
    {
        using var httpClient = BuildClient();
        
        var query = new Dictionary<string, string?>
        {
            ["public_key"] = string.Empty + Prefix + string.Empty,
            ["phone"] = phoneNumber,
            ["campaign_id"] = FLASH_CALL_CAMPAIGN_ID,
            ["expand"] = "0",
            ["from_created_date"] = DateTimeOffset.UtcNow
                .AddMinutes(-15)
                .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        };

        var uri = QueryHelpers.AddQueryString("/manager/cabapi_external/api/v1/phones/calls_by_phone/", query);

        using var result = await httpClient.GetAsync(uri, cancellationToken);
        var content = await result.Content.ReadAsStringAsync(cancellationToken);
        return 0;
    }

    private static HttpClient BuildClient()
    {
        var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri("https://zvonok.com");

        return httpClient;
    }
}