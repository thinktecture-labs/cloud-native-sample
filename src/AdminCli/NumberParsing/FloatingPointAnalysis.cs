using System;

namespace AdminCli.NumberParsing;

/// <summary>
/// Provides a member to analyse a string for points and commas. This is
/// usually used to determine the decimal sign and thousand-delimiter sign
/// in strings to parse them in an appropriate culture.
/// </summary>
public static class FloatingPointAnalysis
{
    /// <summary>
    /// Counts the number of commas and points within the specified text
    /// and determines the last index for each of them. This can be used
    /// to determine the decimal sign and thousand-delimiter sign
    /// in strings.
    /// </summary>
    /// <param name="text">The read-only span that points the the text to be analysed.</param>
    /// <returns>A structure that holds all results.</returns>
    public static FloatingPointAnalysisResult AnalyseText(ReadOnlySpan<char> text)
    {
        var numberOfCommas = 0;
        var numberOfPoints = 0;
        var indexOfLastComma = -1;
        var indexOfLastPoint = -1;

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];
            switch (character)
            {
                case ',':
                    numberOfCommas++;
                    indexOfLastComma = i;
                    break;
                case '.':
                    numberOfPoints++;
                    indexOfLastPoint = i;
                    break;
            }
        }

        return new (numberOfCommas,
                    indexOfLastComma,
                    numberOfPoints,
                    indexOfLastPoint);
    }
}
