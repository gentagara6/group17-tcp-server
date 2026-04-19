# Group 17 - TCP Server
**Protokolli:** TCP | **Gjuha:** C# | **Viti akademik:** 2025-26

## Anëtarët dhe Detyrat
| Anëtari | Detyrë |
|---------|--------|
| Altin Mulaj    | Server.cs, ClientHandler.cs |
| Genti Kafexhiu | Client.cs, CommandHandler.cs |
| Genta Gara     | FileManager.cs |
| Gerti Parduzi  | HttpStatsServer.cs |

## Struktura e Projektit
```
group17-tcp-server/
├── Server/
│   ├── Server.cs           - TCP server kryesor
│   ├── ClientHandler.cs    - Menaxhon çdo klient
│   ├── FileManager.cs      - Menaxhon file-at
│   └── HttpStatsServer.cs  - HTTP server për statistika
├── Client/
│   ├── Client.cs           - Klienti TCP
│   └── CommandHandler.cs   - Menaxhon komandat
├── server_files/
│   └── test.txt
└── README.md
```

---

## Instalimi

### Ubuntu/Linux
```bash
sudo apt update
sudo apt install -y dotnet-sdk-8.0
dotnet --version
```

### Windows
1. Shko tek: https://dotnet.microsoft.com/download
2. Shkarko **.NET 8.0 SDK**
3. Instalo dhe hap **Command Prompt** ose **PowerShell**
4. Verifiko instalimin: `dotnet --version`

---

## Si Ekzekutohet

### Ubuntu/Linux

**Hapi 1 - Klono projektin:**
```bash
git clone https://github.com/gentagara6/group17-tcp-server.git
cd group17-tcp-server
```

**Hapi 2 - Ekzekuto Serverin (Terminal 1):**
```bash
cd Server
dotnet run
```

**Hapi 3 - Ekzekuto Klientin (Terminal 2 i ri):**
```bash
cd Client
dotnet run
```

### Windows

**Hapi 1 - Klono projektin:**
```cmd
git clone https://github.com/gentagara6/group17-tcp-server.git
cd group17-tcp-server
```

**Hapi 2 - Ekzekuto Serverin (CMD ose PowerShell 1):**
```cmd
cd Server
dotnet run
```

**Hapi 3 - Ekzekuto Klientin (CMD ose PowerShell 2 i ri):**
```cmd
cd Client
dotnet run
```

---

## Lidhja në Rrjetë Reale (4 pajisje)

Kur lidheni nga pajisje të ndryshme në të njëjtin WiFi/LAN:

**1. Gjej IP-në e kompjuterit ku është serveri:**

Ubuntu/Linux:
```bash
ip a
```
Windows:
```cmd
ipconfig
```
Shiko `inet` (Ubuntu) ose `IPv4 Address` (Windows) — p.sh. `192.168.1.5`

**2. Starto serverin në kompjuterin kryesor:**
```bash
cd Server
dotnet run
```

**3. Klientët e tjerë lidhen duke shkruar IP-në e serverit:**
```
Shkruaj IP-ne e serverit: 192.168.1.5
```

---

## Statistikat (Browser)

Nga kompjuteri i serverit:
```
http://localhost:8080/stats
```
Nga pajisje tjetër në rrjet:
```
http://192.168.1.5:8080/stats
```

---

## Kredencialet
| Username | Password | Roli |
|----------|----------|------|
| admin | admin123 | Admin - qasje e plotë |
| user1 | user123 | Read-only |
| user2 | user123 | Read-only |
| user3 | user123 | Read-only |

---

## Komandat e Disponueshme

### Admin (qasje e plotë)
| Komanda | Përshkrimi |
|---------|-----------|
| /list | Liston të gjitha file-at |
| /read filename.txt | Lexon përmbajtjen e file-it |
| /upload filename.txt\|permbajtja | Ngarkon file në server |
| /download filename.txt | Shkarkon file nga serveri |
| /delete filename.txt | Fshin file nga serveri |
| /search fjala | Kërkon file sipas fjalës kyçe |
| /info filename.txt | Tregon madhësinë dhe datën |
| /exit | Shkëput nga serveri |

### Read-only
| Komanda | Përshkrimi |
|---------|-----------|
| /list | Liston të gjitha file-at |
| /read filename.txt | Lexon përmbajtjen e file-it |
| /search fjala | Kërkon file sipas fjalës kyçe |
| /info filename.txt | Tregon madhësinë dhe datën |
| /exit | Shkëput nga serveri |

---

## Karakteristikat Teknike
- **Protokolli:** TCP
- **Porta kryesore:** 5000
- **Porta HTTP stats:** 8080
- **Max klientë:** 10
- **Timeout:** 30 sekonda
- **Threading:** Çdo klient trajtohet në thread të veçantë
