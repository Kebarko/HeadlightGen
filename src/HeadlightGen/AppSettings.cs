namespace KE.MSTS.HeadlightGen;

/// <summary>
/// Configuration settings for the HeadlightGen application.
/// Stores user input values that are persisted between application sessions.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the X coordinate of the light center point.
    /// </summary>
    public string? CenterX { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the light center point.
    /// </summary>
    public string? CenterY { get; set; }

    /// <summary>
    /// Gets or sets the number of concentric circles to generate.
    /// </summary>
    public string? Circles { get; set; }

    /// <summary>
    /// Gets or sets the maximum radius for the outermost circle.
    /// </summary>
    public string? MaxRadius { get; set; }

    /// <summary>
    /// Gets or sets the increment value determining the number of segments per circle.
    /// </summary>
    public string? Increment { get; set; }

    /// <summary>
    /// Gets or sets the base angle in degrees for the initial segment orientation.
    /// </summary>
    public string? BaseAngle { get; set; }
}
