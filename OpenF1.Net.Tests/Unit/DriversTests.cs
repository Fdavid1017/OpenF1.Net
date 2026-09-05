using System.Net;
using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class DriversTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("drivers", "Drivers.json");

        var data = await api.GetDriversAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal("M VERSTAPPEN", first.BroadcastName);
#pragma warning disable CS0618 // deprecated field, still asserted while it's live
        Assert.Equal("NED", first.CountryCode);
#pragma warning restore CS0618
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal("Max", first.FirstName);
        Assert.Equal("Max VERSTAPPEN", first.FullName);
        Assert.Equal("https://example.com/verstappen.png", first.HeadshotUrl);
        Assert.Equal("Verstappen", first.LastName);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal("VER", first.NameAcronym);
        Assert.Equal(9161, first.SessionKey);
        Assert.Equal("3671C6", first.TeamColour);
        Assert.Equal("Red Bull Racing", first.TeamName);
    }

    [Fact]
    public async Task Without_ResolveImages_HeadshotUrl_is_unchanged_and_fullbody_stays_null()
    {
        var (api, _) = MockHttpFactory.ForFixture("drivers", "Drivers.json");

        var data = await api.GetDriversAsync();

        Assert.All(
            data,
            d =>
            {
                Assert.StartsWith("https://example.com/", d.HeadshotUrl);
                Assert.Null(d.FullBodyUrlLeft);
                Assert.Null(d.FullBodyUrlRight);
            }
        );
    }

    [Fact]
    public async Task ResolveImages_prefers_the_official_asset_over_the_multiviewer_fallback()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("drivers", "Drivers.json");
        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);

        // Max Verstappen's derived reference (first 3 of first name + first 3 of last name + "01") only
        // exists as F1's own higher-resolution asset, for one year buried in the middle of the search range.
        mockHttp
            .When("https://media.formula1.com/content/dam/fom-website/2018-redesign-assets/drivers/2023/MAXVER01.png")
            .Respond(HttpStatusCode.OK);

        var data = await api.GetDriversAsync().ResolveImages();

        var verstappen = Assert.Single(data, d => d.NameAcronym == "VER");
        Assert.Equal(
            "https://media.formula1.com/content/dam/fom-website/2018-redesign-assets/drivers/2023/MAXVER01.png",
            verstappen.HeadshotUrl
        );
    }

    [Fact]
    public async Task ResolveImages_falls_back_to_multiviewer_when_the_official_asset_is_missing_for_every_year()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("drivers", "Drivers.json");
        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);

        mockHttp.When("https://assets.multiviewer.dev/driver-headshots/2020/HAM.png").Respond(HttpStatusCode.OK);

        var data = await api.GetDriversAsync().ResolveImages();

        var hamilton = Assert.Single(data, d => d.NameAcronym == "HAM");
        Assert.Equal("https://assets.multiviewer.dev/driver-headshots/2020/HAM.png", hamilton.HeadshotUrl);
    }

    [Fact]
    public async Task ResolveImages_populates_only_the_full_body_sides_that_exist()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("drivers", "Drivers.json");
        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);

        // "Red Bull Racing" -> "redbullracing" team slug; only the left-facing render exists for 2025.
        mockHttp
            .When(
                "https://media.formula1.com/image/upload/c_fill,w_720/q_auto/v1/common/f1/2025/redbullracing/maxver01/2025redbullracingmaxver01left.webp"
            )
            .Respond(HttpStatusCode.OK);

        var data = await api.GetDriversAsync().ResolveImages();

        var verstappen = Assert.Single(data, d => d.NameAcronym == "VER");
        Assert.Equal(
            "https://media.formula1.com/image/upload/c_fill,w_720/q_auto/v1/common/f1/2025/redbullracing/maxver01/2025redbullracingmaxver01left.webp",
            verstappen.FullBodyUrlLeft
        );
        Assert.Null(verstappen.FullBodyUrlRight);
    }
}
