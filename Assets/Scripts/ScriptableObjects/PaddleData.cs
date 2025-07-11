using UnityEngine;

[CreateAssetMenu(fileName = "PaddleData", menuName = "Game/Data/PaddleData")]
public class PaddleData : ScriptableObject
{
    [Header("Movimiento")]
    public float speed = 10f;

    [Header("Dimensiones")]
    public float width = 4f;
    public float height = 1f;

    [Header("Límites X")]
    public float xLimit = 8f;
}