using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KE.MSTS.HeadlightGen;

/// <summary>
/// Configuration settings for the HeadlightGen application.
/// Stores user input values that are persisted between application sessions.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// The file path to the application settings JSON file.
    /// </summary>
    [JsonIgnore]
    private static readonly string settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    /// <summary>
    /// Gets or sets the X coordinate of the light center point.
    /// </summary>
    public string? CenterX { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the light center point.
    /// </summary>
    public string? CenterY { get; set; }

    /// <summary>
    /// Gets or sets the Z coordinate of the light center point.
    /// </summary>
    public string? CenterZ { get; set; }

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

    /// <summary>
    /// Serializes the current <see cref="AppSettings"/> instance to JSON (pretty-printed) and writes it to the file specified by <c>settingsPath</c>.
    /// </summary>
    public void Save()
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(settingsPath, json, Encoding.UTF8);
    }

    /// <summary>
    /// Loads an <see cref="AppSettings"/> instance from a JSON file at the specified path.
    /// </summary>
    /// <returns>The deserialized <see cref="AppSettings"/> instance, or <c>null</c> if the file does not exist or if deserialization fails.</returns>
    public static AppSettings? Load()
    {
        if (!File.Exists(settingsPath))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(settingsPath, Encoding.UTF8);

            var settings = JsonSerializer.Deserialize<AppSettings>(json);
            return settings;
        }
        catch
        {
            // Ignore errors
            return null;
        }
    }
}
