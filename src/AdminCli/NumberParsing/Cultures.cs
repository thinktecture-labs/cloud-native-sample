using System.Globalization;

namespace AdminCli.NumberParsing;

public static class Cultures
{
    /// <summary>
    /// Gets the invariant culture.
    /// </summary>
    public static CultureInfo InvariantCulture => CultureInfo.InvariantCulture;

    /// <summary>
    /// Gets the German culture.
    /// </summary>
    public static CultureInfo GermanCulture { get; } = new ("de-DE");
}
