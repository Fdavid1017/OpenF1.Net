using System.Net;

namespace OpenF1.Net.Exceptions;

/// <summary>
/// Defensive placeholder for 401/403 — the docs describe an OAuth2 requirement for real-time data,
/// but live testing during design never reproduced it. Keeps that case from being lumped into the
/// generic <see cref="OpenF1ApiException"/> bucket if the API ever does start returning it.
/// </summary>
public class OpenF1SubscriptionRequiredException(HttpStatusCode statusCode, string detail) : OpenF1Exception(statusCode, detail);
