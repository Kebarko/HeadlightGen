using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KE.MSTS.HeadlightGen.Model;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace KE.MSTS.HeadlightGen.Services;

/// <summary>
/// Represents the orthographic view for rendering.
/// </summary>
public enum RenderView
{
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>
/// Renders calculated light points to a WPF canvas with automatic scaling and centering.
/// Handles transformation of calculated coordinates to canvas display coordinates.
/// </summary>
public class Renderer
{
    /// <summary>
    /// Renders a collection of points as ellipses on a WPF canvas with automatic scaling and centering.
    /// </summary>
    /// <param name="canvasWidth">The width of the canvas in pixels.</param>
    /// <param name="canvasHeight">The height of the canvas in pixels.</param>
    /// <param name="points">The collection of points to render.</param>
    /// <param name="view">The orthographic view to render.</param>
    /// <returns>An enumerable collection of WPF Shape elements (Ellipse) ready to be added to the canvas.</returns>
    public static IEnumerable<Shape> Render(int canvasWidth, int canvasHeight, IList<Point3D> points, RenderView view)
    {
        // Handle empty points collection
        if (points.Count == 0)
            yield break;
        
        // Project points to 2D based on the selected view
        var projectedPoints = points.Select(point => Project(point, view)).ToList();

        // Calculate the bounding box of the projected points
        var pointsBoudingBox = GetBoundingBox(projectedPoints);

        // Calculate scale to fit points within canvas
        float scaleX = canvasWidth / pointsBoudingBox.Width;
        float scaleY = canvasHeight / pointsBoudingBox.Height;

        // Use the smaller scale to maintain aspect ratio
        float scale = Math.Min(scaleX, scaleY);

        // Calculate scaled dimensions of points bounding box
        float scaledWidth = pointsBoudingBox.Width * scale;
        float scaledHeight = pointsBoudingBox.Height * scale;

        // Calculate offsets to center the drawing
        float offsetX = (canvasWidth - scaledWidth) / 2;
        float offsetY = (canvasHeight - scaledHeight) / 2;

        // Transform points to canvas coordinates and flip Y axis
        float minX = pointsBoudingBox.X;
        float minY = pointsBoudingBox.Y;
        var scaledPoints = projectedPoints.Select(p => new PointF
        {
            X = (p.X - minX) * scale + offsetX,
            Y = scaledHeight - (p.Y - minY) * scale + offsetY
        }).ToList();

        // Draw points on canvas
        bool first = true;
        foreach (PointF point in scaledPoints)
        {
            var ellipse = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = first ? Brushes.Red : Brushes.Green
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
            yield return ellipse;

            first = false;
        }
    }

    /// <summary>
    /// Projects a 3D point to 2D based on the selected view.
    /// </summary>
    /// <param name="point">The 3D point to project.</param>
    /// <param name="view">The render view to use for projection.</param>
    /// <returns>The projected 2D point.</returns>
    private static PointF Project(Point3D point, RenderView view)
    {
        return view switch
        {
            RenderView.Front => new PointF(point.X, point.Y),
            RenderView.Back => new PointF(-point.X, point.Y),
            RenderView.Left => new PointF(point.Z, point.Y),
            RenderView.Right => new PointF(-point.Z, point.Y),
            RenderView.Top => new PointF(point.X, point.Z),
            RenderView.Bottom => new PointF(-point.X, point.Z),
            _ => throw new ArgumentOutOfRangeException(nameof(view), view, null)
        };
    }

    /// <summary>
    /// Calculates the bounding box of a list of 2D points.
    /// </summary>
    /// <param name="points">The list of 2D points.</param>
    /// <returns>The bounding box.</returns>
    private static RectangleF GetBoundingBox(IList<PointF> points)
    {
        float minX = points.Min(point => point.X);
        float maxX = points.Max(point => point.X);
        float minY = points.Min(point => point.Y);
        float maxY = points.Max(point => point.Y);

        return new RectangleF(minX, minY, Math.Max(maxX - minX, float.Epsilon), Math.Max(maxY - minY, float.Epsilon));
    }

    /// <summary>
    /// Builds a 3ds Max-style coloured axis tripod indicating world orientation for the selected orthographic view.
    /// The two axes lying in the view plane are drawn as arrows; the axis pointing straight at/away from the
    /// viewer has no visible length, so it is drawn as a coloured dot at the origin instead.
    /// </summary>
    /// <param name="view">The orthographic view the tripod should represent.</param>
    /// <param name="originX">The X coordinate, in canvas pixels, of the tripod's origin.</param>
    /// <param name="originY">The Y coordinate, in canvas pixels, of the tripod's origin.</param>
    /// <param name="axisLength">The length, in canvas pixels, of each arrow.</param>
    /// <returns>An enumerable collection of WPF elements ready to be added to a canvas.</returns>
    public static IEnumerable<UIElement> RenderAxisTripod(RenderView view, double originX, double originY, double axisLength = 32)
    {
        (char HorizontalAxis, Brush HorizontalColor, int HorizontalSign, char VerticalAxis, Brush VerticalColor, char DepthAxis, Brush DepthColor) config = view switch
        {
            RenderView.Front => ('X', Brushes.Red, 1, 'Y', Brushes.Green, 'Z', Brushes.DodgerBlue),
            RenderView.Back => ('X', Brushes.Red, -1, 'Y', Brushes.Green, 'Z', Brushes.DodgerBlue),
            RenderView.Left => ('Z', Brushes.DodgerBlue, 1, 'Y', Brushes.Green, 'X', Brushes.Red),
            RenderView.Right => ('Z', Brushes.DodgerBlue, -1, 'Y', Brushes.Green, 'X', Brushes.Red),
            RenderView.Top => ('X', Brushes.Red, 1, 'Z', Brushes.DodgerBlue, 'Y', Brushes.Green),
            RenderView.Bottom => ('X', Brushes.Red, -1, 'Z', Brushes.DodgerBlue, 'Y', Brushes.Green),
            _ => throw new ArgumentOutOfRangeException(nameof(view), view, null)
        };

        // Horizontal in-plane axis.
        var horizontalTip = new Point(originX + config.HorizontalSign * axisLength, originY);
        foreach (Shape element in CreateAxisArrow(new Point(originX, originY), horizontalTip, config.HorizontalColor))
            yield return element;
        yield return CreateAxisLabel(horizontalTip.X + config.HorizontalSign * 10, horizontalTip.Y, config.HorizontalAxis, config.HorizontalColor);

        // Vertical in-plane axis.
        var verticalTip = new Point(originX, originY - axisLength);
        foreach (Shape element in CreateAxisArrow(new Point(originX, originY), verticalTip, config.VerticalColor))
            yield return element;
        yield return CreateAxisLabel(verticalTip.X, verticalTip.Y - 12, config.VerticalAxis, config.VerticalColor);
    }

    /// <summary>
    /// Creates the shapes for a single tripod arrow: a shaft and an arrowhead pointing from <paramref name="from"/> to <paramref name="to"/>.
    /// </summary>
    private static IEnumerable<Shape> CreateAxisArrow(Point from, Point to, Brush color)
    {
        yield return new Line
        {
            X1 = from.X,
            Y1 = from.Y,
            X2 = to.X,
            Y2 = to.Y,
            Stroke = color,
            StrokeThickness = 2
        };

        const double headLength = 7;
        const double headWidth = 5;

        double dx = to.X - from.X;
        double dy = to.Y - from.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);
        double ux = dx / length;
        double uy = dy / length;
        double px = -uy;
        double py = ux;

        yield return new Polygon
        {
            Points = new PointCollection(
            [
                to,
                new Point(to.X - ux * headLength + px * headWidth / 2, to.Y - uy * headLength + py * headWidth / 2),
                new Point(to.X - ux * headLength - px * headWidth / 2, to.Y - uy * headLength - py * headWidth / 2)
            ]),
            Fill = color
        };
    }

    /// <summary>
    /// Creates a small, bold, coloured text label for an axis tip.
    /// </summary>
    private static TextBlock CreateAxisLabel(double centerX, double centerY, char text, Brush color)
    {
        var label = new TextBlock
        {
            Text = text.ToString(),
            Foreground = color,
            FontWeight = FontWeights.Bold,
            FontSize = 12
        };

        Canvas.SetLeft(label, centerX - 5);
        Canvas.SetTop(label, centerY - 8);
        return label;
    }
}
