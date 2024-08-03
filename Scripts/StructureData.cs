using Godot;

[GlobalClass]
public partial class StructureData : Resource
{
	[Export] public string name = "";
	[Export] public PackedScene tileMapScene; // PackedScene of the structure, the root node of the scene should be a TileMap
	[Export(PropertyHint.Range, "0, 1, 0.01")] public float spawnChance = 1.0f; // Chance for the structure to spawn (seed independent)
	[Export] public float frequency = 2.0f; // Scale of the noise texture, lower value means the structures will spawn close together more frequent
	[Export] public float thresholdMin = -1.0f; // Required Lightness on the Noise Texture for the structure to spawn
	[Export] public float thresholdMax = -0.92f; // ^
}
