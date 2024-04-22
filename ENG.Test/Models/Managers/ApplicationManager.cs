using Autodesk.Revit.UI;

namespace ENG.Test.Models;

/// <summary>
/// Bridge between revit application events and our application ones.
/// </summary>
public class ApplicationManager
{
    private readonly string _documentName;

    /// <summary>
    /// Raised when the revit active document changes.
    /// </summary>
    public event EventHandler? OnActiveDocumentChange;

    /// <summary>
    /// Constructs a new <see cref="ApplicationManager"/>.
    /// </summary>
    /// <param name="revitApplication"></param>
    public ApplicationManager(UIApplication revitApplication)
    {
        _documentName = revitApplication.ActiveUIDocument.Document.Title;

        revitApplication.Application.DocumentClosed += (sender, e) => OnActiveDocumentChange?.Invoke(this, EventArgs.Empty);
        revitApplication.ViewActivated += OnViewActivated;
    }

    /// <summary>
    /// As revit works, if you have multiple documents opened, the only way to
    /// notice if you navigate between one or the other is to subscribe into the
    /// ViewActivated event and compare the view document name with the name of
    /// the document that was used to create this class.
    /// </summary>
    private void OnViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
    {
        if (e.Document.Title == _documentName) return;

        OnActiveDocumentChange?.Invoke(this, EventArgs.Empty);
    }
}