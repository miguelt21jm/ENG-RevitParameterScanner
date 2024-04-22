using Autodesk.Revit.DB;
using System.ComponentModel;

namespace ENG.Test.Models;

/// <summary>
/// Represents a <see cref="Autodesk.Revit.DB.View"/> which parameters can or
/// cannot be scanned.
/// </summary>
public class ScanView : INotifyPropertyChanged
{
    private bool _hasBeenScanned;
    private readonly View _wrappedView;

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Returns true if this <see cref="ScanView"/> can be scanned.
    /// </summary>
    public bool CanBeScanned { get; }

    /// <summary>
    /// Returns true if this <see cref="ScanView"/> has been scanned.
    /// </summary>
    public bool HasBeenScanned
    {
        get => _hasBeenScanned;
        internal set
        {
            if (_hasBeenScanned != value)
            {
                _hasBeenScanned = value;
                OnPropertyChanged(nameof(HasBeenScanned));
            }
        }
    }

    /// <summary>
    /// The <see cref="Autodesk.Revit.DB.ElementId"/> of this <see cref="ScanView"/>.
    /// </summary>
    public ElementId Id { get; }

    /// <summary>
    /// The name of this <see cref="ScanView"/>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Constructs a new <see cref="ScanView"/>.
    /// </summary>
    public ScanView(View view, bool canBeScanned)
    {
        _wrappedView = view;
        this.Name = _wrappedView.Name;
        this.Id = _wrappedView.Id;
        this.CanBeScanned = canBeScanned;
        // By default we are going to assume that a new ScanView hasn't be scanned.
        this.HasBeenScanned = false;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Isolates the provided <paramref name="elements"/> in this view.
    /// </summary>
    public void IsolateElementsTemporary(ICollection<ElementId> elements) => _wrappedView.IsolateElementsTemporary(elements);

    /// <summary>
    /// Ends the isolation of elements of this <see cref="ScanView"/>.
    /// </summary>
    public void DisableViewTemporaryHideIsolate() => _wrappedView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
}