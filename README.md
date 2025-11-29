
# 🚀 Elevator System (C# / .NET)

> A realistic but beginner-friendly elevator simulation with clean architecture, background processing, and easy extensibility.  
> **Made for junior developers, students, and anyone curious about how real systems are designed.**

---

# 🧭 1. What is this project?

This is a **console application** that simulates a **building elevator**, designed to teach:

- Clean architecture basics
- Background workers & concurrency
- Layered folder structure
- Logging without interrupting the user
- Object-oriented modeling

You can:

- Request the elevator from a floor
- Choose a destination
- Watch the elevator move through logs
- Type commands while the elevator works in the background
- Read clean, easy-to-understand code

---

# 🧑‍🏫 2. Who is this for?

### 👶 Junior developers
Learn how real-world systems are structured:
- What layers are
- Clean, readable C#
- How to separate logic from infrastructure
- Safe multi-threading patterns

### 🧓 Non-technical readers
Understand:
- How software models the real world
- Why architecture matters
- How a simple elevator can teach system design

---

# ⚙️ 3. Main Features

✨ **Single elevator (floors 1–10)**  
✨ **Pickup requests** (`r 5 up`)  
✨ **Destination requests** (`d 8`)  
✨ **FIFO scheduling** (first request served first)  
✨ **Background worker** moves the elevator  
✨ **Thread-safe logging** that doesn’t break the input line  
✨ **Stuck elevator detection**  
✨ **Clean, extensible architecture**

---

# 🏗️ 4. How the system works — in simple terms

Imagine the system is a small company:

- 🛗 **Elevator** → the worker
- 🧑‍💼 **ElevatorController** → the manager
- 📬 **Request Queue** → the inbox
- ✍️ **Logger** → the note-taker

### The flow:

1. You type a command (`r 3 up`).
2. The system creates a **pickup request object**.
3. The request goes into a **queue**.
4. A background worker:
    - Looks at the first request
    - Moves the elevator floor by floor
    - Logs what happens
5. Elevator arrives, opens/closes doors
6. If there is a destination → repeat steps

---

# 🧱 5. Architecture (clean, beginner-friendly)

```
/src
  /domain           ← core business logic (what the system *is*)
  /application      ← use cases (what the system *does*)
  /infrastructure   ← technical details (how it *works*)
  /console          ← user interface
/tests              ← automated tests
/docs               ← diagrams & documentation
README.md
```

## 🧩 5.1. Domain Layer — "What the system IS"

Includes:

- `Elevator`
- `ElevatorRequest`
- `Floor`
- `ElevatorState`, `Direction`
- Interfaces: `ILogger`, `IRequestQueue`, `ITimeProvider`

This layer has **zero dependencies**.  
No console, no threads, no files.  
Totally pure logic → **very testable**.

---

## 🚦 5.2. Application Layer — "What the system DOES"

Contains **use cases** like:

- `RequestElevatorUseCase`

This layer coordinates actions but **does not know** about the console or threads.

---

## 🏭 5.3. Infrastructure Layer — "HOW it works"

Contains:

- `ElevatorController` (background worker)
- `FifoRequestQueue`
- `ConsoleLogger`
- `SystemTimeProvider`

The controller:
- Runs a loop
- Processes requests
- Moves the elevator
- Opens/closes doors
- Ensures thread safety
- Detects stuck elevators

---

## 🖥️ 5.4. Console Layer — The user interface

- Reads input
- Shows logs
- Uses the improved logger
- Prevents logs from interrupting user typing

---

# ▶️ 6. Running the project

### Requirements
- .NET 6 or .NET 8
- Terminal or PowerShell

### Commands

```bash
cd src/console
dotnet run
```

You will see:

```
r <floor> <up|down>   request pickup
d <floor>             choose destination
q                     quit
```

---

# 🎮 7. Example Commands

### Request elevator to floor 3 going up:
```
r 3 up
```

### Choose destination floor 8:
```
d 8
```

### Quit:
```
q
```

---

# ♻️ 8. FIFO Scheduling (simple but educational)

The system uses **First-In, First-Out** scheduling.

This means:

> The first request made is the first one processed.

### ✔ Pros:
- Easy to understand
- Good for learning
- Fair to users

### ❌ Cons:
- Not optimal for real buildings
- Elevator may waste time moving inefficiently

But the architecture allows adding better schedulers like:
- SCAN
- LOOK
- Shortest-Job-First
- Nearest-Elevator-First

---

# 🧪 9. Tests

### Unit tests:
- Elevator movement
- Queue behavior

### Integration tests:
- Many requests at once
- No deadlocks
- No race conditions

Run tests:

```bash
cd tests
dotnet test
```

---

# 🔮 10. Future Improvements

Can be extended to:

### 🚀 Multiple elevators
Add:
- Multiple controllers
- A global dispatcher

### 🤖 Smarter scheduling
Implement:
- SCAN
- LOOK
- SJF

### 🌐 Modern UI
Replace console with:
- Web API
- Dashboard
- Desktop app

### 📊 Observability
Track:
- Waiting times
- Load
- Elevator health

---

# 🏆 11. Why this is a great learning project

- Uses **clean architecture principles**
- Fully **thread-safe**
- Demonstrates **background workers**
- Excellent for interviews
- Very readable code
- Great playground for extending and experimenting

---

# ❓ 12. FAQ

### Does this control a real elevator?
No — it’s a simulation.

### Why so many layers for something simple?
Because clean structure helps:
- Learning
- Testing
- Extending
- Avoiding bugs

### What is thread safety?
It means the program behaves correctly even when many things happen at the same time.

---

# 🎉 Final Notes

If you’re a junior developer:
> Open `Program.cs`, then explore the controller and the domain classes.  
> Think of the project like a small company — each part has one responsibility.

If you’d like a **multi-elevator version** or a **web API version**, feel free to ask!
