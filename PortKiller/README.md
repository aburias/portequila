# 🍹 Portequila

**Take a shot. Kill the port!**

![Platform](https://img.shields.io/badge/Platform-Windows-blue) ![.NET](https://img.shields.io/badge/.NET-9.0-purple) ![License](https://img.shields.io/badge/License-MIT-green)

Tired of this?
```
Error: listen EADDRINUSE: address already in use :::3000
```

**Find it. Select it. Kill it.** 💀

## 📥 Download

**[Download Portequila](./releases/PortKiller-v1.0.0-win-x64.zip)** (Windows 10/11, ~54MB, no install needed)

## ✨ Features

- � Scan specific ports
- 📋 See all listening processes  
- ☑️ Multi-select & kill

## 🚀 Quick Start

1. Download `PortKiller.exe` from the releases folder
2. Run the executable (no installation required)
3. Type port numbers and press Enter to add them
4. Click "Scan Ports" or "All Listening"
5. Select processes and click "Kill Selected"

## 🛠️ Building from Source

### Requirements
- Windows 10/11
- .NET 9.0 SDK

### Build Commands

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/port-killer.git
cd port-killer

# Run in development
dotnet run

# Build release executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./releases
```

## 📸 Screenshot

```
🔌 Port Killer
┌─────────────────────────────────────────────────────┐
│ [Type port...]  [🔍 Scan Ports] [📋 All Listening] │
│                                                     │
│ [3000 ✕] [5000 ✕] [8080 ✕]                         │
│                                                     │
│ ☐ │ Port │ Process │ PID   │ Protocol │ State     │
│ ☑ │ 3000 │ node    │ 12456 │ TCP      │ LISTENING │
│ ☐ │ 5000 │ dotnet  │ 8932  │ TCP      │ LISTENING │
│                                                     │
│ ☐ Select All        [🔄 Refresh] [💀 Kill Selected]│
└─────────────────────────────────────────────────────┘
```

## 📝 Common Development Ports

| Port | Common Usage |
|------|--------------|
| 3000 | React, Express, Rails |
| 4200 | Angular |
| 5000, 5001 | .NET, Flask |
| 5173 | Vite |
| 8080 | Various servers |

## ⚠️ Note

You may need to run as **Administrator** to kill certain system processes.

## 📄 License

MIT License - feel free to use and modify!
