# HeadlightGen

## Overview

HeadlightGen is a Windows desktop application for creating Microsoft Train Simulator (MSTS) headlight light-definition files. It calculates points for concentric light circles, displays a preview of the generated layout, and exports the result using an `.inc` template.

The application is also suitable for Open Rails and other simulators that use the MSTS light-definition format.

## Requirements

`.NET 10`

## Installation

Unpack `HeadlightGen.exe`.

## Configuration

The values in the window are saved automatically to `appsettings.json` beside the application when you close it. They are restored the next time you start the application. You normally do not need to edit this file manually.

## Usage

The main window has an input panel on the left, a preview on the right, and a **Generate** button at the bottom of the input panel.

### Input panel

- **Title**: Optional text added as a comment at the beginning of the generated file.
- **Center X**, **Center Y**, and **Center Z**: Coordinates of the centre point, in metres.
- **Number of circles**: Number of concentric circles to create.
- **Max. radius**: Radius of the outermost circle, in centimetres.
- **Increment**: Number of segments in the first circle. Each following circle uses the increment multiplied by its circle number.
- **Rotation**: Starting angle for the first segment of each circle, in degrees.
- **Elevation**: Elevation angle applied to all generated points, in degrees.
- **Total segments**: Read-only count of the points currently generated, including the centre point.

As you enter valid values, the preview is updated automatically. It shows the calculated light points and lets you check the shape and orientation before creating a file. The **Generate** button is available when all required values have been entered.

### Creating a light file

1. Fill in the fields in the input panel.
2. Check the preview and the **Total segments** value.
3. Select **Generate**.
4. In the **Select Template File** dialog, choose an `.inc` template.
5. In the **Select Output File** dialog, choose the name and location of the generated `.inc` file.

The template must contain the following placeholders:

- `{X}`: generated X coordinate
- `{Y}`: generated Y coordinate
- `{Z}`: generated Z coordinate
- `{E}`: elevation value

HeadlightGen writes one copy of the template for every generated point and replaces the placeholders with the calculated values. The generated file can then be copied to the appropriate location in your MSTS or Open Rails installation.

## License

This project is licensed under the MIT License.
