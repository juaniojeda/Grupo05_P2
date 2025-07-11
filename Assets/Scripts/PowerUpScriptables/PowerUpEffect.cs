// PowerUpEffect.cs
using UnityEngine;

public abstract class PowerUpEffect : ScriptableObject
{
    public string powerUpName;
    public Sprite icon;

    /// <summary>
    /// Se llama al tocar pala. 
    /// </summary>
    public abstract void Activate(PowerUpContext ctx);
}
