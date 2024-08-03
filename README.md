# Godot TileMap Procedural Generation
Procedural Generation which features: ores, structures and a world border.

Written in C# so you need to have Godot .NET installed for it to work.

# How To Install
1. Place all of the scripts into your Godot project
2. Add a Node2D to your main TileMap node
3. Add WorldGenerator.cs to the Node2D
4. Right click on the Generation Noise variable, pick FastNoiseLite and configure it to your liking
5. Configure everything under the Tiles category (for example Ground Layer is your TileMaps ground layer, same for other variables. If you don't have a type of tiles just turn off "Generate")
6. Run the game and it should work if you've configured it correctly.

# How To Add Ore
1. Create a folder where you will store your ore resources
2. Make a new OreData Resource (Right click > New Resource > OreData)
3. Configure your ore
   - Atlas coords is the coordinates of the ore in the TileMap image
   - Lower frequency makes them spawn in groups more
   - Threshold is how often it will spawn
4. Add your ore to the WorldGenerator Node under the Ores section (ore at the top will spawn more frequently)
5. Done

# How To Add Structures (or spawn other nodes)
1. Create a folder where you will store your structure resources
2. Make a new StructureData Resource (Right click > New Resource > StructureData)
3. Configure your structure
   - Tile Map Scene should be a PackedScene with a TileMap as the root node (draw your structure in that scene, all of the child nodes like CharacterBody2D for example will also spawn with the structure)
   - Spawn Chance is seed independent
   - Frequency is how close together they will spawn (lower is closer)
   - Threshold is how often it will spawn
4. Add your structure to the WorldGenerator Node under the Structures section

# Need Help?
You can contact me in my Discord server https://discord.gg/MsF7kN54T7

Just post your issue in the "tech-support" channel and I will try to help as soon as I can.
