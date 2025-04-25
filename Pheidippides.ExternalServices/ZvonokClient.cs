using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Pheidippides.ExternalServices;

public class ZvonokClient
{
    private const string FLASH_CALL_CAMPAIGN_ID = "761802590";
    private const string ALERT_CAMPAIGN_ID = "466044763";
    private static readonly string Prefix = "db654378fe019622" + string.Empty + "f293034f4078be43";

    public async Task<ushort> FlashCall(string phoneNumber, CancellationToken cancellationToken)
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

        var content = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken);

        return Convert.ToUInt16(content!.Data.Pincode, CultureInfo.InvariantCulture);
    }
    
    public async Task AlertCall(string phoneNumber, string text, CancellationToken cancellationToken)
    {
        using var httpClient = BuildClient();

        var query = new Dictionary<string, string?>
        {
            ["public_key"] = string.Empty + Prefix + string.Empty,
            ["phone"] = phoneNumber,
            ["campaign_id"] = ALERT_CAMPAIGN_ID,
            ["text"] = text,
            ["max_call_time"] = "15",
        };

        var uri = QueryHelpers.AddQueryString("/manager/cabapi_external/api/v1/phones/call/", query);

        using var responseMessage = await httpClient.PostAsync(uri, null, cancellationToken);
    }

    private static HttpClient BuildClient()
    {
        var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri("https://zvonok.com");

        return httpClient;
    }
}
file class ApiResponse
{
    public string Status { get; init; }
    public ResponseData Data { get; init; }
}

file class ResponseData
{
    public string Balance { get; init; }
    public long CallId { get; init; }
    public DateTime Created { get; init; }
    public string Phone { get; init; }
    public string Pincode { get; init; }
}