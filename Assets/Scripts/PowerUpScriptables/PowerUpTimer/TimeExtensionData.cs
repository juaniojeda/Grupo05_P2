// Assets/Scripts/PowerUpScriptables/TimeExtensionData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/TimeExtension")]
public class TimeExtensionData : PowerUpData
{
    [Tooltip("Segundos que suma al temporizador")]
    public float extraTime = 15f;
}
