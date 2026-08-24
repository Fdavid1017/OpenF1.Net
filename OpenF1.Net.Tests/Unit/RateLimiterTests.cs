using System.Diagnostics;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class RateLimiterTests
{
    [Fact]
    public async Task A_fourth_request_within_one_second_waits_for_the_sliding_window_to_free_up()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openf1.org/v1/*").Respond("application/json", "[]");
        var api = new OpenF1(mockHttp.ToHttpClient()); // UseRateLimit defaults to true

        var stopwatch = Stopwatch.StartNew();
        await api.GetDriversAsync();
        await api.GetDriversAsync();
        await api.GetDriversAsync();
        await api.GetDriversAsync(); // 4th request in the same rolling window must wait
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds >= 900,
            $"Expected the 4th request to wait for the sliding window, but only {stopwatch.ElapsedMilliseconds}ms elapsed.");
    }

    [Fact]
    public async Task Disabling_rate_limiting_lets_requests_through_immediately()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openf1.org/v1/*").Respond("application/json", "[]");
        var api = new OpenF1(mockHttp.ToHttpClient(), new OpenF1Config { UseRateLimit = false });

        var stopwatch = Stopwatch.StartNew();
        await api.GetDriversAsync();
        await api.GetDriversAsync();
        await api.GetDriversAsync();
        await api.GetDriversAsync();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 900,
            $"Expected all 4 requests through immediately with rate limiting disabled, but {stopwatch.ElapsedMilliseconds}ms elapsed.");
    }
}
