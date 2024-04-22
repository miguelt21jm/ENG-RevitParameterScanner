using Autodesk.Revit.UI;

namespace ENG.Test.Models;

/// <summary>
/// A class designed to handle <see cref="Action"/> s that needs to be run
/// inside the revit context.
/// </summary>
public sealed class AsyncEventHandler : IExternalEventHandler
{
    private readonly ExternalEvent _externalEvent;
    private readonly string _identifier;
    private Action<UIApplication>? _action;
    private TaskCompletionSource<bool>? _resultTask;

    /// <summary>
    /// Creates an asynchronous event handler.
    /// </summary>
    public AsyncEventHandler()
    {
        _identifier = GetType().Name;
        _externalEvent = ExternalEvent.Create(this);
    }

    /// <summary>
    /// Method called in revit context to handler our request.
    /// </summary>
    public void Execute(UIApplication uiApplication)
    {
        if (_action == null)
        {
            _resultTask!.SetResult(result: false);
            return;
        }
        try
        {
            _action(uiApplication);
            _resultTask!.SetResult(result: false);
        }
        catch (Exception exception)
        {
            _resultTask!.SetException(exception);
        }
        finally
        {
            _action = null;
            _resultTask = null;
        }
    }

    /// <inheritdoc/>
    public string GetName() => _identifier;

    /// <summary>
    /// Instructing Revit to raise (signal) the external event.
    /// </summary>
    public void Raise()
    {
        _externalEvent.Raise();
    }

    /// <summary>
    /// Raise the provided <paramref name="action"/>.
    /// </summary>
    public async Task RaiseAsync(Action<UIApplication> action)
    {
        if (_action == null) _action = action;
        else _action = (Action<UIApplication>)Delegate.Combine(_action, action);

        this.Raise();

        if (_resultTask == null) _resultTask = new TaskCompletionSource<bool>();

        await _resultTask.Task;
    }
}