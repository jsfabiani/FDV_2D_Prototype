# FDV_2D_Prototype

This project is a prototype of a 2D game. Inspired by metroidvanias, the player must traverse the level, earning points (money) to buy abilities that unlock more parts of the level. Once all abilities are unlocked, the player is free to face the boss and end the game.

# Game Mechanics

## Player Character
![PlayerImage]()

The Player is managed by two scripts: ![PlayerCharacter](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Player/PlayerCharacter.cs) and ![PlayerController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Player/PlayerController.cs), as well as three auxiliary children objects. PlayerCharacter manages the health and score, as well as picking up objects or being hit by enemies which may modifiy them. It also manages ending the game when the player's health reaches 0 and manages most of the character's audio.

PlayerController manages the behavior of the player character. It uses a state machine with the following states:
- Idle: when there are no inputs.
- Running: when the player is moving on the ground.
- Jump: when the player jumps.
- Double Jump: when the player jumps while on the air.
- Fight: when the player attacks.
- Defeated: when the player's health reaches 0.

Double Jump and Fight must be unlocked before the player is able to use these actions. PlayerController also manages the animations and sound effects for jumping and attacking.

![ChildrenImage]()
There are three children objects that round up the Player: ![HitDetector](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Player/HitDetector.cs), ![JumpDetector](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Player/JumpDetector.cs) and ![AreaDetector](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Player/AreaDetector.cs), with their corresponding scripts. HitDetector is a trigger volume that invokes an event for damaging an enemy on contact. It's activated by attacking in PlayerController, and deactivated automatically after a brief window. JumpDetector is a smaller trigger volume under the player, that manages the detection of collisions with floors and enemies to recharge jumps. AreaDetector is another small trigger inside the Player to manage changes in area. It's separated into its own script to avoid unwanted behaviors when attacking, which activates a trigger volume.

The player's health is managed through the script ![Health](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Health.cs), which is also used by all enemies.

## Enemies
![Enemies]
The enemies are also managed by two scripts: ![Enemy](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Enemies/Enemy.cs) and ![EnemyController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Enemies/EnemyController.cs). Enemy is a generic script managing the health, the damage an enemy does to a player when colliding with it and the score the player gets when defeating them. EnemyController is a larger script, with behaviors common to many enemies for patrolling both on the ground and flying, and knockback. It's inherited by three different scripts that define the behavior of specific enemies: GroundEnemyController, SkyEnemyController and BossController.

![GroundEnemy]
The Skateboarder is the main enemy of the game. It's managed by ![GroundEnemyController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Enemies/GroundEnemyController.cs), which has a simpler state machine, with states for patrolling between two points, being knocked back after an attack and being defeated. The behavior when defeated is common to all characters: disabling the collider, making the rigidbody kinematic and stopping all movement.

![SkyEnemy]
The Drone is a nimbler enemy. It's controlled by ![SkyEnemyController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Enemies/SkyEnemyController.cs), which shares most of its behavior with the Skateboarder, the main difference being that the drone patrols on the sky and the Skateboarder on the ground.

![Boss]
The Boss is the final enemy, which when defeated marks the end of the game. Its ![controller](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Enemies/BossController.cs) is a more complex state machine. Its states are:
- Patrol: its default state, it patrols for a set amount of time, firing missiles towards the player periodically.
- Barrage: after enough time patrolling, the boss fires several missiles towards the player, making them harder to avoid.
- Reloading: after the barrage, the boss must reload for a short time, being vulnerable to attacks.
- Defeated: when defeated, the boss invokes an event ending the game.

The Boss becomes active only upon enteriing its arena. It is invulnerable when patrolling and firing its barrage; the players must wait until it is reloading to strike. The boss's missiles are managed using a pooling method in the ![ObjectPooling](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/ObjectPooling.cs) script, attached to the boss. They have their own controller ![BulletController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Enemies/BulletController.cs) and have the Enemy script, but marked invulnerable. 

## Items
![ItemsImage]()
There are two kind of items throughout the map: money and healing items. Both are managed by ![PickUpItem](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/PickUpItem.cs), which simply stores their score and healing amount to be accessed by PlayerCharacter. The healing items use the script ![HoveringItem](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/HoveringItem.cs) with a patrol behavior to create a small animation. All items are in its own layer, ignoring collisions with enemies.

The Healing items are instantiated through a pool of objects, using a script ![ObjectPooling](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/ObjectPooling.cs) attached to an empty object called Game Manager, as well as ![HealingItemManagement](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/HealingItemManagement.cs). It's important for the number of healing item locations to be the same as the amount of pooled objects.

## Platforms and door
There are three types of platform in this game: static platforms, moving platforms and invisible platforms. The moving platforms use the ![MovingPlatform](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/MovingPlatform.cs) script with a simple version of the patrol behavior. It also makes the player's transform a children of the platform's when touching it, so that the player will move with the platform. The invisible platforms use the ![InvisiblePlatform](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/InvisiblePlatform.cs) script, which turns them temporarily visible when another entity touches them.

The way to the boss is blocked by a door. The player must hit it repeatedly to open it. The door uses the Enemy script with a simple ![DoorController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/DoorController.cs) to disable it when defeated.

## Shop
![ShopImage]()
The abilities to double jump and to fight are unlocked by buying two items in a shop, each with a different price. Both are animated by the ![HoveringItem](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/HoveringItem.cs) script. The UI elements are managed by the script ![ShopItemText](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/Environment/ShopItemText.cs), which shows the item's price when the player is touching the item. Each of them has a tag to unlock their respective abilities through the PlayerCharacter script.

## Areas and cameras
![AreasImage]()
This project uses Cinemachine with four different virtual cameras: one for the tutorial platforming puzzle, one for the shop, one for the advanced platforming puzzle and one for the boss arena. There is a fifth camera which follows the player when outside those areas. The cameras are managed with the ![CameraController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/CameraController.cs) script in the Game Manager, which is activated by the player's AreaDetector. That script also has slow motion effects when the player is hit by an enemy and when the game ends.

The background is divided in four layers, which are managed by the ![BackgroundParallaxController](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/BackgroundParallaxController.cs), also in the Game Manager. It uses a texture offset method, lowering the offset speed for layers that are farther away.

## Ending the game
![GameOverScreen]()
The game ends when either the player dies or the boss is defeated. This is managed by the ![EndGame](https://github.com/jsfabiani/FDV_2D_Prototype/blob/main/Scripts/EndGame.cs) script in the Game Manager, which receives its events from either the player or the boss dying, and activates the corresponding end screen before closing the game.

# Environment, UI, Animation and Audio

## Environment

![Tilemaps]()

The game world is built using three tilemaps built using different tile palettes:
- Floor Tilemap: the main shape of the level, it uses a composite tilemap collider.
- Decor Tilemap: a tilemap for decorative objects, which doesn't have a collider.
- Backgorund Tilemap: a tilemap for background tiles, rendered behind all other tilemaps.
- Obstacle Tilemap: like the decor tilemap, but with a tilemap collider. Collisions are ignored with enemies, but not with the player.

## UI

![UI]()

The UI has a health bar as well as a score counter, which are managed in the PlayerCharacter script. Once the double jump and fight abilities are unlocked, two items will appear under the health bar signaling their availability. The text signaling the shop items's price is hidden until the player is in contact with them. Finally, there are two different game over screens, which are shown depending on whether the player succeeds or is defeated.

## Animation

The animation in this project is relatively complex. Here we leave the animators for the player, a boss and an enemy. The animator for the door is similar to the enemy's, without the running state, and the animator for the Shopkeeper uses a single animation on loop. I examine the animators in more detail in the video.

## Audio
There are three different audio mixers: SFX, Ambience and Soundtrack. The Soundtrack Audio Source is in the Game Manager, and it plays a song distorted to sound low fidelity. The Ambience tracks are tied to each area, and are activated and deactivated on entering and exit the area. The SFX are tied to the player and the enemies, representing sounds for being hurt, attacking, jumping, recovering health, etc. 


# Game Design
![Map]()
The game is designed so that the player starts on a simple area, with a slow enemy and readily available health to teach them the main mechanics. The player can pick up 50$ from this platforming challenge, learning about invisible platforms as well. After this, the player will go to the shop and realize they have enough money to buy the double jump, which unlocks most of the map. They may try to go to the right, but they'll find their progress barred by a door. Going to the far left of the map will reveal a more challenging platforming area, which has enough money to buy the fight ability. Once the fight ability is unlocked, the player may break down the door and enter the arena to fight the boss.
