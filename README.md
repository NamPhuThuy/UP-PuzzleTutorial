# UPack-Puzzle-Tutorial

## IDEA

Create some default step for the tutorial hand, the game will create a TutorialAdapter to connect between the main-game and the Tutorial module

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