# Godot TileMap Procedural Generation
Procedural Generation written in C# which uses Noise textures to generate a random terrain.

Ore and structures can be added to the terrain using custom resources.
Structures can be drawn on a TileMap in a new Scene, the child Node2Ds will also generate with the structure (e.g., enemies and items).

Also comes with a script that adds the ability to mine the tiles and add the ore to your inventory.
All you have to do is add a Custom Data Layer "ore" to the TileMap and assign it to your ore tile with the name of your ore.

Here's an example of what I made using this system:

<p float="left">
   <img src="https://raw.githubusercontent.com/sventomasek/Godot-TileMap-Procedural-Generation/main/Images/Example1.png" width="400" />
   <img src="https://raw.githubusercontent.com/sventomasek/Godot-TileMap-Procedural-Generation/main/Images/Example2.gif" width="400" />
</p>

# How To Install
1. Place all of the scripts into your Godot project
2. Add your Player Node to a group called "Player"
3. Add a Node2D "WorldGenerator" to your main TileMap node
4. Add WorldGenerator.cs to the Node2D
5. Right click on the Generation Noise variable, pick FastNoiseLite and configure it to your liking
6. Configure everything under the Tiles category (for example Ground Layer is your TileMaps ground layer, same for other variables. If you don't have a type of tiles just turn off "Generate")
7. Run the game and it should work if you've configured it correctly.

# How To Add Ore
1. Create a folder where you will store your ore resources
2. Make a new OreData Resource (Right click > New Resource > OreData)
3. Configure your ore
   - Atlas coords is the coordinates of the ore in the TileMap image
   - Lower frequency makes them spawn in groups more
   - Threshold is how often it will spawn
4. Add your ore to the WorldGenerator Node under the Ores section (ore at the top will spawn more frequently)

# How To Add Structures (or spawn other nodes)
1. Create a folder where you will store your structure resources
2. Make a new StructureData Resource (Right click > New Resource > StructureData)
3. Configure your structure
   - Tile Map Scene should be a PackedScene with a TileMap as the root node (draw your structure in that scene, all of the child nodes like CharacterBody2D for example will also spawn with the structure)
   - Spawn Chance is seed independent
   - Frequency is how close together they will spawn (lower is closer)
   - Threshold is how often it will spawn
4. Add your structure to the WorldGenerator Node under the Structures section

# How To Mine Tiles
1. Add your TileMap Node to a group called "TileMap"
2. Add the WorldGenerator Node to a group called "WorldGenerator"
1. Add a Node2D "Pickaxe" to your Player
2. Attach the PlayerPickaxe.cs script to it
3. Add a ShapeCast2D as a child of the Pickaxe node
4. You can now mine the tiles

# How To Add Ore To Your Inventory
The PlayerPickaxe.cs script contains an inventory dictionary.
1. Select your TileMap, open your TileSet and add a Custom Data Layer "ore" with Type "String"
2. With your TileMap selected open the TileSet tab (at the bottom of Godot where Output, Debugger, etc is located)
3. Select your Ore Tiles (make sure to be using Select instead of Setup), click on any tile and under "Custom Data" write the name of your ore
4. Now ores will be added to your inventory in PlayerPickaxe.cs
5. To see them you can write GD.Print(inventory); in Process function of the script

# Need Help?
You can contact me in my Discord server https://discord.gg/MsF7kN54T7

Just post your issue in the "tech-support" channel and I will try to help as soon as I can.
