import "../System.sch";

/// <summary>When a player's score reaches a given quantity.</summary>
[TriggerCondition]
function score(Player player, ScoreType scoreType, Comparison comparison, Quantity quantity);

/// <summary>When a player has the lowest score.</summary>
[TriggerCondition]
function lowest_score(Player player, ScoreType scoreType);

/// <summary>When a player has the highest score.</summary>
[TriggerCondition]
function highest_score(Player player, ScoreType scoreType);