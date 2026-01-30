# PropsSpawn
**PropsSpawn** is a plugin for SCP: Secret Laboratory using EXILED . Introduces a simple way to spawn props (predefined SCP:SL prefabs). You can manage these props however you like, moving them, rotating them, and more! It is designed to make map customization fast, intuitive, and fully in-game, without needing external map editors.

![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/SpuderMANs/PropsSpawn/total?style=for-the-badge)
![Latest](https://img.shields.io/github/v/release/SpuderMANs/PropsSpawn?style=for-the-badge&label=Latest%20Release&color=%23D91656)

---
## Get started

To get started, make sure you have:

* EXILED installed
* Hint Service Meow
* A compatible SCP: Secret Laboratory server

Place the plugin DLL inside your EXILED plugins folder and restart the server.

---
## How to use Prop Gun

When you equip the **Prop Gun**, a GUI will appear showing the currently selected mode  
(**Spawn**, **Delete**, **Move**, or **Rotate**).

You can switch between modes by pressing **ALT** (or the key bound to **NoClip**).
You can also change the axis of rotation using the R command (Reloading Command) while in Rotate mode.

---

### Spawn mode
To spawn a prop, select it via the **Remote Admin console**
Example: `pr spawn boxe`
You can replace `boxe` with any available prop name.  
To see the full list of available props, use: `pr list`

### Delete mode
Switch to **Delete** mode and shoot at a prop to remove it.

### Move mode
Switch to **Move** mode and shoot a prop to select it.  
Then shoot at another position to move the prop there.

### Rotate mode
Switch to **Rotate** mode and shoot a prop to rotate it.  
While in Rotate mode, you can change the rotation axis by pressing **R**.



## Features

### In-game prop spawning

Spawn any networked prefab directly in-game using partial name matching.
No need to know the full prefab name — PropSpawn will find it for you.

---

### Prop manipulation

Interact with spawned props using simple actions:

* **Move props** by attaching them to your view and placing them elsewhere
* **Rotate props** incrementally on each axis
* **Delete props** directly by looking at them

All interactions are performed in real time and synced to all players.
---

### Map saving system

Save all spawned props to a JSON file:

* Each prop is linked to its **room**
* Position is stored as a **room-local offset**
* Rotation is fully preserved

This makes saved maps resilient to world position changes.

---

## Support

If you find bugs or want to suggest features:

* Open an issue on GitHub
* Or contribute via pull requests
---
## Avaible Props
<img width="1257" height="618" alt="Screenshot 2026-01-30 074142" src="https://github.com/user-attachments/assets/1b213084-db83-4c13-b750-3af81972098f" />


