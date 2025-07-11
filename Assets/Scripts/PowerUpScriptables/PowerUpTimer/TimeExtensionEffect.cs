using System.Collections;
using System.Collections.Generic;
// Assets/Scripts/PowerUpScriptables/TimeExtensionEffect.cs
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/TimeExtensionEffect")]
public class TimeExtensionEffect : PowerUpEffect
{
    // Este campo lo vincularemos desde el SO de datos
    public float extraTime;

    public override void Activate(PowerUpContext ctx)
    {
        // Llamamos al GameManager para extender el tiempo
        GameManager.Instance.AddTime(extraTime);
        // Opcional: reproducir sonido de PowerUp
        GameManager.Instance.PlayPowerUp();
    }
}
