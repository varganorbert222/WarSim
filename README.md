# Dynamic Battlefield Simulation Engine – Backend  
# Dinamikus Hadszíntér Szimulációs Motor – Backend

---

# 🇭🇺 Magyar leírás

## Dinamikus Hadszíntér Szimulációs Motor – Backend

Ez a projekt egy .NET 8 alapú, nagy teljesítményű szimulációs motor, amely egy valós idejű, dinamikus hadszíntér működését modellezi. A rendszer több ezer egységet, frontvonalat, infrastruktúrát és ballisztikus objektumot képes kezelni, miközben valós időben szolgáltat adatot webes és Unity kliensek számára.

## Fő funkciók

- **Valós idejű szimuláció**  
  Tick-alapú frissítés (100–200 ms), párhuzamosított egység- és lövedékkezeléssel.

- **Egységek kezelése**  
  Pozíció, állapot, irány, mozgás, AI döntések.

- **Frontvonal és infrastruktúra**  
  Úthálózat, áramhálózat, stratégiai objektumok modellezése.

- **Ballisztikus objektumok**  
  Rakéták, tüzérségi lövedékek pályaszámítása.

- **REST API + később SignalR**  
  A frontend (Angular) és a Unity kliens valós idejű kiszolgálása.

## Architektúra

- **Controller alapú Web API**
- **Service réteg** (WorldState, Simulation, AI, Pathfinding, Projectiles)
- **Párhuzamosított szimuláció** (.NET TPL)
- **Lock-mentes snapshot rendszer**

## Cél

A backend biztosítja a teljes hadszíntér logikát és szimulációt, amelyre a webes térkép (Angular) és a 3D vizualizáció (Unity) épül.  
A rendszer moduláris, skálázható és hosszú távon bővíthető.

## Licenc

MIT License

---

# 🇬🇧 English Description

## Dynamic Battlefield Simulation Engine – Backend

This project is a high‑performance simulation engine built on .NET 8, designed to model a real‑time dynamic battlefield. The system can manage thousands of units, frontlines, infrastructure elements, and ballistic objects while providing live data to both web and Unity clients.

## Key Features

- **Real‑time simulation**  
  Tick‑based updates (100–200 ms) with parallelized unit and projectile processing.

- **Unit management**  
  Position, state, heading, movement, and AI decision‑making.

- **Frontline and infrastructure**  
  Road networks, power grids, and strategic structures.

- **Ballistic objects**  
  Missile and artillery trajectory simulation.

- **REST API + future SignalR support**  
  Real‑time communication with Angular and Unity clients.

## Architecture

- **Controller‑based Web API**
- **Service layer** (WorldState, Simulation, AI, Pathfinding, Projectiles)
- **Parallelized simulation** using .NET TPL
- **Lock‑free snapshot world state**

## Purpose

The backend provides the full battlefield logic and simulation layer that powers the web‑based map (Angular) and the 3D visualization (Unity).  
The system is modular, scalable, and built for long‑term extensibility.

## License

MIT License

---

# 🇬🇧 One‑sentence project description

**A high‑performance .NET 8 simulation engine for real‑time dynamic battlefield environments, powering both web and Unity clients.**
