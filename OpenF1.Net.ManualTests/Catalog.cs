using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net.ManualTests;

/// <summary>
/// Every client entry point the harness can exercise — one entry per OpenF1 endpoint, plus the two
/// <c>latest</c> shortcuts. Each entry declares the opt-ins that endpoint's query actually offers, so the
/// options menu never shows a switch the endpoint doesn't have.
/// </summary>
public static class Catalog
{
    public static IReadOnlyList<IEndpointTest> Build() =>
    [
        new EndpointTest<CarDataFilterFields, CarDataPoint>
        {
            Endpoint = "car_data",
            Method = "GetCarDataAsync()",
            CreateQuery = (client, ct) => client.GetCarDataAsync(ct),
            Options = DriverDetailOptions<CarDataFilterFields, CarDataPoint>(),
        },
        new EndpointTest<ChampionshipDriversFilterFields, ChampionshipDriver>
        {
            Endpoint = "championship_drivers",
            Method = "GetChampionshipDriversAsync()",
            CreateQuery = (client, ct) => client.GetChampionshipDriversAsync(ct),
            Options = DriverDetailOptions<ChampionshipDriversFilterFields, ChampionshipDriver>(),
        },
        new EndpointTest<ChampionshipTeamsFilterFields, ChampionshipTeam>
        {
            Endpoint = "championship_teams",
            Method = "GetChampionshipTeamsAsync()",
            CreateQuery = (client, ct) => client.GetChampionshipTeamsAsync(ct),
            // Car render URLs are always resolved for this endpoint — nothing to switch on.
        },
        new EndpointTest<DriversFilterFields, Driver>
        {
            Endpoint = "drivers",
            Method = "GetDriversAsync()",
            CreateQuery = (client, ct) => client.GetDriversAsync(ct),
            Options =
            [
                new QueryOption<DriversFilterFields, Driver>(
                    ".ResolveImages() - headshot + full-body render URLs",
                    query => ((DriversQuery)query).ResolveImages()),
            ],
        },
        new EndpointTest<IntervalsFilterFields, Interval>
        {
            Endpoint = "intervals",
            Method = "GetIntervalsAsync()",
            CreateQuery = (client, ct) => client.GetIntervalsAsync(ct),
            Options = DriverDetailOptions<IntervalsFilterFields, Interval>(),
        },
        new EndpointTest<LapsFilterFields, Lap>
        {
            Endpoint = "laps",
            Method = "GetLapsAsync()",
            CreateQuery = (client, ct) => client.GetLapsAsync(ct),
            Options = DriverDetailOptions<LapsFilterFields, Lap>(),
        },
        new EndpointTest<LocationFilterFields, LocationPoint>
        {
            Endpoint = "location",
            Method = "GetLocationAsync()",
            CreateQuery = (client, ct) => client.GetLocationAsync(ct),
            Options = DriverDetailOptions<LocationFilterFields, LocationPoint>(),
        },
        new EndpointTest<MeetingsFilterFields, Meeting>
        {
            Endpoint = "meetings",
            Method = "GetMeetingsAsync()",
            CreateQuery = (client, ct) => client.GetMeetingsAsync(ct),
            Options =
            [
                new QueryOption<MeetingsFilterFields, Meeting>(
                    ".IncludeCircuitInfo() - one extra MultiViewer request per meeting",
                    query => ((MeetingsQuery)query).IncludeCircuitInfo()),
            ],
        },
        new EndpointTest<OvertakesFilterFields, Overtake>
        {
            Endpoint = "overtakes",
            Method = "GetOvertakesAsync()",
            CreateQuery = (client, ct) => client.GetOvertakesAsync(ct),
        },
        new EndpointTest<PitFilterFields, PitStop>
        {
            Endpoint = "pit",
            Method = "GetPitAsync()",
            CreateQuery = (client, ct) => client.GetPitAsync(ct),
            Options = DriverDetailOptions<PitFilterFields, PitStop>(),
        },
        new EndpointTest<PositionFilterFields, Position>
        {
            Endpoint = "position",
            Method = "GetPositionAsync()",
            CreateQuery = (client, ct) => client.GetPositionAsync(ct),
            Options = DriverDetailOptions<PositionFilterFields, Position>(),
        },
        new EndpointTest<RaceControlFilterFields, RaceControlMessage>
        {
            Endpoint = "race_control",
            Method = "GetRaceControlAsync()",
            CreateQuery = (client, ct) => client.GetRaceControlAsync(ct),
            Options = DriverDetailOptions<RaceControlFilterFields, RaceControlMessage>(),
        },
        new EndpointTest<SessionResultFilterFields, SessionResult>
        {
            Endpoint = "session_result",
            Method = "GetSessionResultAsync()",
            CreateQuery = (client, ct) => client.GetSessionResultAsync(ct),
            Options = DriverDetailOptions<SessionResultFilterFields, SessionResult>(),
        },
        new EndpointTest<SessionsFilterFields, Session>
        {
            Endpoint = "sessions",
            Method = "GetSessionsAsync()",
            CreateQuery = (client, ct) => client.GetSessionsAsync(ct),
        },
        new EndpointTest<StartingGridFilterFields, StartingGridPosition>
        {
            Endpoint = "starting_grid",
            Method = "GetStartingGridAsync()",
            CreateQuery = (client, ct) => client.GetStartingGridAsync(ct),
            Options = DriverDetailOptions<StartingGridFilterFields, StartingGridPosition>(),
        },
        new EndpointTest<StintsFilterFields, Stint>
        {
            Endpoint = "stints",
            Method = "GetStintsAsync()",
            CreateQuery = (client, ct) => client.GetStintsAsync(ct),
            Options = DriverDetailOptions<StintsFilterFields, Stint>(),
        },
        new EndpointTest<TeamRadioFilterFields, TeamRadioMessage>
        {
            Endpoint = "team_radio",
            Method = "GetTeamRadioAsync()",
            CreateQuery = (client, ct) => client.GetTeamRadioAsync(ct),
            Options = DriverDetailOptions<TeamRadioFilterFields, TeamRadioMessage>(),
        },
        new EndpointTest<WeatherFilterFields, Weather>
        {
            Endpoint = "weather",
            Method = "GetWeatherAsync()",
            CreateQuery = (client, ct) => client.GetWeatherAsync(ct),
        },
        new SingleCallTest
        {
            Endpoint = "sessions_latest",
            Method = "GetLatestSessionAsync()",
            Call = async (client, ct) => await client.GetLatestSessionAsync(ct),
        },
        new SingleCallTest
        {
            Endpoint = "meetings_latest",
            Method = "GetLatestMeetingAsync()",
            Call = async (client, ct) => await client.GetLatestMeetingAsync(ct),
        },
    ];

    // Declared in this order on purpose: with both selected, the image-resolving variant is applied last
    // and wins, which is what a user ticking both would mean.
    static QueryOption<TFields, TModel>[] DriverDetailOptions<TFields, TModel>() =>
    [
        new QueryOption<TFields, TModel>(
            ".IncludeDriverDetails() - attach each row's /drivers record",
            query => ((DriverEnrichableQuery<TFields, TModel>)query).IncludeDriverDetails()),
        new QueryOption<TFields, TModel>(
            ".IncludeDriverDetails(resolveImages: true) - and resolve that driver's image URLs",
            query => ((DriverEnrichableQuery<TFields, TModel>)query).IncludeDriverDetails(resolveImages: true)),
    ];
}
