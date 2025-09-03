# CS2 Stretched Launcher

A simple Windows launcher for Counter-Strike 2 that:
- Sets your desktop resolution to **1280×960** before launch
- Launches CS2 via Steam
- Waits for the game to close
- Restores your desktop resolution to **1920×1080** afterwards

Written in C# using Win32 APIs for reliable display mode changes (QRes-style behavior).

---

## 📦 How it Works
This program directly calls Windows display APIs to:
1. Target the **primary display device**
2. Change to the desired resolution (committed to registry)
3. Launch `steam://rungameid/730`
4. Wait for `cs2.exe` to exit
5. Restore your original resolution

---

## 🖥️ Requirements
- Windows 10/11 (64-bit)
- Steam installed
- .NET 9.0 Runtime (only if you don't use the -standalone file.)

---

## 🚀 Running the Prebuilt EXE
If you just want to use it:
1. Download the latest **`CS2StretchedLauncher.exe`** from [Releases](https://github.com/huh445/cs2-stretched-launcher/releases)
2. Place it anywhere (e.g., Desktop)
3. Double-click to launch CS2 with stretched res

Note: If you don't leave the .json with it, it will default to 1280x960 and 1920x1080

---

## 🛠️ Building from Source

### Clone the repository
```bash
git clone https://github.com/huh445/cs2-stretched-launcher.git
cd cs2-stretched-launcher
```

### Build (Release)
```bash
dotnet build -c Release
```

### Publish (Portable EXE)
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --no-self-contained
```
Output will be in:
```
bin/Release/net9.0/win-x64/publish/CS2StretchedLauncher.exe
```

### Publish (Self-contained EXE)
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true
```
This version runs on systems without .NET installed (larger file size).

---

## ⚠️ Notes
- The program commits resolution changes to the registry so Windows/DWM treats them as the actual desktop resolution.
