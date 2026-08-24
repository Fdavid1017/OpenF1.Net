using System.Reflection;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class HttpClientOwnershipTests
{
    static HttpClient GetInternalHttpClient(OpenF1 api) =>
        (HttpClient)typeof(OpenF1).GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(api)!;

    [Fact]
    public async Task Disposing_an_owned_HttpClient_disposes_it()
    {
        var api = new OpenF1();
        var httpClient = GetInternalHttpClient(api);

        await api.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => httpClient.GetAsync("https://api.openf1.org/v1/drivers"));
    }

    [Fact]
    public async Task Disposing_does_not_dispose_an_externally_provided_HttpClient()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openf1.org/v1/*").Respond("application/json", "[]");
        var externalClient = mockHttp.ToHttpClient();
        var api = new OpenF1(externalClient);

        await api.DisposeAsync();

        // Should not throw ObjectDisposedException — the caller still owns this client.
        var response = await externalClient.GetAsync("https://api.openf1.org/v1/drivers");
        Assert.True(response.IsSuccessStatusCode);
    }
}
