using System.Drawing;

namespace KE.MSTS.HeadlightGen;

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
    /// <param name="maxRadius">The maximum radius of the outermost circle as a percentage.</param>
    /// <param name="increment">The number of segments for the first circle; scales up for outer circles.</param>
    /// <param name="baseAngle">The starting angle in degrees for the first segment.</param>
    /// <returns>A list of points representing the calculated circle segments, starting with the center point.</returns>
    public IList<PointF> Calculate(PointF center, int circles, float maxRadius, int increment, int baseAngle)
    {
        var result = new List<PointF>();
        
        if (circles <= 0 || maxRadius <= 0 || increment <= 0)
        {
            return result;
        }
        
        result.Add(center);

        for (int circle = 1; circle <= circles; circle++)
        {
            float radius = maxRadius / 100 / circles * circle;
            int segments = increment * circle;

            for (int seg = 0; seg < segments; seg++)
            {
                float angle = (float)(baseAngle * Math.PI / 180 + 2.0 * Math.PI * seg / segments);
                float x = (float)Math.Round(center.X + radius * (float)Math.Cos(angle), 3, MidpointRounding.AwayFromZero);
                float y = (float)Math.Round(center.Y + radius * (float)Math.Sin(angle), 3, MidpointRounding.AwayFromZero);
                result.Add(new PointF(x, y));
            }
        }
        
        return result;
    }
}
