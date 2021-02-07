import "../EnumTypes/Index.sch";

/// <summary>Ends the game for player with EndCondition.</summary>
function end(Player player, EndCondition endCondition);

/// <summary>Sets the alliance status between two players.</summary>
function set_alliance(Player player, Player targetPlayer, AllianceStatus allianceStatus);

/// <summary>Sets the mission objectives, defaults to all players.</summary>
function set_mission_objectives(Text text, Player player);

/// <summary>Sets the mission objectives, defaults to all players.</summary>
function set_mission_objectives(string text);

/// <summary>Centers DstLocation on a unit at SrcLocation.</summary>
function move_loc(Unit unit, Player player, Location sourceLocation, Location destinationLocation); 

/// <summary>Sets the countdown timer.</summary>
function set_countdown(int expression);

/// <summary>Adds to the countdown timer.</summary>
function add_countdown(int expression);

/// <summary>Subtracts from the countdown timer.</summary>
function sub_countdown(int expression);

/// <summary>Pauses the countdown timer.</summary>
function pause_countdown();

/// <summary>Unpauses the countdown timer.</summary>
function unpause_countdown();

/// <summary>Mutes unit speech.</summary>
function mute_unit_speech();

/// <summary>Unmutes unit speech.</summary>
function unmute_unit_speech();