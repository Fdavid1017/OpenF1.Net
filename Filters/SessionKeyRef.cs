using System.Globalization;

namespace OpenF1.Net.Filters;

/// <summary>Lets a filter accept either a real session_key int or the API's "latest" sentinel.</summary>
public readonly struct SessionKeyRef
{
    readonly int? _value;
    readonly bool _isLatest;

    SessionKeyRef(int? value, bool isLatest)
    {
        _value = value;
        _isLatest = isLatest;
    }

    public static readonly SessionKeyRef Latest = new(null, true);

    public static implicit operator SessionKeyRef(int value) => new(value, false);

    public static bool operator ==(SessionKeyRef a, SessionKeyRef b) => a.Equals(b);
    public static bool operator !=(SessionKeyRef a, SessionKeyRef b) => !a.Equals(b);

    public override bool Equals(object? obj) =>
        obj is SessionKeyRef other && _isLatest == other._isLatest && _value == other._value;

    public override int GetHashCode() => HashCode.Combine(_value, _isLatest);

    public override string ToString() => _isLatest ? "latest" : _value!.Value.ToString(CultureInfo.InvariantCulture);
}
