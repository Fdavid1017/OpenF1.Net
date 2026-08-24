namespace OpenF1.Net.Internal;

/// <summary>Shape of the API's error body: {"detail": "..."}.</summary>
internal record ApiErrorResponse(string? Detail);
