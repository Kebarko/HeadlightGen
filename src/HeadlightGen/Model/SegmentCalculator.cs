using System.Drawing;

namespace KE.MSTS.HeadlightGen.Model;

/// <summary>
/// Calculates points arranged in concentric circles with configurable segments.
/// Generates a pattern of points that can be used to represent vehicle headlight positions.
/// </summary>
public class SegmentCalculator
{
    /// <summary>
    /// Calculates a collection of points arranged in concentric circles around a center point.
    /// </summary>
    /// <param name="center">The center point of the circle arrangement.</param>
    /// <param name="circles">The number of concentric circles to generate.</param>
    /// <param name="maxRadius">The maximum radius of the outermost circle</param>
    /// <param name="increment">The number of segments for the first circle; scales up for outer circles.</param>
    /// <param name="rotation">The starting angle in degrees for the first segment.</param>
    /// <param name="elevation">The elevation value to be applied to all light points.</param>
    /// <param name="boundingBox">The bounding rectangle that encloses all generated points.</param>
    /// <returns>A list of points representing the calculated circle segments, starting with the center point.</returns>
    public static IList<Point3D> Calculate(Point3D center, int circles, float maxRadius, int increment, int rotation, int elevation, out RectangleF boundingBox)
    {
        var result = new List<Point3D>();

        if (circles <= 0 || maxRadius <= 0 || increment <= 0)
        {
            boundingBox = new RectangleF();
            return result;
        }

        result.Add(center);

        for (int circle = 1; circle <= circles; circle++)
        {
            float radius = maxRadius / circles * circle;
            int segments = increment * circle;

            for (int seg = 0; seg < segments; seg++)
            {
                float angle = (float)(rotation * Math.PI / 180 + 2.0 * Math.PI * seg / segments);
                float x = (float)Math.Round(center.X + radius * (float)Math.Cos(angle), 3, MidpointRounding.AwayFromZero);
                float y = (float)Math.Round(center.Y + radius * (float)Math.Sin(angle), 3, MidpointRounding.AwayFromZero);
                float z = (float)Math.Round(center.Z + (y - center.Y) * Math.Tan(elevation * Math.PI / 180), 3, MidpointRounding.AwayFromZero);
                result.Add(new Point3D(x, y, z));
            }
        }

        boundingBox = new RectangleF
        {
            X = center.X - maxRadius,
            Y = center.Y - maxRadius,
            Width = 2 * maxRadius,
            Height = 2 * maxRadius
        };

        return result;
    }
}
