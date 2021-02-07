import "../System.sch";

/// <summary>When a player owns a quantity of units at location.</summary>
[TriggerCondition]
function bring(Player, Comparison, Quantity, Unit, Location);

/// <summary>When a player commands a number of units.</summary>
[TriggerCondition]
function commands(Player player, Comparison comparison, Quantity quantity, Unit unit);

/// <summary>When a player commands the least number of units at a location.</summary>
[TriggerCondition]
function commands_least(Player player, Unit unit, Location location);

/// <summary>When a player commands the least number of units from anywhere.</summary>
[TriggerCondition]
function commands_least(Player player, Unit unit);

/// <summary>When a player commands the most number of units at a location.</summary>
[TriggerCondition]
function commands_most(Player player, Unit unit, Location location);

/// <summary>When a player commands the most number of units from anywhere.</summary>
[TriggerCondition]
function commands_most(Player player, Unit unit);

/// <summary>When a player has killed a number of units.</summary>
[TriggerCondition]
function killed(Player player, Comparison comparison, Quantity quantity, Unit unit);

/// <summary>When a player has killed the lowest quantity of a given unit.</summary>
[TriggerCondition]
function killed_least(Player player, Unit unit);

/// <summary>When a player has killed the highest quantity of a given unit.</summary>
[TriggerCondition]
function killed_most(Player player, Unit unit);

/// <summary>When a player has lost a number of units.</summary>
[TriggerCondition]
function deaths(Player player, Comparison comparison, Quantity quantity, Unit unit);