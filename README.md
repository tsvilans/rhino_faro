# RhinoFaro
A [Rhino](https://www.rhino3d.com/) plug-in for importing [Faro](https://www.faro.com/) scan files.

Exactly what it says on the tin. Uses the Faro .NET SDK to allow importing Faro scan files into Rhino for viewing or processing.

Adds some commands:
- **RFLoadCloud**: Loads a Faro scan file. This is not added to the Rhino document, but is instead kept in its own buffer until it is 'baked'. For some reason baked pointclouds sometimes lose their color data. Keeping it in a separate buffer prevents this from happening.
- **RFClearCloud**: Clear the loaded scan from the buffer.
- **RFLoadSettings**: Load import settings from a file. Not optimal by any means, and subject to change.
- **RFScanSettings**: Set scanner settings. This will eventually allow you to run the scanner from here as well, but not yet.
- **RFTransformCloud**: Move the scan around.
- **RFBakeCloud**: Move the scan from the buffer to the Rhino doc, effectively baking it.

Same spiel: as-is, great if it works for you and if you find it helpful, feel free to suggest improvements.

# Contact
[tsvi@kadk.dk](mailto:tsvi@kadk.dk)

[http://tomsvilans.com](http://tomsvilans.com)

