/// <summary>Spawns units at a location with optional properties.</summary>
function spawn(Unit unit, Player player, int expression, Location location);

/// <summary>Kills units at a location.</summary>
function kill(Unit unit, Player player, int expression, Location location);

/// <summary>Kills units from anywhere.</summary>
function kill(Unit unit, Player player, int expression);

/// <summary>Removes units at a location.</summary>
function remove(Unit unit, Player player, int expression, Location location);

/// <summary>Removes units from anywhere.</summary>
function remove(Unit unit, Player player, int expression);

/// <summary>Moves units from one location to another.</summary>
function move(Unit unit, Player player, int expression, Location sourceLocation, Location destinationLocation);

/// <summary>Orders units to move, attack or patrol.</summary>
function order(Unit unit, Player player, Order order, Location sourceLocation, Location destinationLocation);

/// <summary>Modifies a unit's HP, SP, energy or hangar count.</summary>
function modify(Unit unit, Player player, int expression, UnitMod unitMod, int modQuantity, Location location);

/// <summary>Gives units to another player.</summary>
function give(Unit unit, Player sourcePlayer, Player destinationPlayer, int expression, Location location);

/// <summary>Toggles a doodad's state.</summary>
function set_doodad(Player player, Unit unit, State state, Location location);

/// <summary>Toggles invincibility for units at location.</summary>
function set_invincibility(Player player, Unit unit, State state, Location location);

/// <summary>Runs an AI script at a location.</summary>
function run_ai_script(Player player, AIScript aiScript, location Location);

/// <summary>Runs an AI script.</summary>
function run_ai_script(Player player, AIScript aiScript);