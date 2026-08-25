using System.Net;
using OpenF1.Net;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.TestHelpers;

/// <summary>Wires an <see cref="OpenF1Client"/> instance to a MockHttp handler serving a fixture file, rate limiting off.</summary>
static class MockHttpFactory
{
    public static (OpenF1Client Api, MockHttpMessageHandler MockHttp) ForFixture(string endpoint, string fixtureFileName)
    {
        var mockHttp = new MockHttpMessageHandler();
        var json = File.ReadAllText(Path.Combine("Fixtures", fixtureFileName));
        mockHttp.When($"https://api.openf1.org/v1/{endpoint}*").Respond("application/json", json);
        return (new OpenF1Client(mockHttp.ToHttpClient(), new OpenF1Config { UseRateLimit = false }), mockHttp);
    }

    public static OpenF1Client ForResponse(HttpStatusCode statusCode, string body)
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openf1.org/v1/*").Respond(statusCode, "application/json", body);
        return new OpenF1Client(mockHttp.ToHttpClient(), new OpenF1Config { UseRateLimit = false });
    }
}
