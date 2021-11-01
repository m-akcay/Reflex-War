# Reflex-War

Color puzzle and reaction time based simple multiplayer game

There are 2 phases in the game: reflex phase and spawn phase
In reflex phase, players need to connect the color nodes in the given order to successfully finish the reflex phase and give themselves a shooter to spawn.
Unsuccessful reflex phases do not lead to a spawn.
Number of color nodes to combine starts with 4 and ends with 10; however, difficulty multiplier is not capped to 10 and continues to increase as the new reflex phases come.
Difficulty multiplier does not make the phase harder but it makes the troops stronger if the phase leads to a spawn.
Number of color nodes is also used for setting up the shooter parameters, combined with the reaction time.

There are 3 types of shooters -> Purple, Green and Whitish. 
Players gain one of them solely based on their reaction time on the reflex phase compared to the maximum time allowed in that phase.
Different type of shooters have different attributes which are: movement speed, fire rate and range.
Range is capped to prevent shooters from shooting from spawn point.
Movement speed and fire rate is not capped and shooters continue to get more powerful as the difficulty increases.
For example, a purple troop spawned when difficulty is 13 is more powerful than the one spawned with difficulty 8 or a green shooter that is spawned with difficulty 12 can be more powerful than a purple troop that is spawned with difficulty 8.

Shooters choose the closest tower to attack
