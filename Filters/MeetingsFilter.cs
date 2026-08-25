using OpenF1.Net.Models;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Filters;

public class MeetingsFilterFields
{
    public int CircuitKey => throw new NotSupportedException();
    public string CircuitShortName => throw new NotSupportedException();
    public CircuitType CircuitType => throw new NotSupportedException();
    public string CountryCode => throw new NotSupportedException();
    public int CountryKey => throw new NotSupportedException();
    public string CountryName => throw new NotSupportedException();
    public DateTime DateEnd => throw new NotSupportedException();
    public DateTime DateStart => throw new NotSupportedException();
    public bool IsCancelled => throw new NotSupportedException();
    public string Location => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public string MeetingName => throw new NotSupportedException();
    public string MeetingOfficialName => throw new NotSupportedException();
    public int Year => throw new NotSupportedException();
}

public class MeetingsQuery(
    Func<string, CancellationToken, Task<Meeting[]>> execute,
    Func<string, CancellationToken, Task<CircuitInfo>> fetchCircuitInfo,
    CancellationToken ct
) : EndpointQuery<MeetingsFilterFields, Meeting>(execute, ct)
{
    bool _includeCircuitInfo;

    /// <summary>
    /// Also fetches each meeting's <see cref="Meeting.CircuitInfo"/> from its <see cref="Meeting.CircuitInfoUrl"/>
    /// — one extra HTTP request per meeting, sent to MultiViewer rather than the OpenF1Client API.
    /// </summary>
    public MeetingsQuery IncludeCircuitInfo()
    {
        _includeCircuitInfo = true;
        return this;
    }

    protected override async Task<Meeting[]> ExecuteAsync()
    {
        var meetings = await base.ExecuteAsync().ConfigureAwait(false);
        if (!_includeCircuitInfo)
            return meetings;

        foreach (var meeting in meetings)
            meeting.CircuitInfo = await fetchCircuitInfo(meeting.CircuitInfoUrl, CancellationToken).ConfigureAwait(false);

        return meetings;
    }
}
