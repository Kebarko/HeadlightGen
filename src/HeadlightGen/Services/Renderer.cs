using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KE.MSTS.HeadlightGen.Model;

namespace KE.MSTS.HeadlightGen.Services;

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
    /// <param name="pointsBoudingBox">The bounding box of the points to render in model coordinates.</param>
    /// <param name="points">The collection of points to render.</param>
    /// <returns>An enumerable collection of WPF Shape elements (Ellipse) ready to be added to the canvas.</returns>
    public static IEnumerable<Shape> Render(int canvasWidth, int canvasHeight, RectangleF pointsBoudingBox, IList<Point3D> points)
    {
        // Calculate scale to fit points within canvas
        float scaleX = canvasWidth / pointsBoudingBox.Width;
        float scaleY = canvasHeight / pointsBoudingBox.Height;

        // Use the smaller scale to maintain aspect ratio
        float scale = Math.Min(scaleX, scaleY);

        // Calculate scaled dimensions of points bounding box
        float scaledWidth  = pointsBoudingBox.Width  * scale;
        float scaledHeight = pointsBoudingBox.Height * scale;

        // Calculate offsets to center the drawing
        float offsetX = (canvasWidth  - scaledWidth)  / 2;
        float offsetY = (canvasHeight - scaledHeight) / 2;

        // Transform points to canvas coordinates and flip Y axis
        float minX = pointsBoudingBox.X;
        float minY = pointsBoudingBox.Y;
        var scaledPoints = points.Select(p => new PointF
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
}
