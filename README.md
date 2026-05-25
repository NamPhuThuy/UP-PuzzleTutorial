# UPack-Puzzle-Tutorial

## IDEA

Create some default step for the tutorial hand, the game will create a TutorialAdapter to connect between the main-game and the Tutorial module

## MODULE FEATURES

- **TutorialManager** (`TutorialManager.cs`): **Coordinator** — Manages key-components (Hand, Panel, Data) and holds current gameplay adapter reference.
- **TutorialData** (`TutorialData.cs`): **Configuration** — ScriptableObject storage defining tutorial flows, step actions, rewards, and booster unlocking.
- **TutorialHand** (`TutorialHand.cs`): **Visual Pointer** — Drives hand gestures, movement pathing, dragging, and animations using Images or Spine skeletons.
- **TutorialPanel** (`TutorialPanel.cs`): **UI Overlay** — Renders instruction text messages, handles alpha fading transitions, and displays skip option.
- **BoosterRule** (`BoosterRule.cs`): **Booster State** — Controls locked/unlocked overrides and grants initial item counts for specific tutorial levels.

## HOW TO USE

Step 1: Create a new class TutorialAdapter inhertited from TutorialAdapterBase in your project
Step 2: Create the scriptableObject TutorialData

The submodule should be placed like this:
<pre>
_Project/
├── Tutorial_Module/
│   ├── TutorialAdapter.cs
│   ├── TutorialData.asset
│   └── UPack-Puzzle-Tutorial/
│   │   ├── 
│   │   ├── TutorialHand.cs // Control the tutorial-hand
│   │   ├── TutorialData.cs
│   │   ├── 
│   │   ├── TutorialManager.cs //
</pre>


## NOTE
When tutorial-step is completed in gameplay, call the TutorialAdapterBase.OnStepCompletedFromGameplay() to mark the completion

The module is lack of Booster-declaration, use your game Booster to match with this

If Spine module is being used, ensure added the scripting symbol "USE_SPINE"