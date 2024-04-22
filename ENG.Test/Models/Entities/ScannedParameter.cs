using Autodesk.Revit.DB;

namespace ENG.Test.Models;

/// <summary>
/// Represents the scan of a <see cref="Parameter"/> with the same name (see <see cref="Name"/>).
/// </summary>
public class ScannedParameter
{
    private readonly Dictionary<string, ScannedParameterValue> _values = new Dictionary<string, ScannedParameterValue>();

    /// <summary>
    /// The name of the scanned parameter.
    /// </summary>
    /// <remarks>
    /// As revit works, this name can belong to multiple <see cref="Parameter"/>(s).
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// The values associated with this <see cref="ScannedElement"/>.
    /// </summary>
    public IEnumerable<ScannedParameterValue> Values => _values.Values;

    /// <summary>
    /// Constructs a new <see cref="ScannedParameter"/>.
    /// </summary>
    public ScannedParameter(Parameter revitParameter)
    {
        this.Name = revitParameter.Definition.Name;
    }

    /// <summary>
    /// Adds the <paramref name="newValue"/> and its associated
    /// <paramref name="element"/> in this <see cref="Values"/>. If the value
    /// already exists, the method adds the <paramref name="element"/> to the
    /// existing entry.
    /// </summary>
    public void AddValueAndElement(string newValue, Element element)
    {
        if (!_values.TryGetValue(newValue, out var scannedParameterValue))
        {
            scannedParameterValue = new ScannedParameterValue(newValue);
            _values.Add(newValue, scannedParameterValue);
        }

        scannedParameterValue.AddElement(element);
    }
}