using Godot;

[GlobalClass]
public partial class OreData : Resource
{
	[Export] public string name = "";
	[Export] public Vector2I atlasCoords; // Coordinates of the Tile on the TileMap Atlas
	[Export] public float frequency = 0.1f; // Scale of the noise texture
	[Export] public float thresholdMin = -1.0f; // Required Lightness on the Noise Texture for the ore to spawn
	[Export] public float thresholdMax = -0.85f; // ^
}
