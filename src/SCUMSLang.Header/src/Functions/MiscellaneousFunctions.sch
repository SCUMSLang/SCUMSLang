/// <summary>Prints a message, defaults to specified player.</summary>
function print(Text text, Player player);

/// <summary>Prints a message, defaults to all players.</summary>
function print(Text text);

/// <summary>Returns a random value between 0 and 255 (inclusive).</summary>
function random();

/// <summary>Gets the amount of present players.</summary>
function present_players(Player player);

/// <summary>Checks if a player is in the game.</summary>
function is_present(Player player);

/// <summary>Waits for a given amount of milliseconds. (Use with care!)</summary>
fucntion sleep(Quantity);

/// <summary>Pauses the game (singleplayer only).</summary>
function pause_game();

/// <summary>Unpauses the game (singleplayer only).</summary>
function unpause_game();

/// <summary>Sets the next map to run (singleplayer only)</summary>
function set_next_scenario(Text);

/// <summary>Plays .wav sound for specified player. The .wav from specified file path is saved once to map.</summary>
function play_sound(string file_path, Player player);

/// <summary>Plays .wav sound for all players. The .wav from specified file path is saved once to map.</summary>
function play_sound(string file_path);