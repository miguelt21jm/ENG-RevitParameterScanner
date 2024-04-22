using Autodesk.Revit.DB;

namespace ENG.Test.Models;

/// <summary>
/// A comparer for <see cref="View"/>(s).
/// </summary>
public class ViewComparer : IEqualityComparer<View>
{
    /// <inheritdoc/>
    public bool Equals(View x, View y) => x.Id.IntegerValue == y.Id.IntegerValue;

    /// <inheritdoc/>
    public int GetHashCode(View obj) => obj.Id.IntegerValue.GetHashCode();
}