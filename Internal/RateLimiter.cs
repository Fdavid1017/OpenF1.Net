namespace OpenF1.Net.Internal;

/// <summary>
/// Paces outgoing requests so no more than 3 land inside any trailing 1-second window — a true
/// sliding window, not fixed-interval spacing, since spacing calls exactly 1000/3 ms apart still lets
/// 4 requests fall inside a single rolling second at the boundary (0, 333, 666, 1000ms).
/// </summary>
internal class RateLimiter
{
    const int MaxRequestsPerWindow = 3;
    static readonly TimeSpan Window = TimeSpan.FromSeconds(1);

    readonly SemaphoreSlim _gate = new(1, 1);
    readonly Queue<DateTime> _recentRequests = new();

    public async Task WaitAsync(CancellationToken ct)
    {
        await _gate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            while (true)
            {
                var now = DateTime.UtcNow;
                while (_recentRequests.Count > 0 && now - _recentRequests.Peek() >= Window)
                    _recentRequests.Dequeue();

                if (_recentRequests.Count < MaxRequestsPerWindow)
                    break;

                var waitFor = Window - (now - _recentRequests.Peek());
                if (waitFor > TimeSpan.Zero)
                    await Task.Delay(waitFor, ct).ConfigureAwait(false);
            }
            _recentRequests.Enqueue(DateTime.UtcNow);
        }
        finally
        {
            _gate.Release();
        }
    }
}
