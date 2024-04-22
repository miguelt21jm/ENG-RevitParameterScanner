using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ENG.Test.Models;

public class ViewScannerManager
{
    private readonly Document _document;
    private readonly Dictionary<ScanView, IEnumerable<ScannedParameter>> _scanResults = new Dictionary<ScanView, IEnumerable<ScannedParameter>>();
    private readonly Dictionary<View, ScanView> _scanViews = new Dictionary<View, ScanView>(new ViewComparer());
    private ScanView _currentView;

    public event EventHandler? CurrentViewChanged;

    /// <summary>
    /// The current <see cref="ScanView"/> on revit.
    /// </summary>
    public ScanView CurrentView
    {
        get => _currentView;
        private set
        {
            if (_currentView != value)
            {
                _currentView = value;
                CurrentViewChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Constructs a new <see cref="ViewScannerManager"/>.
    /// </summary>
    public ViewScannerManager(Document document, UIApplication revitApplication)
    {
        _document = document;

        var activeView = revitApplication.ActiveUIDocument.ActiveView;

        _currentView = new ScanView(activeView, this.CanBeScanned(activeView));

        _scanViews.Add(activeView, _currentView);

        revitApplication.ViewActivated += OnViewChanged;
    }

    /// <summary>
    /// Raised when the revit view changes. This raise the
    /// <see cref="ViewScanned"/> event.
    /// </summary>
    private void OnViewChanged(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
    {
        var activeView = e.CurrentActiveView;

        if (!_scanViews.TryGetValue(activeView, out var scanView))
        {
            _scanViews.Add(activeView, scanView = new ScanView(activeView, this.CanBeScanned(activeView)));
        }

        this.CurrentView = scanView;
    }

    /// <summary>
    /// Return true if the provided <paramref name="view"/> can be scanned.
    /// </summary>
    /// <remarks>
    /// We are only going to scan <see cref="View"/>(s) that are floor plans,
    /// ceiling plans, or 3D views.
    /// </remarks>
    public bool CanBeScanned(View view)
    {
        var viewType = view.ViewType;

        return viewType is ViewType.FloorPlan || viewType is ViewType.CeilingPlan || viewType is ViewType.ThreeD;
    }

    /// <summary>
    /// Gets the scan result from the provided <paramref name="scanView"/>.
    /// </summary>
    public IEnumerable<ScannedParameter> GetScanResult(ScanView scanView)
    {
        _scanResults.TryGetValue(scanView, out var scannedParameters);

        return scannedParameters;
    }

    /// <summary>
    /// Return true if the provided <paramref name="view"/> has been scanned.
    /// </summary>
    public bool HasBeenScanned(View view) => _scanViews.TryGetValue(view, out _);

    /// <summary>
    /// Scans the provided <paramref name="view"/>.
    /// </summary>
    public void Scan(ScanView scanView)
    {
        scanView.DisableViewTemporaryHideIsolate();

        var scannedParameters = new Dictionary<string, ScannedParameter>();

        var allRevitElements = new FilteredElementCollector(_document, scanView.Id)
            .WhereElementIsNotElementType()
            .ToElements()
            .Where(element => element.Category?.IsVisibleInUI ?? false)
            .ToList();

        foreach (var element in allRevitElements)
        {
            // Ordered parameters because we only want UI visible parameters.
            foreach (Parameter parameter in element.GetOrderedParameters())
            {
                if (!scannedParameters.TryGetValue(parameter.Definition.Name, out var scannedParameter))
                {
                    scannedParameter = new ScannedParameter(parameter);
                    scannedParameters.Add(parameter.Definition.Name, scannedParameter);
                }

                scannedParameter.AddValueAndElement(parameter.AsValueString() ?? "", element);
            }
        }

        _scanResults[scanView] = scannedParameters.Values;

        scanView.HasBeenScanned = true;
    }
}