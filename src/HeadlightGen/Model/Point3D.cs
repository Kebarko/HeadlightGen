namespace KE.MSTS.HeadlightGen.Model;

/// <summary>
/// Represents a point in three-dimensional space with X, Y, and Z coordinates.
/// </summary>
/// <param name="X">The X-coordinate of the point.</param>
/// <param name="Y">The Y-coordinate of the point.</param>
/// <param name="Z">The Z-coordinate of the point.</param>
public record Point3D(float X, float Y, float Z);
