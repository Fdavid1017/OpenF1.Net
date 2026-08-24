using System.Globalization;

namespace OpenF1.Net.Filters;

/// <summary>Lets a filter accept either a real meeting_key int or the API's "latest" sentinel.</summary>
public readonly struct MeetingKeyRef
{
    readonly int? _value;
    readonly bool _isLatest;

    MeetingKeyRef(int? value, bool isLatest)
    {
        _value = value;
        _isLatest = isLatest;
    }

    public static readonly MeetingKeyRef Latest = new(null, true);

    public static implicit operator MeetingKeyRef(int value) => new(value, false);

    public static bool operator ==(MeetingKeyRef a, MeetingKeyRef b) => a.Equals(b);
    public static bool operator !=(MeetingKeyRef a, MeetingKeyRef b) => !a.Equals(b);

    public override bool Equals(object? obj) =>
        obj is MeetingKeyRef other && _isLatest == other._isLatest && _value == other._value;

    public override int GetHashCode() => HashCode.Combine(_value, _isLatest);

    public override string ToString() => _isLatest ? "latest" : _value!.Value.ToString(CultureInfo.InvariantCulture);
}
