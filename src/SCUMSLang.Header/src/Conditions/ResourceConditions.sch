/// <summary>When a player owns a quantity of resources.</summary>
[TriggerCondition]
function accumulate(Player player, Comparison comparison, Quantity quantity, ResourceType resourceType);

/// <summary>When a player owns the most of a given resource.</summary>
[TriggerCondition]
function most_resources(Player player, ResourceType resourceType);

/// <summary>When a player owns the least of a given resource.</summary>
[TriggerCondition]
function least_resources(Player player, ResourceType resourceType);