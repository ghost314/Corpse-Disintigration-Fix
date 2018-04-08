The general purpose of this mod, is to prevent corpses from blowing themselves up, before the player has a chance to loot them.

This is done by altering the spawn location of corpses to prevent them from stacking on top of one another, or on top of other blocks that can't support additional blocks on top of them.

Depending on the circumstances, this may cause corpses to show up in some odd locations, but the algorithm will always pick the closest (in terms of latitude and longitude) available spot relative to where the zombie died.

After installing the mod, you can adjust some of the corpse positioning behavior by editing the new "CorpseDisintigrationFixConfig" block in blocks.xml. The configuration properties are:

MAX_SEARCH_RADIUS - The maximum horizontal distance (in blocks) that a corpse is allowed to appear from the location where the zombie died. Decrease to improve performance, increase if you need corpses to pile up farther away.

MAX_HEIGHT - The highest point in the game world that a corpse can be positioned at (this should not generally be changed).

MIN_HEIGHT - The lowest point in the game world that a corpse can be positioned at (this should not generally be changed).

CACHE_PERSISTANCE - This mod caches the blocks it scans while positioning corpses to improve performance. This number is the amount of time, in game 'ticks', that the cache is allowed to persist, before it is cleared. Increase to improve performance, at the expense of corpse placement accuracy when blocks are frequently added/removed/changed.

DEBUG_MODE - Set to true if you want any errors encountered in this mods code to force the game console to open. If set to false, the error will be silently logged to avoid breaking game immersion.

SPAWN_ON_SPIKES - Set to true if you want corpses to be allowed to spawn on top of blocks that deal damage over time to anything on top of them (log spikes, barbed wire, wooden spikes etc). Bear in mind that if these blocks are destroyed from attrition of zombies running into them, the corpse on top of them will fall and be destroyed.