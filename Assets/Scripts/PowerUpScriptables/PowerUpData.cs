// PowerUpData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/PowerUp Data")]
public class PowerUpData : ScriptableObject
{
    [Header("Visual")]
    public GameObject visualPrefab;

    [Header("Falling")]
    public float fallSpeed = 2f;

    [Tooltip("Si la posición Y del Power-Up baja de este valor, se destruye.")]
    public float destroyY = -10f;

    [Header("Efecto")]
    public PowerUpEffect effect;
}
