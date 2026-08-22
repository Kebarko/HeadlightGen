using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;
using KE.MSTS.HeadlightGen.Model;
using Brushes = System.Windows.Media.Brushes;

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
    /// <param name="pointsBoudingBox">The bounding box of the points to render in model coordinates. Ignored for views different from Front and Back.</param>
    /// <param name="points">The collection of points to render.</param>
    /// <param name="view">The orthographic view to render.</param>
    /// <returns>An enumerable collection of WPF Shape elements (Ellipse) ready to be added to the canvas.</returns>
    public static IEnumerable<Shape> Render(int canvasWidth, int canvasHeight, RectangleF pointsBoudingBox, IList<Point3D> points, RenderView view)
    {
        // Handle empty points collection
        if (points.Count == 0)
            yield break;
        
        // Project points to 2D based on the selected view
        var projectedPoints = points.Select(point => Project(point, view)).ToList();

        // For views other than Front and Back, calculate the bounding box of the projected points
        if (view != RenderView.Front && view != RenderView.Back)
            pointsBoudingBox = GetBoundingBox(projectedPoints);

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
}
