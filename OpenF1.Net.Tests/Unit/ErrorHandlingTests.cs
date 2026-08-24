using System.Net;
using OpenF1.Net.Exceptions;
using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class ErrorHandlingTests
{
    [Fact]
    public async Task NotFound_with_no_results_detail_returns_empty_array()
    {
        var api = MockHttpFactory.ForResponse(HttpStatusCode.NotFound, """{"detail":"No results found."}""");

        var data = await api.GetDriversAsync();

        Assert.Empty(data);
    }

    [Fact]
    public async Task NotFound_with_a_different_detail_throws_generic_api_exception()
    {
        var api = MockHttpFactory.ForResponse(HttpStatusCode.NotFound, """{"detail":"Endpoint not found."}""");

        var ex = await Assert.ThrowsAsync<OpenF1ApiException>(async () => await api.GetDriversAsync());

        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal("Endpoint not found.", ex.Detail);
    }

    [Fact]
    public async Task TooManyRequests_throws_rate_limit_exceeded()
    {
        var api = MockHttpFactory.ForResponse(HttpStatusCode.TooManyRequests, """{"detail":"max 3 requests per second"}""");

        var ex = await Assert.ThrowsAsync<OpenF1RateLimitExceededException>(async () => await api.GetDriversAsync());

        Assert.Equal(HttpStatusCode.TooManyRequests, ex.StatusCode);
        Assert.Equal("max 3 requests per second", ex.Detail);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task Unauthorized_or_Forbidden_throws_subscription_required(HttpStatusCode statusCode)
    {
        var api = MockHttpFactory.ForResponse(statusCode, """{"detail":"subscription required"}""");

        var ex = await Assert.ThrowsAsync<OpenF1SubscriptionRequiredException>(async () => await api.GetDriversAsync());

        Assert.Equal(statusCode, ex.StatusCode);
        Assert.Equal("subscription required", ex.Detail);
    }

    [Fact]
    public async Task Non_json_error_body_falls_back_to_the_raw_body_as_detail()
    {
        var api = MockHttpFactory.ForResponse(HttpStatusCode.InternalServerError, "not json");

        var ex = await Assert.ThrowsAsync<OpenF1ApiException>(async () => await api.GetDriversAsync());

        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        Assert.Equal("not json", ex.Detail);
    }

    [Fact]
    public async Task Unprocessable_entity_throws_generic_api_exception()
    {
        var api = MockHttpFactory.ForResponse(HttpStatusCode.UnprocessableEntity, """{"detail":"too much data at once"}""");

        var ex = await Assert.ThrowsAsync<OpenF1ApiException>(async () => await api.GetDriversAsync());

        Assert.Equal(HttpStatusCode.UnprocessableEntity, ex.StatusCode);
        Assert.Equal("too much data at once", ex.Detail);
    }
}
