using Godot;
using Godot.Collections;
using System;

public partial class PlayerPickaxe : Node2D
{
	[Export] private float miningSpeed = 1f;
	[Export] private GpuParticles2D mineParticles;
	[Export] private AudioStreamPlayer breakSFX;
	[Export] private PackedScene breakVFX;

	public Dictionary inventory = new Dictionary{};

	private TileMap tileMap; // Add your TileMap node to a "TileMap" group
	private WorldGenerator worldGenerator; // Add your WorldGenerator node to a "WorldGenerator" group

	private CharacterBody2D player; // Add your Player node to a "Player" group
	private ShapeCast2D shapeCast;

	public override void _Ready()
	{
		tileMap = GetTree().GetFirstNodeInGroup("TileMap") as TileMap;
		worldGenerator = GetTree().GetFirstNodeInGroup("WorldGenerator") as WorldGenerator;

		player = GetTree().GetFirstNodeInGroup("Player") as CharacterBody2D;
		shapeCast = GetNode<ShapeCast2D>("ShapeCast2D");
		shapeCast.AddException(player);
	}

	public override void _Process(double delta)
	{
		RotateToMouse();

		// Mining particles
		if (mineParticles != null)
		{
			if (Input.IsActionPressed("mine") && shapeCast.IsColliding()) mineParticles.Emitting = true;
			else mineParticles.Emitting = false;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        // Mining
		if (Input.IsActionPressed("mine") && shapeCast.IsColliding())
		{
			// Get tiles
			var cellsToMine = new Array<Vector2I>{};;
			Vector2I tilePos = tileMap.LocalToMap(shapeCast.GetCollisionPoint(0) + Vector2.Right.Rotated(Rotation) * 1.1f); // 1.1 units further otherwise it will mine the wrong tile
			cellsToMine.Add(tilePos);

			// Mine tiles
			if (cellsToMine != null && tileMap.GetCellTileData((int)worldGenerator.wallsConfig["Layer"], cellsToMine[0]) != null) MineTiles(cellsToMine);
		}
    }

    private void MineTiles(Array<Vector2I> cellsToMine)
	{
		Vector2I tile = cellsToMine[0];
		TileData oreTileData = tileMap.GetCellTileData((int)worldGenerator.oreConfig["Layer"], tile);
		worldGenerator.tileData["health" + tile.ToString()] = (float)worldGenerator.tileData["health" + tile.ToString()] - (float)GetPhysicsProcessDeltaTime() * miningSpeed;

		// Breaking animation
		float healthValue = (float)worldGenerator.tileData["health" + tile.ToString()];

		if ((bool)worldGenerator.breakingAnimation["Generate"])
		{
			float healthPercent = healthValue * 100f;
			int breakingLayer = (int)worldGenerator.breakingAnimation["Layer"];
			int breakingSourceID = (int)worldGenerator.breakingAnimation["SourceID"];
			Vector2I breakingState = Vector2I.Zero;

			if (healthPercent == 100f) breakingState = new Vector2I(0, 0);
			if (healthPercent <= 80f) breakingState = new Vector2I(1, 0);
			if (healthPercent <= 60f) breakingState = new Vector2I(2, 0);
			if (healthPercent <= 40f) breakingState = new Vector2I(3, 0);
			if (healthPercent <= 20f) breakingState = new Vector2I(4, 0);
			if (healthPercent <= 0f) breakingState = new Vector2I(5, 0);

			tileMap.SetCell(breakingLayer, tile, breakingSourceID, breakingState);
		}

		// Erase tile and get ore
		if (healthValue <= 0f)
		{
			// Get ore
			if (tileMap.GetCellSourceId((int)worldGenerator.oreConfig["Layer"], cellsToMine[0]) == (int)worldGenerator.oreConfig["SourceID"])
			{
				String ore = oreTileData.GetCustomData("ore").ToString();
				if (!inventory.ContainsKey(ore)) inventory.Add(ore, 0);
				inventory[ore] = (int)inventory[ore] + 1;
			}

			// Destroy the tile
			for (int layer = 0; layer < tileMap.GetLayersCount(); layer++)
			{
				if (layer != (int)worldGenerator.groundConfig["Layer"])
				{
					tileMap.SetCellsTerrainConnect(layer, cellsToMine, (int)worldGenerator.wallsConfig["TerrainSet"], -1);
					tileMap.SetCellsTerrainConnect(layer, cellsToMine, 2, -1);
				}
			}

			// Sound
			if (breakSFX != null)
			{
				var random = new Godot.RandomNumberGenerator();
				random.Randomize();
				breakSFX.PitchScale = random.RandfRange(0.9f, 1.1f);
				breakSFX.Play();
			}

			// VFX
			if (breakVFX != null)
			{
				GpuParticles2D vfx = breakVFX.Instantiate<GpuParticles2D>();
				GetTree().Root.AddChild(vfx);
				vfx.GlobalPosition = tileMap.MapToLocal(cellsToMine[0]);
				vfx.Emitting = true;
			}
		}
	}

	private void RotateToMouse()
	{
		// Rotation
		Vector2 mousePos = GetGlobalMousePosition();
		float mouseDir = (player.GlobalPosition - mousePos).Angle() + Mathf.DegToRad(180);
		GlobalRotation = mouseDir;

		// Flipping
		float rot = Mathf.RadToDeg((player.GlobalPosition - mousePos).Angle());
		if (rot >= -180 && rot <= 180) Scale = new Vector2(Scale.X, -1);
		else Scale = new Vector2(Scale.X, 1);
	}
}
