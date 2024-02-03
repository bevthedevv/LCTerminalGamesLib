# LCTerminalGamesLib Guide

LCTerminalGamesLib is a powerful library designed for developers to create and integrate their own games into the Lethal Company terminal. This guide will walk you through the steps to utilize this library effectively.

The base mod comes with a Snake game preinstalled to help you learn the library

## Getting Started

To create a new game, you will need to instantiate a `TerminalGame` object and add it to the `TerminalGames.games` collection in your main plugin file. Here's the basic structure to follow:

```csharp
TerminalGames.games.Add(new TerminalGame(
    "GameName", // The name of your game
    "GameDescription", // A short description of your game
    GameTickSpeed, // How fast the game updates (in seconds)
    RunGame, // Method to update the game logic
    StartGame, // Method called when the game starts
    HandleKey // Method to handle key inputs
));
```

# Example: Adding a Snake Game
Here's how you could add the classic Snake game:

```csharp
TerminalGames.games.Add(new TerminalGame(
    "Snake", // Game name
    "Retro nokia game of Snake", // Game description
    0.2f, // Game tick speed (5 times per second)
    Snake.RunGame, // Method to run the game logic
    Snake.StartGame, // Method called on game start
    Snake.HandleKey // Method to handle key inputs
));
```

# Parameters Explained
- GameName (string): The name of your game.
- GameDescription (string): A brief description of what your game is about.
- GameTickSpeed (float): The speed at which your game updates, in seconds.
- RunGame (Action<string> setScreen): A method that contains the game logic, called every game tick. Use setScreen to update the terminal display.
- StartGame (Action): A method called when your game starts. Use this to initialize game state.
- HandleKey (Action<KeyCode> keyCode): A method to process keyboard input. The keyCode parameter represents the key that was pressed.

# Additional Resources
For more detailed examples and templates, visit our GitHub repository. This is a great place to start if you're looking to develop your own game or contribute to the library.
