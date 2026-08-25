using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class MeetingsTests
{
    [Fact]
    public async Task Deserializes_all_three_circuit_type_values()
    {
        var (api, _) = MockHttpFactory.ForFixture("meetings", "Meetings.json");

        var data = await api.GetMeetingsAsync();

        Assert.Equal(3, data.Length);
        var singapore = data[0];
        Assert.Equal(61, singapore.CircuitKey);
        Assert.Equal("https://example.com/circuit/61.json", singapore.CircuitInfoUrl);
        Assert.Equal("Marina Bay", singapore.CircuitShortName);
        Assert.Equal(CircuitType.TemporaryStreet, singapore.CircuitType);
        Assert.Equal("SGP", singapore.CountryCode);
        Assert.Equal("https://example.com/flags/sgp.png", singapore.CountryFlag);
        Assert.Equal(157, singapore.CountryKey);
        Assert.Equal("Singapore", singapore.CountryName);
        Assert.Equal("08:00:00", singapore.GmtOffset);
        Assert.False(singapore.IsCancelled);
        Assert.Equal("Marina Bay", singapore.Location);
        Assert.Equal(1219, singapore.MeetingKey);
        Assert.Equal("Singapore Grand Prix", singapore.MeetingName);
        Assert.Equal("FORMULA 1 SINGAPORE AIRLINES SINGAPORE GRAND PRIX 2023", singapore.MeetingOfficialName);
        Assert.Equal(2023, singapore.Year);

        Assert.Equal(CircuitType.Permanent, data[1].CircuitType);
        Assert.Equal(CircuitType.TemporaryStreetRoad, data[2].CircuitType);
        Assert.True(data[2].IsCancelled);
    }

    [Fact]
    public async Task IncludeCircuitInfo_populates_CircuitInfo_per_meeting()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("meetings", "Meetings.json");
        var circuitInfoJson = File.ReadAllText(Path.Combine("Fixtures", "CircuitInfo.json"));
        mockHttp.When("https://example.com/circuit/*").Respond("application/json", circuitInfoJson);

        var data = await api.GetMeetingsAsync().IncludeCircuitInfo();

        Assert.Equal(3, data.Length);
        var monza = data[0].CircuitInfo!;
        Assert.Equal(39, monza.CircuitKey);
        Assert.Equal("Monza", monza.CircuitName);
        Assert.Equal("ITA", monza.CountryIocCode);
        Assert.Equal("1076", monza.MeetingKey);
        Assert.Null(monza.MeetingOfficialName);
        Assert.Equal(new DateTime(2021, 9, 12, 0, 0, 0, DateTimeKind.Utc), monza.RaceDate);
        Assert.Equal(95, monza.Rotation);
        Assert.Equal(14, monza.Round);
        Assert.Equal(2021, monza.Year);

        Assert.Equal(25.43m, monza.PitLoss.Normal);
        Assert.Equal(16.11m, monza.PitLoss.Sc);
        Assert.Equal(18.40m, monza.PitLoss.Vsc);

        Assert.Equal(2, monza.Corners.Length);
        Assert.Equal(1, monza.Corners[0].Number);
        Assert.Equal(153.78733177568182, monza.Corners[0].Angle);
        Assert.Equal(-569.5805053710938, monza.Corners[0].TrackPosition.X);
        Assert.Equal(8153.724609375, monza.Corners[0].TrackPosition.Y);

        Assert.Single(monza.MarshalLights);
        Assert.Single(monza.MarshalSectors);

        var candidateLap = monza.CandidateLap!;
        Assert.Equal("3", candidateLap.DriverNumber);
        Assert.Equal(2, candidateLap.LapNumber);
        Assert.Equal(new DateTime(2021, 9, 10, 12, 32, 32, 752, DateTimeKind.Utc), candidateLap.LapStartDate);
        Assert.Equal("FP1", candidateLap.Session);
        Assert.Equal(85.995, candidateLap.LapTime);

        Assert.Equal([33, 70, 83], monza.MiniSectorsIndexes);
        Assert.Equal([-1393, -1384, -1383], monza.X);
        Assert.Equal([-874, -794, -787], monza.Y);

        // Every meeting hits the same mocked circuit_info_url pattern in this fixture — assert all three got enriched.
        Assert.All(data, m => Assert.NotNull(m.CircuitInfo));
    }

    [Fact]
    public async Task Without_IncludeCircuitInfo_CircuitInfo_stays_null()
    {
        var (api, _) = MockHttpFactory.ForFixture("meetings", "Meetings.json");

        var data = await api.GetMeetingsAsync();

        Assert.All(data, m => Assert.Null(m.CircuitInfo));
    }
}
