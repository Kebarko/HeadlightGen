using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace KE.MSTS.HeadlightGen;

public class LightGenerator
{
    public void GenerateLight(float centerX, float centerY, int circles, float maxRadius, int increment, string templatePath, string outputPath)
    {
        // Calculate points
        var segmentCalculator = new SegmentCalculator();
        var points = segmentCalculator.Calculate(new PointF(centerX, centerY), circles, maxRadius, increment);

        // Read template
        string template = File.ReadAllText(templatePath, Encoding.UTF8);

        // Create string builder for output
        StringBuilder result = new StringBuilder();

        result.AppendLine($"Lights ( {points.Count}");

        // Iterate over points and replace placeholders in template
        foreach (var point in points)
        {
            result.AppendLine(template
                .Replace("{X}", point.X.ToString("G3", NumberFormatInfo.InvariantInfo))
                .Replace("{Y}", point.Y.ToString("G3", NumberFormatInfo.InvariantInfo)));
        }

        result.AppendLine(")");

        // Write output to file
        File.WriteAllText(outputPath, result.ToString(), Encoding.UTF8);
    }
}
