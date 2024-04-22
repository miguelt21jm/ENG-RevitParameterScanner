using Autodesk.Revit.DB;

namespace ENG.Test.Models;

/// <summary>
/// A comparer for <see cref="Autodesk.Revit.DB.Element"/>(s).
/// </summary>
public class ElementComparer : IEqualityComparer<Element>
{
    /// <inheritdoc />
    public bool Equals(Element x, Element y)
    {
        return x.Id.IntegerValue == y.Id.IntegerValue;
    }

    /// <inheritdoc />
    public int GetHashCode(Element obj)
    {
        return obj.Id.GetHashCode();
    }
}