# Ourea
## Project Description

## My Work

### Gameplay
I created all gameplay withing the game. Below I outline some specifics that I created over the 9 months of development.

#### Player
I implemented all player mechanics. This included movement, interactions, animations 
implementation, cross-scene cutscene controls and all others.

#### Puzzle Implementation
I implemented the three puzzles and the final battle mechanics. This included creating the growth rune, the squish rune, the seesaw mechanic, the broken growth rune, the boulder and the planks for the bridge puzzle.

### Scene Loading
Ourea is playable from start to finish, without a loading screen once the game started. This was due to the game long-shot approach, which meant that the camera would not cut throughout the entire game. This meant that Ourea required an additive scene loading strategy. The game was split into 11 sections. These were loaded in, mid gameplay, whenever the player progressed to them and unloaded when the player was unable to backtrack to them.

The final version of the level loading worked in the following ways. A scene bundle asset that contains references to its Assets, Camera and Lights scene gets used to async load those scenes. Then the three scenes are merged to allow for easier unloading. The lag spike caused by loading in new sections was optimised by splitting the load into three async operations as well as creating a Gradual Loader that instantiated monobehaviours frame by frame. This was done using the Abstract class Gradual Loader that allowed for all intensive monobehaviours to be easily adopted into the gradual loading process.

### Menus
I created menus for the game that worked for both controller and keyboard and mouse. The menus included:
Settings: This allowed you to changes various game and audio settings which would save across different sessions.
Controls: This was a small tab that displayed how to control the game, with both controller and keyboard.
Gallery: This tab showcased all of the discovered intractable cave paintings that player had found. It also allowed the player to rewatch them.
Level Select: This tab gets unlocked, once the player finishes the game. It allows the player to replay the game from four different points in the story.

### Game Saving
I created a save system that loads the player in where they last left off. This was done through a binary encoder for security. This is integrated into the level select in the Ui. I also added saving of settings and audio levels through player prefs.

### Optimisation
As the game got closer to release, I spent a large amount of time optimising the game. This included the use of occlusion culling, lighting optimisations and static batching and GPU instancing. The Gradual loader was also introduced late in development, which helped tremendously with loading frame rate drops.

### Steam Integration
I integrated steam achievements and other steam functionality using a C# Steamworks wrapper.

### Audio
 I created all sounds (excluding music) in the game. The process was sourcing multiple sounds on FreeSounds.org, editing them heavily and combining multiple sounds in Adobe Audition and Audacity. This also included creating audio for the various cutscene throughout the game, SFX and ambience.

### Lighting
I created the lighting for all areas of the game. Each section was reworked multiple times throughout the phases of the project. Lighting also included creating local density volumes to accentuate volumetric rays. 

## Usage Help
