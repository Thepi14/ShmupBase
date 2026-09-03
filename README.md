# ShmupBase
A Unity project that can be used as a base for making shmups, the project features:
- Two Scenes, one for the main menu and another for the game;
- Bullet system, featuring:
  - Basic bullets;
  - Laser bullets;
  - Curved laser bullets;
  - Parametric-function based bullets;
  - All these bullets have parameters like velocity, acceleration, rotationspeed, a implemented coroutine that can be executed and more.
- Entity system, with inheritance and interfaces for movement, health and attacks;
- System for collectable items, that can give points and string arguments when collected;
- Player system, with support for inheritance, giving the option for multiple characters;
- Boss system, that also allows multiple bosses;
- Score system;
- Complete replay system, that also supports random number generators, that are implemented on GameManager.cs;
- Sound system, that can play songs, pause, play sounds at locations, with support for the Audio Random Containers;
- Pause system, based on Time.timeScale, with a UI completely based on LateUpdate(), making the UI unnafected by the time scale;
- Complete stage system, using coroutine based stages, also coming with a ending event that can trigger a custom behaviour defined by the developer;
- Practice mode, which allows stage practice or boss only practice;
- Continue system for when player dies, also coming with the punishment of not triggering the ending event;
- A camera effect system, coming with code that can manipulate post effect profiles with animators, and also coming with a shake camera effect;
- Also multiple different cameras, one for the UI, one for the game itself, another for the stages background, and the last one for the UI black background;
- Automatic screen scaling system, which maintains the game always at the same screen proportions no matter how the games window is scaled;
- An full screen switch manager that fixes screen scaling when changing resolutions;
- A complete prefab based UI, which can speedup UI customization and configuration considerably;
- TextMeshPro package;
- An already implemented localization package, with all UI texts having Localize String Events components to update when localization changes;
- The Unity's "new" Input System, with (almost) all controls already defined;
- Custom function libraries, that each gives helping functions for varied uses;
- A collection of editor scripts that helps production;
- Various extension functions for Unity's components, classes and structs;
- A mouse locking system for games that run on consoles, alongside a complete UI navigation.
- A complete start screen, with other panels like:
  - Difficulty selection panel;
  - Character selection panel;
  - Practice stage selection panel;
  - Replay selection panel;
  - Music room;
  - Settings panel, featuring:
    - General section;
    - Sound section;
    - Graphics section;
    - Controls section;
    - Localization section;


(This project is still in development, so many bugs _will_ be present.)
