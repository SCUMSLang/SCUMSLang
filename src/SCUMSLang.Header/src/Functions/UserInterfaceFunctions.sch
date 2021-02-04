/// <summary>Centers the view on a location for a player.</summary>
function center_view(Player player, Location location);

/// <summary>Triggers a minimap ping on a location for a player.</summary>
function ping(Player player, Location location);

/// <summary>Shows the unit talking portrait for an amount of seconds.</summary>
function talking_portrait(Player player, Unit unit, Quantity quantity);

/// <summary>Sets whether destination player will share vision with source player.</summary>
function set_vision(Player sourcePlayer, Player destinationPlayer, boolean vision);