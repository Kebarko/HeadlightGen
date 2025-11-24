using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace KE.MSTS.HeadlightGen.Services;

/// <summary>
/// Exports calculated light points to a file using a template format.
/// Generates output compatible with MSTS (Microsoft Train Simulator) light definitions.
/// </summary>
public class Exporter
{
    /// <summary>
    /// Exports calculated light points to an output file by applying them to a template.
    /// </summary>
    /// <param name="title">Optional title comment to include in the output file. If null, no title comment is added.</param>
    /// <param name="points">A collection of light point coordinates (X, Y) to be exported.</param>
    /// <param name="centerZ">The Z-coordinate (depth) value to be applied to all light points.</param>
    /// <param name="templatePath">The file path to the template file containing placeholder values ({X}, {Y}, {Z}).</param>
    /// <param name="outputPath">The file path where the generated output will be written.</param>
    public static void Export(string? title, IList<PointF> points, float centerZ, string templatePath, string outputPath)
    {
        // Read template
        string template = File.ReadAllText(templatePath, Encoding.Unicode);

        // Create string builder for output
        StringBuilder result = new();

        // Add title comment
        if (!string.IsNullOrEmpty(title))
            result.AppendLine($"comment ( {title} )");

        // Iterate over points and replace placeholders in template
        foreach (var point in points)
        {
            result.AppendLine(template
                .Replace("{X}", point.X.ToString("G", NumberFormatInfo.InvariantInfo))
                .Replace("{Y}", point.Y.ToString("G", NumberFormatInfo.InvariantInfo))
                .Replace("{Z}", centerZ.ToString("G", NumberFormatInfo.InvariantInfo)));
        }

        // Write output to file
        File.WriteAllText(outputPath, result.ToString(), Encoding.Unicode);
    }
}
