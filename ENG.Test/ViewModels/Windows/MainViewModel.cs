using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ENG.Test.Models;
using ENG.Test.Views;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ENG.Test.ViewModels;

/// <summary>
/// The view model for the <see cref="MainView"/>.
/// </summary>
public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly ElementManager _elementManager;
    private readonly ViewScannerManager _scannerManager;
    private readonly ISnackbarService _snackbarService;
    private readonly TransactionManager _transactionManager;

    /// <summary>
    /// All the scanned parameters inside revit.
    /// </summary>
    [ObservableProperty] private bool _activeViewCanBeScanned;

    /// <summary>
    /// All the scanned parameters inside revit.
    /// </summary>
    [ObservableProperty] private bool _activeViewHasBeenScanned = false;

    /// <summary>
    /// The name of the active view.
    /// </summary>
    [ObservableProperty] private string _activeViewName;

    private IEnumerable<ScannedParameter> _currentScanResult = Enumerable.Empty<ScannedParameter>();
    private ScanView _currentView;
    private bool _disposed = false;

    /// <summary>
    /// True if the active view is being scanned.
    /// </summary>
    [ObservableProperty] private bool _isScanning = false;

    /// <summary>
    /// True if the <see cref="SearchedParameter"/> is valid, otherwise false.
    /// </summary>
    [ObservableProperty] private bool _isValidSearchParameter = false;

    /// <summary>
    /// True if the <see cref="SearchedParameterValue"/> is valid, otherwise false.
    /// </summary>
    [ObservableProperty] private bool _isValidSearchParameterValue = false;

    /// <summary>
    /// All the scanned parameters inside revit.
    /// </summary>
    [ObservableProperty] private IList<string> _parameters;

    /// <summary>
    /// The values of the <see cref="SearchedParameter"/>.
    /// </summary>
    [ObservableProperty] private IList<string> _parameterValues = new List<string>();

    /// <summary>
    /// All the scanned parameters inside revit.
    /// </summary>
    [ObservableProperty] private string _searchedParameter = "";

    /// <summary>
    /// The values of the <see cref="SearchedParameter"/>.
    /// </summary>
    [ObservableProperty] private string _searchedParameterValue = string.Empty;

    /// <summary>
    /// Constructs a new <see cref="MainViewModel"/>.
    /// </summary>
    public MainViewModel(ViewScannerManager scannerManager,
                         TransactionManager transactionManager,
                         ElementManager elementManager,
                         ISnackbarService snackbarService)
    {
        _scannerManager = scannerManager;
        _transactionManager = transactionManager;
        _elementManager = elementManager;
        _snackbarService = snackbarService;

        _isScanning = false;
        _parameters = new List<string>();

        #region Current view properties

        _currentView = _scannerManager.CurrentView;
        _activeViewHasBeenScanned = _currentView.HasBeenScanned;
        _activeViewCanBeScanned = _currentView.CanBeScanned;
        _activeViewName = _currentView.Name;
        _currentView.PropertyChanged += OnCurrentViewPropertyChanged;

        #endregion Current view properties

        #region Model Subscription

        _scannerManager.CurrentViewChanged += OnScannerManagerCurrentViewChanged;

        #endregion Model Subscription
    }

    /// <summary>
    /// Change this <see cref="Parameters"/> and <see cref="ParameterValues"/>
    /// based on the <see cref="_currentView"/>.
    /// </summary>
    private void ChangeParameters()
    {
        if (_currentView.HasBeenScanned == false)
        {
            _currentScanResult = new List<ScannedParameter>();
            this.Parameters = new List<string>();
        }

        _currentScanResult = _scannerManager.GetScanResult(_currentView);
        this.Parameters = _currentScanResult.Select(scannedParameter => scannedParameter.Name).ToList();
    }

    /// <summary>
    /// Handles the change of the has been scanned property of the <see cref="_currentView"/>.
    /// </summary>
    private void OnCurrentViewPropertyChanged(object sender, EventArgs e)
    {
        this.ActiveViewHasBeenScanned = _currentView.HasBeenScanned;

        this.ChangeParameters();
    }

    /// <summary>
    /// Calls the <see cref="_elementManager"/> to isolate the elements of the
    /// current <see cref="SearchedParameterValue"/> in the <see cref="_currentView"/>.
    /// </summary>
    [RelayCommand]
    private async Task OnIsolateElements()
    {
        try
        {
            var parameterValueElements = _currentScanResult.First(scannedParameter => scannedParameter.Name == this.SearchedParameter).Values
                                                           .First(value => value.Value == this.SearchedParameterValue);
            await _transactionManager.InsideOfTransaction(_ => _elementManager.IsolateElements(parameterValueElements, _currentView),
                                                          "Isolate elements");
            _snackbarService.Show("Elements Isolated",
                                  $"{parameterValueElements.Count()} element(s) was/were isolate",
                                  ControlAppearance.Info,
                                  new SymbolIcon(SymbolRegular.Warning24),
                                  TimeSpan.FromSeconds(9));
        }
        catch
        {
            _snackbarService.Show("Isolate failed",
                                  "The Isolate of the elements failed. This could possible be" +
                                  " because some of the elements where deleted in the revit interface." +
                                  " Please scan the view again in order to avoid this error.",
                                  ControlAppearance.Danger,
                                  new SymbolIcon(SymbolRegular.Fluent24),
                                  TimeSpan.FromSeconds(20));

            _currentView.HasBeenScanned = false;
        }
    }

    /// <summary>
    /// Raised when the <see cref="IsValidSearchParameter"/> changes.
    /// </summary>
    partial void OnIsValidSearchParameterChanged(bool oldValue, bool newValue)
    {
        if (newValue == true && oldValue != newValue)
        {
            this.ParameterValues = _currentScanResult.First(parameter => parameter.Name == this.SearchedParameter)
                                                     .Values.Select(value => value.Value).ToList();
        }
        else
        {
            this.ParameterValues = new List<string>();
        }
    }

    /// <summary>
    /// Scans the current view.
    /// </summary>
    [RelayCommand]
    private async Task OnScan()
    {
        try
        {
            await _transactionManager.InsideOfTransaction(_ => _scannerManager.Scan(_currentView),
                                                         "Isolate elements",
                                                         false);
        }
        catch
        {
            _snackbarService.Show("Error on scanning the current view parameters",
                                  "An unexpected error ocurred on scanning the current view parameters." +
                                  " Please, contact me at https://www.linkedin.com/in/migueltamara/.",
                                  ControlAppearance.Danger,
                                  new SymbolIcon(SymbolRegular.Fluent24),
                                  TimeSpan.FromSeconds(20));
        }
    }

    /// <summary>
    /// Raised when the active view on the document changes. This resets the
    /// view binding to mathch the actual views.
    /// </summary>
    private void OnScannerManagerCurrentViewChanged(object sender, EventArgs e)
    {
        _currentView.PropertyChanged -= OnCurrentViewPropertyChanged;
        _currentView = _scannerManager.CurrentView;
        _currentView.PropertyChanged += OnCurrentViewPropertyChanged;
        this.ActiveViewName = _currentView.Name;
        this.ActiveViewCanBeScanned = _currentView.CanBeScanned;
        this.ActiveViewHasBeenScanned = _currentView.HasBeenScanned;

        this.ResetInputs();
    }

    /// <summary>
    /// Raised when the <see cref="SearchedParameter"/> changes. This checks if
    /// a valid parameter has been searched.
    /// </summary>
    partial void OnSearchedParameterChanged(string? oldValue, string newValue)
    {
        this.IsValidSearchParameterValue = false;
        this.IsValidSearchParameter = this.Parameters.Contains(newValue);
    }

    /// <summary>
    /// Raised when the <see cref="SearchedParameter"/> changes. This checks if
    /// a valid parameter has been searched.
    /// </summary>
    partial void OnSearchedParameterValueChanged(string? oldValue, string newValue)
    {
        this.IsValidSearchParameterValue = this.ParameterValues.Contains(newValue);
    }

    /// <summary>
    /// Calls the <see cref="_elementManager"/> to select the elements of the
    /// current <see cref="SearchedParameterValue"/>.
    /// </summary>
    [RelayCommand]
    private void OnSelectElements()
    {
        try
        {
            var parameterValueElements = _currentScanResult.First(scannedParameter => scannedParameter.Name == this.SearchedParameter).Values
                                                           .First(value => value.Value == this.SearchedParameterValue);

            _elementManager.SelectElements(parameterValueElements);

            _snackbarService.Show("Elements Selected",
                                  $"{parameterValueElements.Count()} element(s) was/were selected.",
                                  ControlAppearance.Info,
                                  new SymbolIcon(SymbolRegular.Warning24),
                                  TimeSpan.FromSeconds(9));
        }
        catch
        {
            _snackbarService.Show("Selection failed",
                                  "The selection of the elements failed. This could possible be" +
                                  " because some elements where deleted in the revit interface." +
                                  " Please scan the view again in order to avoid this error.",
                                  ControlAppearance.Danger,
                                  new SymbolIcon(SymbolRegular.Fluent24),
                                  TimeSpan.FromSeconds(20));

            _currentView.HasBeenScanned = false;
        }
    }

    /// <summary>
    /// Reset the <see cref="SearchedParameter"/> and the <see cref="SearchedParameterValue"/>.
    /// </summary>
    private void ResetInputs()
    {
        this.SearchedParameter = string.Empty;
        this.SearchedParameterValue = string.Empty;
    }

    /// <summary>
    /// Releases the events subscriptions.
    /// </summary>
    public void Dispose()
    {
        _currentView.PropertyChanged -= OnCurrentViewPropertyChanged;

        _scannerManager.CurrentViewChanged -= OnScannerManagerCurrentViewChanged;
    }
}