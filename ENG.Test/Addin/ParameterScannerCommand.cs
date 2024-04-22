using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ENG.Test.Models;
using ENG.Test.ViewModels;
using ENG.Test.Views;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Windows;
using Wpf.Ui;

namespace ENG.Test.Addin;

/// <summary>
/// An <see cref="IExternalCommand"/> that start the parameter scanner logic.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
[Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
public class ParameterScannerCommand : IExternalCommand
{
    /// <summary>
    /// The location path in which this <see cref="ParameterScannerCommand"/> is.
    /// </summary>
    internal static string AssmeblyLocation = Assembly.GetExecutingAssembly().Location;

    /// <summary>
    /// Represents the fully qualified name of the
    /// <see cref="ParameterScannerCommand"/> class.
    /// </summary>
    internal static string FullClassName = typeof(ParameterScannerCommand).Namespace + "." + nameof(ParameterScannerCommand);

    /// <summary>
    /// Creates the IoC Container as a <see cref="ServiceProvider"/>.
    /// </summary>
    private IServiceProvider GetServiceProvider(ExternalCommandData externalCommandData)
    {
        var revitApplication = externalCommandData.Application;

        var document = revitApplication.ActiveUIDocument.Document;

        var serviceCollection = new ServiceCollection();

        // Revit main classes for convenience
        serviceCollection.AddSingleton(document);
        serviceCollection.AddSingleton(revitApplication);

        // Models
        serviceCollection.AddSingleton<ViewScannerManager>();
        serviceCollection.AddSingleton<ElementManager>();
        serviceCollection.AddSingleton<ApplicationManager>();
        serviceCollection.AddSingleton<TransactionManager>();

        // ViewModels
        serviceCollection.AddTransient<MainViewModel>();
        serviceCollection.AddSingleton<ISnackbarService, SnackbarService>();
        //Views
        serviceCollection.AddTransient<MainView>();

        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Resolves assembly dependencies for the current assembly. When the
    /// application requires an assembly that is not already loaded, this method
    /// attempts to load the assembly from a specified directory.
    /// </summary>
    private Assembly? OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
        var appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var addInPath = Path.Combine(appDataRoaming, "Autodesk", "Revit", "Addins", "2023", "Eng Test");

        var assemblyName = new AssemblyName(args.Name).Name + ".dll";

        string assemblyPath = Path.Combine(addInPath, assemblyName);

        if (File.Exists(assemblyPath))
        {
            return Assembly.LoadFrom(assemblyPath);
        }

        return null;
    }

    /// <inheritdoc/>
    public Result Execute(ExternalCommandData commandData,
                          ref string message,
                          ElementSet elements)
    {
        var document = commandData.Application.ActiveUIDocument.Document;

        // Workaround to force creation of dispatcher.
        if (System.Windows.Application.Current == null)
        {
            _ = new System.Windows.Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }

        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

        var serviceProvider = GetServiceProvider(commandData);

        serviceProvider.GetRequiredService<MainView>().Show();

        return Result.Succeeded;
    }
}