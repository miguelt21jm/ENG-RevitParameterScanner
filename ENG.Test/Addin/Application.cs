using Autodesk.Revit.UI;
using ENG.Test.Constants;
using ENG.Test.Resources;
using System.Reflection;

namespace ENG.Test.Addin;

/// <summary>
/// The main entry point inside revit's context.
/// </summary>
public class Application : IExternalApplication
{
    private readonly string _tabName = TestsConstants.TabName;
    private readonly string _panelName = TestsConstants.PanelName;
    private readonly string _buttonName = TestsConstants.ButtonName;

    /// <inheritdoc/>
    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }

    /// <inheritdoc/>
    public Result OnStartup(UIControlledApplication application)
    {
        this.CreateRibbon(application);
        return Result.Succeeded;
    }

    /// <summary>
    /// Creates the ribbon tab with its buttons.
    /// </summary>
    private void CreateRibbon(UIControlledApplication application)
    {
        application.CreateRibbonTab(_tabName);

        var panel = application.CreateRibbonPanel(_tabName, _panelName);

        var buttonUniqueIdenfier = Guid.NewGuid().ToString();
        var buttonData = new PushButtonData(buttonUniqueIdenfier,
                                            _buttonName,
                                            ParameterScannerCommand.AssmeblyLocation,
                                            ParameterScannerCommand.FullClassName)
        {
            LargeImage = ImageManager.GetIcon("main-icon-32.png"),
        };

        _ = panel.AddItem(buttonData);
    }
}