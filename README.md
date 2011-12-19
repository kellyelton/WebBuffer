# WebBuffer

A web server traffic queuing system built from raw sockets. When more users connect than the configured limit, excess visitors are shown a "waiting room" page that polls via AJAX to check their place in the queue. Once they reach the front, they're automatically redirected to the real website — like a hand-rolled version of Cloudflare's waiting room feature.

## How It Works

1. **Server** listens on a TCP socket and accepts HTTP connections
2. If the site is under capacity (`maxusers`), new visitors are immediately redirected to the target site (`www.skylabsonline.com`)
3. If over capacity, visitors receive a waiting page with a unique queue ID
4. The waiting page uses jQuery AJAX to poll `/place?{id}` every second for their queue position
5. A background thread (`PositionChecker`) automatically expires stale clients based on a configurable timeout
6. When a visitor reaches position 0, the next poll redirects them to the real site

## Configuration

`settings.ini`:
```
webserverport:8080
maxusers:10
usertimeout:30
```

## Tech Stack

- **Framework**: .NET 4.0 Client Profile
- **Language**: C# 4.0
- **HTTP**: Raw `System.Net.Sockets.Socket` (no HttpListener or web framework)
- **Frontend**: HTML 4 + jQuery 1.7.1 for AJAX polling
- **IDE**: Visual Studio 2010

## Development Timeline

Written in a single day: **December 18, 2011**.

| Time | File | Activity |
|------|------|----------|
| 12:47 PM | AssemblyInfo.cs | Project created |
| 1:16 PM | settings.ini | Configuration file |
| 1:26 PM | WebBuffer.sln | Solution setup |
| 1:29 PM | Settings.cs | Settings parser |

*~6 hour break*

| Time | File | Activity |
|------|------|----------|
| 7:22 PM | settings.ini | Resumed work |
| 7:31 PM | WebBuffer.csproj | Project file updated |
| 8:20 PM | Program.cs | Main entry point |
| 8:39 PM | Client.cs | Queue client/manager |
| 9:04 PM | wait.html | Waiting room page |
| 9:06 PM | Server.cs | HTTP server (last edit) |

**Total span**: 8 hours 19 minutes across 2 sessions — an afternoon setup session (~42 minutes) and an evening coding session (~1 hour 44 minutes). Approximately **2.5 hours of active development**.

## AI Code Review

*Code review performed by AI (Claude) and graded relative to the era the code was written in (.NET 4.0, 2011).*

| Criterion | Grade | Notes |
|-----------|-------|-------|
| **Completeness** | B- | All pieces present: server, queue manager, settings, wait page with AJAX polling. The settings file defines `webserverport:8080` but the server hardcodes port 80. |
| **Functionality** | C | Would mostly work but has bugs: hardcoded port 80, inverted `AddToQ` return logic, potential socket ownership conflict in `GetHeaders`/`WriteToBrowser`, `IComparer` implemented but never used for sorting. |
| **Patterns & Practices** | C+ | Raw sockets for HTTP were a reasonable learning exercise. Thread-based concurrency with `Thread.Sleep` polling was standard pre-async/await. Lock-based thread safety shows awareness, though not fully atomic. The `goto` in Program.cs is functional but ugly. |
| **Code Quality** | B- | Clean file separation. Readable code with clear intent. Good variable naming (`Running`, `Clients`, `ConnectTime`). jQuery AJAX polling page is well-structured. Settings parser handles `#` comments. |
| **Architecture** | B | Good separation into logical classes (Server, ClientsBox, Settings, Program). Static classes appropriate for this scale. HTML template with `%%id%%` replacement is a clean approach. |
| **Ambition** | A- | Building a web traffic queuing system from raw sockets is genuinely ambitious and creative for a learning project. Shows understanding of HTTP protocol, socket programming, threading, and client-server architecture. |

**Overall: B-** — An impressively complete concept for a single afternoon/evening of work. The architecture shows real thought and the idea itself is creative. A few bugs suggest it wasn't fully end-to-end tested, but the structure is solid.
