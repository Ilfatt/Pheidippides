using System.Net;
using System.Text.Json;

namespace Pheidippides.ExternalServices;

public class YandexApiClient 
{
    public async Task ExecuteScenario(
        string scenarioName,
        string oAuthToken, 
        CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri("https://api.iot.yandex.net");
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {oAuthToken}");

        using var result = await httpClient.PostAsync(
            $"v1.0/scenarios/{scenarioName}/actions",
            null,
            cancellationToken);

        if (result.StatusCode != HttpStatusCode.OK)
            throw new HttpRequestException(JsonSerializer.Serialize(result));
    }
}