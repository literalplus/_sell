_Sell
=====
This is a simple point-of-sale application created in something like 2012. Everything is configured in the code, and
it only works on Windows. The code is basically a big pile of hacks that somehow function, but while ignoring all C#
code style guidelines.

Screenshot
==========
![Screenshot of the application main window](https://github.com/xxyy/_sell/raw/master/screenshot.png)

Building
========

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (v10.0.101 or later)
- Windows OS (WPF application) .. sorry!

## Build with .NET CLI
```bash
# Restore dependencies
dotnet restore

# Build (Debug)
dotnet build

# Build (Release)
dotnet build -c Release

# Run
dotnet run --project _Sell
```

## Build with IDE
Use either **Visual Studio 2022** (17.12+) or **JetBrains Rider** (2024.3+) - they will automatically detect the .NET 10 SDK and configure everything for you.

## Publish for Distribution
```bash
# Self-contained single executable (recommended)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Framework-dependent (requires .NET 10 runtime on target machine)
dotnet publish -c Release -r win-x64
```

Output will be in `_Sell\bin\Release\net10.0-windows\win-x64\publish\`

License
=======
MIT License: https://choosealicense.com/licenses/mit/ (See the `LICENSE.txt` file for details)
