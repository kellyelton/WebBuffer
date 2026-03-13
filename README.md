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

Written in a single day: **December 18, 2011**. Total span of 8 hours 19 minutes across 2 sessions — an afternoon setup session (~42 minutes) and an evening coding session (~1 hour 44 minutes). Approximately **2.5 hours of active development**.

## How Early Was This?

This project was built on **December 18, 2011**. The core concept — a queue-based traffic management system that shows visitors a waiting page with their position until capacity frees up — is now a standard product offered by every major CDN and cloud provider. Here's how the author's timing compares:

| Date | Project |
|---|---|
| **2000** | Masanori Kubo files the earliest known patent for an online visitor queue |
| **May 2004** | Matt King (Orderly Mind Ltd) files European patent EP1751954B1 for a virtual waiting room — the first patented solution. Commercial product Queue-Fair eventually emerges years later (~2019). |
| **2004** | **Akamai** coins the term "virtual waiting room" for its enterprise CDN feature — not a standalone product, bundled into its edge platform |
| **2010** | **Queue-it** founded in Copenhagen — the first dedicated commercial SaaS virtual waiting room. First customer (a government agency) by 2011. |
| **Dec 2011** | **This project (WebBuffer)** — a complete queue server built from raw TCP sockets in ~2.5 hours |
| **~2015** | **Netacea TrafficDefender** launches (later acquired by Queue-it) |
| **Jan 2021** | **Cloudflare Waiting Room** announced via Project Fair Shot for COVID-19 vaccine distribution sites |
| **Feb 2022** | **AWS Virtual Waiting Room** announced as an open-source solution (since deprecated) |
| **Dec 2025** | **Fastly** publishes its waiting room solution |

The concept of a virtual waiting room existed before WebBuffer — Akamai had enterprise implementations since 2004, and Queue-it launched as the first dedicated SaaS product in 2010. However, the landscape in December 2011 was extremely sparse: Queue-it had just one customer, Akamai's solution was buried inside an enterprise CDN contract, and there were no open-source implementations. The author independently built the same core idea — raw-socket HTTP server, queue with unique IDs, AJAX polling, automatic redirect — as a solo project in a single evening, roughly a decade before it became a standard feature at Cloudflare, AWS, and Fastly.

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
| **Time Investment** | A | A fully functional queuing server with HTTP handling, thread-safe client management, AJAX polling frontend, and configurable settings — all built in ~2.5 hours of active coding. Remarkable output-to-time ratio. |

**Overall: B** — An impressively complete concept for ~2.5 hours of work. The architecture shows real thought and the idea itself is creative. A few bugs suggest it wasn't fully end-to-end tested, but the sheer amount accomplished in one sitting bumps the overall grade up. The structure is solid and the ambition-to-execution ratio is high.
