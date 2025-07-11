using UnityEngine;

[CreateAssetMenu(fileName = "BallData", menuName = "Game/Data/BallData")]
public class BallData : ScriptableObject
{
    [Header("Pool Settings")]
    public int poolSize = 10;
    public GameObject ballPrefab;

    [Header("Movement")]
    public float speed = 5f;

    [Header("Bounds")]
    public float leftLimit = -8f;
    public float rightLimit = 8f;
    public float topLimit = 4.5f;
    public float bottomLimit = -4f;
}
