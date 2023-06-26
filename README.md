# Founder's Minecraft Utils

FMU, which stands for Founder's Minecraft Utils or Functions for Minecraft Utilities, is a C# library that provides a collection of asynchronous functions to assist in creating external programs for Minecraft Bedrock Edition (MCBE). It allows developers to interact with and manipulate game settings and data stored in the local Minecraft directory.

## Features
* **Retrieve Game Options**: Extract specific game settings from the options.txt file.
* **Read Data from Files**: Read data from files within the Minecraft directory.
* **Modify Game Options**: Programmatically modify game settings within the options.txt file.
* **Unlock FPS**: Customize game settings to unlock the frame rate for improved performance.
* **Retrieve Player Information**: Extract information such as the player's username, language setting, ClientID, and DeviceID.

## Example Usage
```csharp
using FMU;

// Retrieve the player's username
string username = await MinecraftAsync.GetMPUsername();

// Retrieve the player's language setting
string language = await MinecraftAsync.GetLanguage();

// Unlock the player's FPS
bool success = await MinecraftAsync.UnlockFPS();
```

## Getting Started

To use FMU in your project, simply include the **FMU** namespace and make use of the static class **MinecraftAsync**, which contains various asynchronous functions for interacting with Minecraft Bedrock Edition. Alternatively, if you prefer synchronous operations, you can use the **MinecraftSync** class.

## Contributing

Contributions are welcome! Feel free to open issues for feature requests, bug reports, or submit pull requests.