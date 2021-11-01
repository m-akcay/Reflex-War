# Reflex-War

Color puzzle and reaction time based simple multiplayer game
<br>
Video -> https://drive.google.com/file/d/1h1bQiN_17ndrTvKI7maz6RwCrHT4C8s9/view?usp=sharing

<h2> Gameplay </h2>

* There are 2 phases in the game: reflex phase and spawn phase <br>
* In reflex phase, players need to connect the color nodes in the given order to successfully finish the reflex phase and give themselves a shooter to spawn<br>
* Unsuccessful reflex phases do not lead to a spawn<br>
* Number of color nodes to combine starts with 4 and ends with 10; however, difficulty multiplier is not capped to 10 and continues to increase as the new reflex phases come<br>
* Difficulty multiplier does not make the phase harder but it makes the troops stronger if the phase leads to a spawn<br>
* Number of color nodes is also used for setting up the shooter parameters, combined with the reaction time<br>
<br><br>
* There are 3 types of shooters -> Purple, Green and White(ish)<br>
* Players gain one of them solely based on their reaction time on the reflex phase compared to the maximum time allowed in that phase.<br>
* Different type of shooters have different attributes which are: movement speed, fire rate and range<br>
* Range is capped to prevent shooters from shooting from spawn point<br>
* Movement speed and fire rate is not capped and shooters continue to get more powerful as the difficulty increases<br>
* For example, a purple troop spawned when difficulty is 13 is more powerful than the one spawned with difficulty 8 or a green shooter that is spawned with difficulty 12 can be more powerful than a purple troop that is spawned with difficulty 8<br>

* Shooters choose the closest tower to attack <br>
* Spawn indicator is shown on desktop version with mouse movement but not on mobile version as it happens with a touch
<br><br>
<h2> Finish Condition </h2>
<h3> Single Player </h3>

  Game ends when all 3 towers run out of health. Score is determined with the remaining time as seconds.

<h3> Multiplayer </h3>

  Winner is determined by the total damage made to the towers by the players.
