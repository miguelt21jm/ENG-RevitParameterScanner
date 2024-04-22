using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ENG.Test.Models;

public class TransactionManager
{
    private readonly AsyncEventHandler _asyncEventHandler = new AsyncEventHandler();

    public TransactionManager()
    {
    }

    /// <summary>
    /// Runs the provided <paramref name="action"/> inside of a <see cref="Autodesk.Revit.DB.Transaction"/>.
    /// </summary>
    public async Task InsideOfTransaction(Action<Document> action, string transactionName, bool commit = true)
    {
        // We only want to notice if our current transaction ends. That is the
        // reason of this task competition source.

        Action<UIApplication> revitAction = revitApplication =>
        {
            var document = revitApplication.ActiveUIDocument.Document;

            using var transaction = new Transaction(document, transactionName);

            transaction.Start();

            action.Invoke(document);

            if (commit) transaction.Commit();

            transaction.Dispose();
        };

        await _asyncEventHandler.RaiseAsync(revitAction);
    }
}