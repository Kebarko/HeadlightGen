using System.Drawing;

namespace KE.MSTS.HeadlightGen;

public class SegmentCalculator
{
    public IList<PointF> Calculate(PointF center, int circles, float maxRadius, int increment)
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
                float angle = (float)(seg * 2.0 * Math.PI / segments);
                float x = center.X + radius * (float)Math.Cos(angle);
                float y = center.Y + radius * (float)Math.Sin(angle);
                result.Add(new PointF(x, y));
            }
        }
        
        return result;
    }
}
