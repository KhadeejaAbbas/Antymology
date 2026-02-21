# Assignment 3: Antymology

This project mimics ants building a nest with a queen ant, similar to real life.

![Ants](Images/Ants.gif)

## Overview

This project utlizes pheromones to guide ants towards wanted blocks. Each ant follows the same logic to supprot the queen. The queen follows her own ruleset. 

## How It Works
The worker ants have the following logic they are following:
1. They start with 500 health
2. Ants move every second
3. Every move, their health is decreased by 5
4. If the queen's health is less than 300, all ants will try to rush to her nest to donate health to her
5. If anther ant is on the same block, the ant with more health will donate to the other ant
6. Ants will use pheromones to know where the nest blocks are
7. Mulch blocks have their own pheromones so ants will naturally try to follow this if the queen is not in danger
8. If the ants run into a big wall (taller than 2 blocks), they will try moving in a random direction
9. If there are more than 2 ants on a block, the ant will try randomly moving elsewhere

The queen ant has the following logic:
1. Start with 900 health and that is the maximum health it can accumulate
2. The queen only moves every 4 seconds

You are able to experience it generating an environment by simply running the project once you have loaded it into unity.

### Agents
The agents component is currently empty. This is where you will place most of your code. The component will be responsible for moving ants, digging, making nests, etc. You will need to come up with a system for how ants interact within the world, as well as how you will be maximising their fitness (see ant behaviour).

### Configuration
This is the component responsible for configuring the system. For example, currently there exists a file called ConfigurationManager which holds the values responsible for world generation such as the dimensions of the world, and the seed used in the RNG. As you build parameters into your system, you will need to place your necesarry configuration components in here.

### Terrain
The terrain memory, generation, and display all take place in the terrain component. The main WorldManager is responsible for generating everything.

### UI
This is where all UI components will go. Currently only a fly camera, and a camera-controlled map editor are present here.

## Requirements

### Admin
 - This assignment must be implemented using Unity 2019or above (see appendix)
 - Your code must be maintained in a github (or other similar git environment) repository.
 - You must fork from this repo to start your project.
 - You will be marked for your commit messages as well as the frequency with which you commit. Committing everything at once will receive a letter grade reduction (A →A-).
 - All project documentation should be provided via a Readme.md file found in your repo. Write it as if I was an employer who wanted to see a portfolio of your work. By that I mean write it as if I have no idea what the project is. Describe it in detail. Include images/gifs.

### Interface
- The camera must be usable in play-mode so as to allow the grader the ability to look at what is happening in the scene.
- You must create a basic UI which shows the current number of nest blocks in the world

### Ant Behaviour
- Ants must have some measure of health. When an ants health hits 0, it dies and needs to be removed from the simulation
- Every timestep, you must reduce each ants health by some fixed amount
- Ants can refill their health by consuming Mulch blocks. To consume a mulch block, and ant must be directly ontop of a mulch block. After consuming, the mulch block must be removed from the world.
- Ants cannot consume mulch if another ant is also on the same mulch block
- When moving from one black to another, ants are not allowed to move to a block that is greater than 2 units in height difference
- Ants are able to dig up parts of the world. To dig up some of the world, an ant must be directly ontop of the block. After digging, the block is removed from the map
- Ants cannot dig up a block of type ContainerBlock
- Ants standing on an AcidicBlock will have the rate at which their health decreases multiplied by 2.
- Ants may give some of their health to other ants occupying the same space (must be a zero-sum exchange)
- Among your ants must exists a singular queen ant who is responsible for producing nest blocks
- Producing a single nest block must cost the queen 1/3rd of her maximum health.
- No new ants can be created during each evaluation phase (you are allowed to create as many ants as you need for each new generation though).

## Tips
Initially you should first come up with some mechanism which each ant uses to interact with the environment. For the beginning phases your ants should behave completely randomly, at least until you have gotten it so that your ants don't break the pre-defined behaviour above.

Once you have the interaction mechanism nailed down, begin thinking about how you will get your ants to change over time. One approach might be to use a neural network to dictate ant behaviour

https://youtu.be/zIkBYwdkuTk

another approach might be to use phermone deposits (I\'ve commented how you could achieve this in the code for the AirBlock) and have your genes be what action should be taken for different phermone concentrations, etc.

## Submission
Export your project as a Unity package file. Submit your Unity package file and additional document using the D2L system under the corresponding entry in Assessments/Dropbox. Inlude in the message a link to your git repo where you did your work.
