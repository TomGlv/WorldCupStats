# WorldCupStats

A .NET solution displaying statistics from the **2018 FIFA Men's World Cup** and the **2019 FIFA Women's World Cup**, built as a university project for Object-Oriented Programming in .NET.

---

## Solution Structure

```
WorldCupStats.sln
├── DataLayer/                  # Class Library — shared data layer
├── WorldCupStats.WinForms/     # Windows Forms application
└── WorldCupStats.WPF/          # WPF application
```

---

## Requirements

- **Visual Studio 2022** (Community or higher)
- **.NET 8.0 SDK** (all three projects target `net8.0` or `net8.0-windows`)
- **NuGet package:** `Newtonsoft.Json 13.0.3`
- Internet connection (for API mode) **or** local JSON files (for offline mode)

---

## Projects

### DataLayer (Class Library)
Shared between both client applications. Responsible for:
- Fetching data from the REST API or local JSON files
- Parsing and mapping JSON to C# models (`Team`, `Match`, `Player`, `MatchEvent`)
- Reading and writing settings, favorite team, and favorite players to text files
- Switching between **API mode** and **JSON file mode** via `AppConfig`

**API Endpoints used:**
| Endpoint | Description |
|---|---|
| `/men/teams/results` | Men's team list |
| `/women/teams/results` | Women's team list |
| `/men/matches` | All men's matches |
| `/women/matches` | All women's matches |
| `/men/matches/country?fifa_code=XXX` | Men's matches for a specific team |
| `/women/matches/country?fifa_code=XXX` | Women's matches for a specific team |

Base URL: `https://worldcup-vua.nullbit.hr`

---

### WorldCupStats.WinForms
Windows Forms desktop application covering learning outcomes **I1, I2, I3** (60 points).

**Features:**
- **Initial settings form** — choose championship (Men/Women) and language (English/Croatian), saved to file
- **Favorite team** — select from ComboBox in `NAME (FIFA_CODE)` format, persisted across sessions
- **Favorite players** — choose up to 3 favorite players; supports drag & drop and context menu between two panels
- **Player user control** — displays name, shirt number, position, captain badge, favorite star ⭐, and player photo
- **Player photos** — set a custom image per player via file dialog; falls back to a default image
- **Rankings** — player ranking by goals/yellow cards and match ranking by attendance
- **Print / PDF export** — print rankings via the system print dialog
- **Settings form** — change championship and language at any time; Enter to confirm, Esc to cancel
- **Exit confirmation** — prompts the user before closing

**Saved files (in application directory):**
| File | Contents |
|---|---|
| `settings.txt` | Language and championship preference |
| `favorite_team.txt` | FIFA code of the selected team |
| `favorite_players.txt` | Names of favorite players |

---

### WorldCupStats.WPF
WPF desktop application covering learning outcomes **I4, I5** (40 points). Fully responsive layout.

**Features:**
- **Initial settings window** — championship, language, and window size (Fullscreen / 1280×720 / 1024×600 / 800×500)
- **Shared settings** — reads and writes the same settings file as the WinForms app
- **Team overview** — two ComboBoxes (home/away team), displays match score between selected teams
- **Team info window** — opens with a 0.5s fade-in animation; shows name, FIFA code, games/wins/losses/draws, goals
- **Starting lineup display** — football pitch view with player controls placed by position (Goalie, Defender, Midfield, Forward)
- **Player info window** — opens with a 0.3s scale animation; shows name, number, photo, position, captain status
- **Settings window** — change preferences at runtime; Enter to confirm, Esc to cancel
- **Exit confirmation** — prompts the user before closing

---

## Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/your-username/WorldCupStats.git
cd WorldCupStats
```

### 2. Open in Visual Studio
Open `WorldCupStats.sln` in **Visual Studio 2022**.

### 3. Restore NuGet packages
Visual Studio will restore packages automatically, or run:
```
Tools → NuGet Package Manager → Restore NuGet Packages
```

### 4. Set startup project
- Right-click **WorldCupStats.WinForms** → **Set as Startup Project** to run the WinForms app
- Right-click **WorldCupStats.WPF** → **Set as Startup Project** to run the WPF app

### 5. Run
Press **F5** or click **Start**.

---

## Data Mode Configuration

In `DataLayer/Config/AppConfig.cs`:

```csharp
// true  = fetch data from the live API
// false = read from local JSON files
public static bool UseApi { get; set; } = true;
```

To use local JSON files, set `UseApi = false` and place the downloaded `.json` files in:
```
<OutputDirectory>/JsonData/
```

Files expected:
- `men_teams.json`
- `women_teams.json`
- `men_matches.json`
- `women_matches.json`

---

## Project Notes

- All file paths are **relative** — no hard-coded absolute paths
- All API calls are **asynchronous** (`async/await`) with a loading indicator
- All exceptions are caught and displayed as **dialog boxes** — the app will never crash on an unhandled error
- Player images and settings files are **shared** between WinForms and WPF

---

## Tech Stack

| Technology | Usage |
|---|---|
| C# / .NET 8 | All projects |
| Windows Forms | WinForms client app |
| WPF + XAML | WPF client app |
| Newtonsoft.Json | JSON parsing |
| HttpClient | REST API calls |
| System.IO | File persistence |

---

## License

This project was created for educational purposes at **Algebra University College**.
