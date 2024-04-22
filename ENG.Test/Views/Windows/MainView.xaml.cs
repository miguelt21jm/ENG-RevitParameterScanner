using Autodesk.Revit.UI;
using ENG.Test.Models;
using ENG.Test.ViewModels;
using Wpf.Ui;

namespace ENG.Test.Views;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView
{
    /// <summary>
    /// The view model of this <see cref="MainView"/>.
    /// </summary>
    public MainViewModel ViewModel { get; }

    /// <summary>
    /// Constructs a new <see cref="MainView"/>.
    /// </summary>
    public MainView(MainViewModel viewModel,
                    ApplicationManager applicationManager,
                    ISnackbarService snackbarService)
    {
        this.ViewModel = viewModel;

        this.InitializeComponent();

        this.DataContext = this;
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);

        this.Closed += (e, args) => viewModel.Dispose();

        // We are not going to handle multiple revit documents.
        applicationManager.OnActiveDocumentChange += (e, args) => this.Close();
    }
}