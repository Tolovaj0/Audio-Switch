AudioSwitch

A tiny Windows tray utility for switching between two audio output devices with a single click – speakers, headphones, monitors, soundbars and more.

<!-- Add a screenshot of the tray icon and settings window here --> <!-- ![AudioSwitch screenshot](docs/screenshot.png) -->
Features
Switch between two configured audio output devices
Configurable icon for each device
Tray icon with a number indicator showing the active device
Optional start with Windows
Remembers your devices and settings
Built with C# and Windows Forms
Download

Get the latest AudioSwitch.exe from the Releases page.

Windows x64 · Portable · No installer. It is self-contained, so you don't need to install .NET.

"Windows protected your PC" warning

The executable is not code-signed, so Windows SmartScreen may warn you the first time you run it:

Click More info.
Click Run anyway.

To verify the download, compare its checksum with the SHA-256 published on the release page:

powershell
Get-FileHash .\AudioSwitch.exe -Algorithm SHA256

Website: audio-switch.com

Prefer not to run a binary? Build it from source.

Usage

On first launch, select the two audio output devices you want to switch between.

Action	Result
Left-click the tray icon	Switch to the other device
Right-click → Switch device	Switch to the other device
Right-click → Settings	Change the configured devices and icons
Right-click → Start with Windows	Enable or disable automatic startup
Right-click → Exit	Close AudioSwitch
Requirements
Windows 10 or Windows 11 (x64)
Configuration

Selected device IDs and icon preferences are stored in your user application data folder (%APPDATA%\AudioSwitch). To reset the app, delete that folder.

Uninstall

There is no installer. Turn off Start with Windows, exit the app, then delete AudioSwitch.exe and the configuration folder above.

Building from source

Requires the .NET 8 SDK. Clone the repository, then:

powershell
dotnet restore
dotnet build
dotnet run

To produce the standalone executable:

powershell
dotnet publish -c Release -r win-x64 --self-contained true
Support

AudioSwitch is free. If it saves you some clicks, you can support development:

Buy Me a Coffee
PayPal
GitHub Sponsors
License

Released under the MIT License.
