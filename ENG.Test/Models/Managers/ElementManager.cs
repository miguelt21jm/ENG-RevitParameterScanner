using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;

namespace ENG.Test.Models;

public class ElementManager
{
    private readonly UIApplication _revitApplication;
    private readonly TransactionManager _transactionManager;

    /// <summary>
    /// Constructs a new <see cref="ElementManager"/>.
    /// </summary>
    public ElementManager(UIApplication revitApplication, TransactionManager transactionManager)
    {
        _revitApplication = revitApplication;
        _transactionManager = transactionManager;
    }

    /// <summary>
    /// Selects the provided <paramref name="elements"/> in the revit ui.
    /// </summary>
    public void SelectElements(IEnumerable<Element> elements)
    {
        var uiDocument = _revitApplication.ActiveUIDocument;

        var elementIds = new Collection<ElementId>();

        foreach (var element in elements) elementIds.Add(element.Id);

        uiDocument.Selection.SetElementIds(elementIds);
    }

    /// <summary>
    /// Isolates the provided <paramref name="elements"/> in the provided <paramref name="view"/>.
    /// </summary>
    public void IsolateElements(IEnumerable<Element> elements, ScanView view)
    {
        view.DisableViewTemporaryHideIsolate();

        var elementIds = new Collection<ElementId>();

        foreach (var element in elements) elementIds.Add(element.Id);

        view.IsolateElementsTemporary(elementIds);
    }
}