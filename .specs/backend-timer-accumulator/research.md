# Research & Design Decisions

---

## Summary
- **Feature**: `backend-timer-accumulator`
- **Discovery Scope**: Extension of existing time-tracking and SCADA real-time polling logic.
- **Key Findings**:
  - WebApp communicates with the ATSCADA OPC service via WCF Net.Tcp (`net.tcp://localhost:8010/ATSCADAService`) wrapped in `RealtimeService.cs`.
  - Browser-side delta calculation fails because of packet loss, tab sleeping/background throttling, and database polling synchronization race conditions.
  - Using the state of `is_paused` (from database) and `STOP` tag (from PLC) provides a clean, state-driven transition event to reset/increment timers safely, completely preventing the "missing reset event" race condition.

---

## Research Log

### PLC Tag values and State transition checks
- **Context**: How to correctly identify when to calculate delta vs. when a reset occurs.
- **Sources Consulted**: PLC tag lists, `OverviewRealtime.js` implementation, and developer feedback on pause/resume states.
- **Findings**:
  - `is_paused` is set to `1` in the database when the batch is paused.
  - The `STOP` tag in the PLC transitions to active when the batch stops.
  - When the batch is paused (`is_paused = 1`), the timer resets to `0` or does not increase.
  - Checking the transition `is_paused = 1 -> 0` is the most reliable way to handle the timer baseline reset, since this transition is driven by user commands and logged persistently.

---

## Architecture Pattern Evaluation

| Option | Description | Strengths | Risks / Limitations | Notes |
|--------|-------------|-----------|---------------------|-------|
| Background Thread in WebApp | Static thread spawned at Application_Start in Global.asax | Easy to implement inside the same project, runs constantly while WebApp is active | Thread might stop if IIS AppPool recycles or goes idle | Can be mitigated by setting AppPool start mode to AlwaysRunning |
| Separate Console Executable | Run calculation inside Connect.exe or a new Windows Service | Completely independent of IIS lifecycle, runs 24/7 | Requires installing and maintaining a separate service/executable | Good if WebApp is hosted in a volatile environment |

---

## Design Decisions

### Decision: Spawning Background Thread in WebApp
- **Context**: Need a simple, self-contained service that does not require installing separate Windows Services.
- **Alternatives Considered**:
  1. Add background loop to `Connect.exe` — rejected because `Connect` is not configured or active in this project repository yet.
  2. Implement in WebApp using a static thread — selected.
- **Selected Approach**: Spawning a custom background thread inside `Global.asax.cs`'s `Application_Start()`.
- **Rationale**: Keeps the codebase unified and allows direct access to the database connector and existing WCF `RealtimeService`.

---

## Risks & Mitigations
- **IIS App Pool Recycles** — Mitigation: The Background Service is stateless, it loads the current accumulated times from the database upon startup, so no data is lost during app pool recycles.
- **WCF Channel Faults** — Mitigation: Wrapped `RealtimeService.Instance.Read` with automatic channel recreation (using the existing `HandleException` logic).
