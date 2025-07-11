using UnityEngine;

[CreateAssetMenu(fileName = "BrickGeneratorConfig", menuName = "Config/Brick Generator")]
public class BrickGeneratorConfig : ScriptableObject
{
    [Header("Grid Settings")]
    public int rows = 3;
    public int columns = 5;
    public float spacing = 0.1f;

    [Header("Brick Prefabs (elige uno al azar)")]
    public GameObject[] brickPrefabs;

    [Header("Brick Materials (elige uno al azar)")]
    public Material[] materials;
}
