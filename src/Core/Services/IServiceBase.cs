using System;
using System.Threading;
using System.Threading.Tasks;

namespace TopNotch.Core.Services;

/// <summary>
/// Base interface for all services
/// </summary>
public interface IServiceBase : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the service is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets a value indicating whether the service has been initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Starts the service.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the start operation.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the service.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the stop operation.</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Base implementation for services
/// </summary>
public abstract class ServiceBase : IServiceBase
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isRunning;
    private bool _isInitialized;

    public bool IsRunning => _isRunning;
    public bool IsInitialized => _isInitialized;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning) return;

        _isInitialized = true;
        _isRunning = true;

        await OnStartAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning) return;

        _isRunning = false;

        await OnStopAsync(cancellationToken).ConfigureAwait(false);
    }

    protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        return ValueTask.CompletedTask;
    }
}