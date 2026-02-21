# Assignment 3: Antymology

This project mimics ants building a nest with a queen ant, similar to real life.

![Ants](Images/antQ2.mov)

## Overview

This project utlizes pheromones to guide ants towards wanted blocks. Each ant follows the same logic to supprot the queen. The queen follows her own ruleset. 

## How It Works
The worker ants have the following logic they are following:
1. They start with 500 health
2. Ants move every second
3. The ant is a rigidbody, meaning it is affected by gravity. It uses this to land exactly ontop of blocks
4. Every move, their health is decreased by 5
5. If the queen's health is less than 300, all ants will try to rush to her nest to donate health to her
6. An ant donates half their health to the queen if it on a nest block
7. If anther ant is on the same block, the ant with more health will donate to the other ant
8. Ants will use pheromones to know where the nest blocks are
9. Mulch blocks have their own pheromones so ants will naturally try to follow this if the queen is not in danger
10. If the ants run into a big wall (taller than 2 blocks), they will try moving in a random direction
11. If there are more than 2 ants on a block, the ant will try randomly moving elsewhere
12. If the queen dies, all ants die

The queen ant has the following logic:
1. The queen ant is the biggest red ant on the map
2. Start with 900 health and that is the maximum health it can accumulate
3. The queen only moves every 4 seconds
4. The queen is a rigidbody and uses this to land exactly ontop of blocks. However, this also means that to go from one block to the next, the queen 'hops'
5. The queen randomly moves left, right, forward, or backwards
6. Every move is followed by a nest block being placed
7. If a nest block is placed, pheromones start emitting from it

### Notes
The graphical interface was changed to make it work with a trackpad. Please either use a trackpad or change it back to work with a mouse (see commit history).

### Bugs
This was the first time I used Unity in-depth to build a game like this. I learnt a lot. One big learning curve was figuring out how to make the ants move correctly. My ants had a tendency to fly or fall out of the map.
![Ants](Images/antFlying.mov)

![Ants](Images/antsFalling.mov)

I initially tried making the ants move like they were walking before switching to making them snap to their positions. I also learnt about RigidBodies near the end of implementing and switched everything to be a RigidBody. 
I also had a weird bug where my enitre WorldManager would be deleted after a bit but I figured it out after a few hours!
It was a big learning curve but I feel I am more prepared to start the next assignment with Unity!
 
