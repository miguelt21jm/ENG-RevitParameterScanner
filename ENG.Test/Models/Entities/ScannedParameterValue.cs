using Autodesk.Revit.DB;
using System.Collections;

namespace ENG.Test.Models;

/// <summary>
/// Represents the value of a <see cref="Parameter"/>.
/// </summary>
public class ScannedParameterValue : IEnumerable<Element>
{
    private readonly HashSet<Element> _elements = new HashSet<Element>(new ElementComparer());

    /// <summary>
    /// The <see cref="Autodesk.Revit.DB.Element"/>(s) that have this <see cref="ScannedParameterValue"/>.
    /// </summary>
    public IEnumerable<Element> Elements => _elements;

    /// <summary>
    /// The value as string of this <see cref="ScannedParameterValue"/>.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Constructs a new <see cref="ScannedParameterValue"/>.
    /// </summary>
    public ScannedParameterValue(string value)
    {
        this.Value = value;
    }

    /// <inheritdoc/>
    public IEnumerator<Element> GetEnumerator() => _elements.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>
    /// Adds the provided <paramref name="element"/> into this <see cref="Elements"/>.
    /// </summary>
    public void AddElement(Element element) => _elements.Add(element);
}