# 🍹 Portequila

**Take a shot. Kill the port!**

![Platform](https://img.shields.io/badge/Platform-Windows-blue) ![.NET](https://img.shields.io/badge/.NET-9.0-purple) ![License](https://img.shields.io/badge/License-MIT-green)

Tired of this?
```
Error: listen EADDRINUSE: address already in use :::3000
```

**Find it. Select it. Kill it.** 💀

## 📥 Download

**[Download Portequila](./PortKiller/releases/Portequila-v1.0.0-win-x64.zip)** (Windows 10/11, ~54MB, no install needed)

## ✨ Features

- 🎯 Scan specific ports
- 📋 See all listening processes  
- ☑️ Multi-select & kill

## 🚀 Quick Start

1. Download and extract the zip
2. Run `Portequila.exe` (no installation required)
3. Type port numbers and press Enter to add them
4. Click "Scan Ports" or "All Listening"
5. Select processes and click "Kill Selected"

## 🛠️ Building from Source

```bash
cd PortKiller
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./releases
```

## ⚠️ Note

Run as **Administrator** to kill certain system processes.

## 📄 License

MIT License
