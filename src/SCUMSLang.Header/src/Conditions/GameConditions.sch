/// <summary>When a certain amount of time has elapsed.</summary>
[TriggerCondition]
function elapsed_time(Comparison comparison, Quantity quantity);

/// <summary>When the countdown timer reaches a amount of seconds.</summary>
[TriggerCondition]
function countdown(Comparison comparison, Quantity quantity);

/// <summary>When a player has a number of opponents remaining in the game.</summary>
[TriggerCondition]
function opponents(Player player, Comparison comparison, Quantity quantity);