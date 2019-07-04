# Messenging_ECS

## Description

Messenging_ECS is a humble library for using a message, or event based system with Unities Entity Component System.
It is recommended to have all non-deterministic actions be initiated through a message. Doing so will provide insight into what actions and changes occur during the lifecycle of your game or project, using the message logging system.
Moreover, it will be possible to record and replay the last game session.
If non-deterministic actions occur without passing through the message system, the replay may show a different result than the actually played session.

## Demo

A simple demo is included to illustrate how the messenging system can be used.

![](docs/images/demo_01.png)

### Playing the demo

The cube can be moved around using the wasd keys.
The cube can emit blue shiny spheres in multiple directions by pressing the enter key.
The cube can grow grass around it by pressing the spacebar.

The number of spheres and grass halms spawned per frame can be set by clicking the cube and modifying the Spawn Count on the ProjectilePrototypeProxy, and the PlantPrototypeProxy respectively.

### Logging

By default all handled messages will be serialized and output to the console.
By clicking the Settings object, log settings can be adjusted in the inspector window.
Logging can be enabled or disabled by modifying the Logging Enabled checkbox.
It is possible to filter what messages should be output to the console by adding patterns to the Log Filter array.

### Replay

By default all messages handled will be saved so that the last session played can be replayed.
To replay the last session, select the Settings object, and check the Replay Enabled checkbox.
When hitting the play button, the recording of the last played session will be shown.
