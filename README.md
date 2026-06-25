# 🚀 Stellar Echoes — Survival is the Real Victory

> **Control a lone spaceship in deep space. Fight endless waves of asteroids and enemy ships. Stay alive as long as you can.**

![Platform](https://img.shields.io/badge/Platform-Android-3DDC84?style=flat&logo=android&logoColor=white)
![Engine](https://img.shields.io/badge/Engine-Unity-000000?style=flat&logo=unity&logoColor=white)
![Language](https://img.shields.io/badge/Language-C%23-239120?style=flat&logo=csharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-Shipped-brightgreen?style=flat)
![Type](https://img.shields.io/badge/Type-Original%20Title-blueviolet?style=flat)

---

## 🎮 Play It

[![Play on itch.io](https://img.shields.io/badge/Play_on-itch.io-FA5C5C?style=flat&logo=itch.io&logoColor=white)](https://gameofbugsofficial.itch.io/stellar-echoes)

---

## 📖 About

Stellar Echoes is a fast-paced arcade space shooter built entirely from scratch. You control a lone spaceship in deep space, fighting endless waves of asteroids and enemy ships while trying to survive just a little longer every time.

Simple to start, intense to master — every run feels different and every high score feels earned.

---

## 🎮 Core Gameplay

- Fly freely in all directions while dodging incoming threats
- Shoot asteroids and enemy ships to survive longer
- Collect power-ups that change how you play
- Face increasing difficulty the longer you stay alive

**Your mission: stay alive, score higher, push your limits.**

---

## 🔹 Features

- **⚔️ Multiple Enemy Types** — destructible asteroids and enemy ships with distinct behavior patterns
- **🎁 Power-Up System** — rapid fire, shields, and multi-shot pickups that meaningfully shift gameplay mid-run
- **📈 Dynamic Difficulty Scaling** — enemy spawn rate, speed, and aggression increase the longer you survive
- **🚀 Free Movement** — full directional control optimized for both keyboard and mobile touch
- **🏆 Score Attack** — high score system designed for replayability and personal bests

---

## 🔧 What I Built

- **Wave Spawner with Dynamic Scaling** — enemy and asteroid spawn rates, speeds, and counts increase at timed intervals based on survival time, keeping pressure escalating without feeling scripted
- **Power-Up System** — three distinct pickups (rapid fire, shield, multi-shot) with timed durations, each implemented as independent components that modify the player's weapon state on collection and revert cleanly on expiry
- **Enemy AI** — multiple enemy types with different movement and attack patterns; asteroids drift on randomized vectors, enemy ships track and strafe the player
- **Weapon System** — base single shot upgradeable to multi-shot via power-up, with configurable fire rate driven by rapid fire pickup
- **Collision & Health System** — shield absorbs one hit, health decrements on unshielded hits, death triggers score save and game over flow
- **Score & High Score** — session score tied to survival time and kills, personal best persisted with PlayerPrefs

---

## 🧠 What I Learned

- **Game balance through playtesting** — difficulty scaling that feels fair required many iterations; too fast and it felt unfair, too slow and it felt boring
- **Power-up architecture** — designing pickups as self-contained components that cleanly modify and restore player state without coupling to the player script
- **Enemy variety on a budget** — creating meaningfully different enemy feel using the same base AI with tweaked parameters (speed, turn rate, fire rate) rather than entirely separate systems
- **Player flow** — structuring the game loop so each death feels like *my mistake* rather than *unfair design*, which is the core of good arcade feel

---

## ⚙️ Tech

| Tool | Use |
|------|-----|
| Unity | Game engine |
| C# | All game logic |
| PlayerPrefs | Persistent high score |
| Rigidbody2D | Ship and projectile physics |

---

## 🚀 Run Locally

1. Clone: `git clone https://github.com/gameofbugs/Stellar-Echoes`
2. Open in Unity Hub
3. Open the main scene and press Play

---

## 👤 About

Built solo by **Manoj S** — one of 7 shipped Android Unity games. Stellar Echoes is the project I'm most proud of in my solo dev journey.
More projects: [gameofbugsofficial.itch.io](https://gameofbugsofficial.itch.io)
