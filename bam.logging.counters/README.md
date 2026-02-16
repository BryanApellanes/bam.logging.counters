# bam.logging.counters

Lightweight in-memory counters and timers for application instrumentation and statistics.

## Overview

`bam.logging.counters` provides simple, thread-safe instrumentation primitives for counting events and timing operations within BAM applications. The `Stats` class serves as the central static API, maintaining a `ConcurrentDictionary` of named `Counter` and `Timer` instances. Callers use static methods like `Stats.Increment("requests")`, `Stats.Start("operation")`, and `Stats.End("operation")` to track metrics without any configuration or setup.

The `Counter` class supports increment, decrement, diff (comparison between counters), and a pluggable `CountReader` delegate for custom count sources. The `Timer` class measures elapsed time in milliseconds between `Start()` and `End()` calls, with a `SoFar()` method for in-progress measurement. A static convenience method `Timer.Time(action)` wraps an `Action` and returns its duration.

The library also includes persistable data classes (`CounterData`, `TimerData`) that extend the BAM repository model with `CompositeKeyAuditRepoData`, enabling counter and timer values to be stored per user in a database. These data classes are intended for long-term persistence of instrumentation data.

## Key Classes

| Class | Description |
|---|---|
| `Stats` | Static API for managing named counters and timers via a `ConcurrentDictionary`. Provides `Start`, `End`, `Increment`, `Decrement`, `Count`, and `Diff` methods. |
| `Counter` | Named counter with `Increment()`, `Decrement()`, and `Diff(counter)` operations. Supports a custom `CountReader` delegate for external count sources. |
| `Timer` | Named timer that measures elapsed milliseconds between `Start()` and `End()`. Provides `SoFar()` for in-progress timing and a static `Time(action)` convenience method. |
| `CounterData` | Persistable counter record with composite key of `UserName` + `CounterName`, extending `CompositeKeyAuditRepoData`. |
| `TimerData` | Persistable timer record with `UserName`, `Name`, and `Value` fields, extending `CompositeKeyAuditRepoData`. |

## Dependencies

### Project References
- `bam.base` -- `Log`, `Instant`, `RandomExtensions`, core types
- `bam.data.repositories` -- `CompositeKeyAuditRepoData`, `[CompositeKey]` attribute
- `bam.data` -- data extensions

### Package References
- None

## Target Framework
- `net10.0`

## Usage Examples

### Counting events
```csharp
using Bam.Logging.Counters;

// Increment a named counter
Stats.Increment("http_requests");
Stats.Increment("http_requests");

// Read the current count
Counter counter = Stats.Count("http_requests");
ulong count = counter.Count; // 2

// Decrement
Stats.Decrement("http_requests");
```

### Timing an operation
```csharp
using Bam.Logging.Counters;

// Static convenience: time an action and get milliseconds
int ms = Timer.Time("db_query", () =>
{
    // Perform database query
    Thread.Sleep(100);
});
// ms ~= 100

// Manual start/end
Timer timer = Stats.Start("processing");
// ... do work ...
int duration = Stats.End("processing").Duration;
```

### Using SoFar for in-progress measurement
```csharp
Timer timer = Timer.Start("long_operation");
// ... partial work ...
int elapsed = timer.SoFar(); // milliseconds so far
// ... more work ...
int total = timer.End(); // total milliseconds
```

### Comparing counters
```csharp
Counter before = Stats.Count("items", 100);
// ... process items ...
Counter after = Stats.Count("after_items", 85);
Counter diff = before.Diff(after); // diff.Count == 15
```

### Custom count reader
```csharp
Counter queueCounter = Stats.Count("queue_depth", () => (ulong)myQueue.Count);
ulong depth = queueCounter.Count; // reads from myQueue.Count dynamically
```

## Known Gaps / Not Yet Implemented

- **Counter is not thread-safe for Increment/Decrement**: The `_count` field uses `++`/`--` operators rather than `Interlocked.Increment`/`Decrement`, which could produce incorrect counts under high concurrency.
- **Timer.Value setter has a bug**: The `set` accessor references `Value` instead of `value` (`Duration = (int)Value;`), causing a self-referencing assignment rather than using the incoming value.
- **CounterData and TimerData are defined but not wired**: No repository or service class exists in this library to persist counter/timer data; these are data models awaiting integration.
