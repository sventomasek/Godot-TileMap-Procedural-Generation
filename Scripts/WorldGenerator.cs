// Visit github.com/sventomasek/Godot-TileMap-Procedural-Generation for help!
using Godot;
using Godot.Collections;
using System;

public partial class WorldGenerator : Node2D
{
	[Export] private bool generate = true;
	[ExportGroup("Map")]
	[Export] private String worldSeed; // Leave blank for random
	[Export] private Vector2I mapSize = new Vector2I(100, 100);
	[Export] private Dictionary borderSize = new Dictionary{{"Top", 5}, {"Bottom", 5}, {"Left", 5}, {"Right", 5}};

	[ExportGroup("Noise")]
	[Export] private FastNoiseLite generationNoise;
	[Export] private float thresholdMin = -1.0f;
	[Export] private float thresholdMax = 0.2f;

	[ExportGroup("Tiles")]
	[Export] public Dictionary groundConfig = new Dictionary
	{
		{"Generate", true},
		{"Layer", 0},
		{"SourceID", 0},
		{"TerrainSet", 0},
		{"Terrain", 0}
	};
	[Export] public Dictionary wallsConfig = new Dictionary
	{
		{"Generate", true},
		{"Layer", 1},
		{"SourceID", 1},
		{"TerrainSet", 1},
		{"Terrain", 0}
	};
	[Export] public Dictionary borderConfig = new Dictionary
	{
		{"Generate", true},
		{"Layer", 1},
		{"SourceID", 2},
		{"TerrainSet", 2},
		{"Terrain", 0}
	};
	[Export] public Dictionary oreConfig = new Dictionary
	{
		{"Generate", true},
		{"Layer", 2},
		{"SourceID", 10}
	};
	[Export] public Dictionary breakingAnimation = new Dictionary
	{
 		{"Generate", true},
		{"Layer", 3},
		{"SourceID", 11}
	};

	public TileMap tileMap;
	public Dictionary tileData = new Dictionary{};

	[ExportGroup("Resources")]
	// Ore at the top will be more common than the ones at the bottom, as they will not spawn on top of already generated ore
	[Export] private Array<OreData> ores;
	[Export] private Array<StructureData> structures;
	private bool generateStructures = true;

	private CharacterBody2D player; // Add your player to a group called "Player"

	public override void _Ready()
	{
		// References
		player = GetTree().GetFirstNodeInGroup("Player") as CharacterBody2D;
		tileMap = GetParent() as TileMap;
	}

    public override void _Process(double delta)
    {
        if (generate)
		{
			GenerateWorld();
			generate = false;
		}
    }

    public void GenerateWorld()
	{
		// Randomize the seed
		var random = new Godot.RandomNumberGenerator();
		random.Randomize();
		if (worldSeed == null) worldSeed = random.Randi().ToString();

		// Clear the TileMap
		tileMap.Clear();

		// Generation
		if ((bool)groundConfig["Generate"]) GenerateGround();
		if ((bool)wallsConfig["Generate"]) GenerateWalls();
		if ((bool)borderConfig["Generate"]) GenerateBorder();
		if ((bool)oreConfig["Generate"] && ores.Count > 0) GenerateOre();
		if (generateStructures && structures.Count > 0) GenerateStructures();
		AddCustomData();

		// Delete cells on player
		var cellsToDelete = tileMap.GetSurroundingCells(tileMap.LocalToMap(player.GlobalPosition));
		cellsToDelete.Add(tileMap.LocalToMap(player.GlobalPosition));
		for (int layer = 0; layer < tileMap.GetLayersCount(); layer++)
		{
			if (layer != (int)groundConfig["Layer"]) tileMap.SetCellsTerrainConnect(layer, cellsToDelete, (int)wallsConfig["TerrainSet"], -1);
		}

		// Print that the generation is complete
		GD.PrintRich("[color=ROSY_BROWN][" + "[color=GRAY]" + Name + "[color=ROSY_BROWN]] " + "Finished generating terrain in " + (Time.GetTicksMsec() / 1000f) + " seconds with seed " + worldSeed);
	}

	private FastNoiseLite GenerateNoise(int noiseSeed, float frequency = 0f)
	{
		FastNoiseLite noise;
		noise = generationNoise;
		noise.Seed = noiseSeed;
		if (frequency != 0f) noise.Frequency = frequency;
		return noise;
	}

	private void GenerateGround()
	{
		var cells = new Array<Vector2I>{};
		foreach (int x in GD.Range(-mapSize.X / 2, mapSize.X / 2))
		{
			foreach (int y in GD.Range(-mapSize.Y / 2, mapSize.Y / 2))
			{
				cells.Add(new Vector2I(x, y));
			}
		}

		tileMap.SetCellsTerrainConnect((int)groundConfig["Layer"], cells, (int)groundConfig["TerrainSet"], (int)groundConfig["Terrain"], false);
	}

	private void GenerateWalls()
	{
		FastNoiseLite noise = GenerateNoise((int)worldSeed.Hash());
		var cells = new Array<Vector2I>{};
		foreach (int x in GD.Range(-mapSize.X / 2, mapSize.X / 2))
		{
			foreach (int y in GD.Range(-mapSize.Y / 2, mapSize.Y / 2))
			{
				float lightness = noise.GetNoise2D(x, y);
				if (lightness > thresholdMin && lightness < thresholdMax) cells.Add(new Vector2I(x, y));
			}
		}

		tileMap.SetCellsTerrainConnect((int)wallsConfig["Layer"], cells, (int)wallsConfig["TerrainSet"], (int)wallsConfig["Terrain"], false);
	}

	private void GenerateBorder()
	{
		var cells = new Array<Vector2I>{};
		for (int xSize = 0; xSize < mapSize.X + (int)borderSize["Top"] * 2; xSize++)
		{
			for (int ySize = 0; ySize < (int)borderSize["Top"]; ySize++)
			{
				cells.Add(new Vector2I(xSize - mapSize.X / 2 - (int)borderSize["Top"], -(mapSize.Y / 2 + ySize + 1)));
			}
		}

		for (int xSize = 0; xSize < mapSize.X + (int)borderSize["Bottom"] * 2; xSize++)
		{
			for (int ySize = 0; ySize < (int)borderSize["Bottom"]; ySize++)
			{
				cells.Add(new Vector2I(xSize - mapSize.X / 2 - (int)borderSize["Bottom"], mapSize.Y / 2 + ySize));
			}
		}

		for (int ySize = 0; ySize < mapSize.Y + (int)borderSize["Left"] * 2; ySize++)
		{
			for (int xSize = 0; xSize < (int)borderSize["Left"]; xSize++)
			{
				cells.Add(new Vector2I(-(mapSize.X / 2 + xSize + 1), ySize - mapSize.Y / 2 - (int)borderSize["Left"]));
			}
		}

		for (int ySize = 0; ySize < mapSize.Y + (int)borderSize["Right"] * 2; ySize++)
		{
			for (int xSize = 0; xSize < (int)borderSize["Right"]; xSize++)
			{
				cells.Add(new Vector2I(mapSize.X / 2 + xSize, ySize - mapSize.Y / 2 - (int)borderSize["Right"]));
			}
		}

		tileMap.SetCellsTerrainConnect((int)borderConfig["Layer"], cells, (int)borderConfig["TerrainSet"], (int)borderConfig["Terrain"], false);
	}

	private void GenerateOre()
	{
		int offset = 0; // Seed offset for the noise texture, so we get a different noise for every ore and structure
		int i = 0;
		foreach (OreData ore in ores)
		{
			FastNoiseLite noise = GenerateNoise((int)worldSeed.Hash() + i + offset, ore.frequency);
			foreach (int x in GD.Range(-mapSize.X / 2, mapSize.X / 2))
			{
				foreach (int y in GD.Range(-mapSize.Y / 2, mapSize.Y / 2))
				{
					float lightness = noise.GetNoise2D(x, y);
					if (lightness > ore.thresholdMin && lightness < ore.thresholdMax)
					{
						// Check if the tile is a wall tile, and not an already generated ore
						int cellID = tileMap.GetCellSourceId((int)wallsConfig["Layer"], new Vector2I(x, y));
						if (cellID == (int)wallsConfig["SourceID"] && cellID != (int)oreConfig["SourceID"])
						{
							// Generate ore
							tileMap.SetCell((int)oreConfig["Layer"], new Vector2I(x, y), (int)oreConfig["SourceID"], ore.atlasCoords);
						}
					}
				}
			}
			i += 1;
		}
	}

	private void GenerateStructures()
	{
		int offset = 1;
		int i = 0;
		foreach (StructureData structure in structures)
		{
			FastNoiseLite noise = GenerateNoise((int)worldSeed.Hash() + i + offset, structure.frequency);
			foreach (int x in GD.Range(-mapSize.X / 2, mapSize.X / 2))
			{
				foreach (int y in GD.Range(-mapSize.Y / 2, mapSize.Y / 2))
				{
					float lightness = noise.GetNoise2D(x, y);
					if (lightness > structure.thresholdMin && lightness < structure.thresholdMax)
					{
						// Decide if the structure should be spawned (seed independent)
						bool shouldSpawn = GD.RandRange(0f, 1f) < structure.spawnChance;

						// Generate structure
						if (shouldSpawn)
						{
							TileMap structureTileMap = structure.tileMapScene.Instantiate() as TileMap;

							// Generate tiles
							for (int layer = 0; layer < structureTileMap.GetLayersCount(); layer++)
							{
								foreach (Vector2I cell in structureTileMap.GetUsedCells(layer))
								{
									int sourceID = structureTileMap.GetCellSourceId(layer, cell);
									Vector2I atlasCoords = structureTileMap.GetCellAtlasCoords(layer, cell);
									Vector2I cellPos = new Vector2I(x, y) + cell;

									// Check if the tile is outside the map
									if (Mathf.Abs(cellPos.X) < mapSize.X / 2 && Mathf.Abs(cellPos.Y) < mapSize.Y / 2)
									{
										tileMap.SetCell(layer, cellPos, sourceID, atlasCoords); // Place structure tiles
										tileMap.EraseCell((int)oreConfig["Layer"], cellPos); // Erase ore tiles that generated on structures
									}
								}
							}

							// Add children nodes to the scene
							foreach (Node2D child in structureTileMap.GetChildren())
							{
								// Check if the tile is outside the map
								int tileSize = structureTileMap.RenderingQuadrantSize;
								Vector2 pos = structureTileMap.MapToLocal(new Vector2I(x, y)) + child.Position - new Vector2(tileSize / 2, tileSize / 2);
								if (Mathf.Abs(tileMap.LocalToMap(pos).X) < mapSize.X / 2 && Mathf.Abs(tileMap.LocalToMap(pos).Y) < mapSize.Y / 2)
								{
									// Generate
									structureTileMap.RemoveChild(child);
									child.Owner = null;
									GetTree().Root.AddChild(child);
									child.GlobalPosition = pos;
								}
							}

							// Delete the scene
							structureTileMap.QueueFree();
						}

					}
				}
			}
		}
	}

	private void AddCustomData()
	{
		// Add health data for every generated wall tile
		foreach (Vector2I tile in tileMap.GetUsedCells((int)wallsConfig["Layer"]))
		{
			tileData["health" + tile.ToString()] = 1.0;
		}
	}
}
