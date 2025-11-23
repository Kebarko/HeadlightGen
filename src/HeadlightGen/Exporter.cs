using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace KE.MSTS.HeadlightGen;

/// <summary>
/// Exports calculated light points to a file using a template format.
/// Generates output compatible with MSTS (Microsoft Train Simulator) light definitions.
/// </summary>
public class Exporter
{
    /// <summary>
    /// Exports light points to a file by replacing placeholders in a template.
    /// </summary>
    /// <param name="centerX">The X coordinate of the light center point.</param>
    /// <param name="centerY">The Y coordinate of the light center point.</param>
    /// <param name="centerZ">The Z coordinate of the light center point.</param>
    /// <param name="circles">The number of concentric circles to export.</param>
    /// <param name="maxRadius">The maximum radius as a percentage.</param>
    /// <param name="increment">The base increment value for segment calculation.</param>
    /// <param name="baseAngle">The starting angle in degrees for segment orientation.</param>
    /// <param name="templatePath">The path to the template file containing {X} and {Y} placeholders.</param>
    /// <param name="outputPath">The path where the generated output file will be written.</param>
    public void Export(float centerX, float centerY, float centerZ, int circles, float maxRadius, int increment, int baseAngle, string templatePath, string outputPath)
    {
        // Calculate points
        var segmentCalculator = new SegmentCalculator();
        var points = segmentCalculator.Calculate(new PointF(centerX, centerY), circles, maxRadius, increment, baseAngle);

        // Read template
        string template = File.ReadAllText(templatePath, Encoding.Unicode);

        // Create string builder for output
        StringBuilder result = new StringBuilder();

        result.AppendLine();
        result.AppendLine($"Lights ( {points.Count}");

        // Iterate over points and replace placeholders in template
        foreach (var point in points)
        {
            result.AppendLine(template
                .Replace("{X}", point.X.ToString("G", NumberFormatInfo.InvariantInfo))
                .Replace("{Y}", point.Y.ToString("G", NumberFormatInfo.InvariantInfo))
                .Replace("{Z}", centerZ.ToString("G", NumberFormatInfo.InvariantInfo)));
        }

        result.AppendLine(")");

        // Write output to file
        File.WriteAllText(outputPath, result.ToString(), Encoding.Unicode);
    }
}
