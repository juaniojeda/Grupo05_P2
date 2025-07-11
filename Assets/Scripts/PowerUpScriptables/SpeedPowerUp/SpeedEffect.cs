using UnityEngine;

[CreateAssetMenu(
  fileName = "NewSpeedEffect",
  menuName = "PowerUps/Effects/Speed Effect"
)]
public class SpeedEffect : PowerUpEffect
{
    [Header("Buff Settings")]
    [Tooltip("Cuánto multiplicar la velocidad de la pala")]
    public float speedMultiplier = 2f;

    [Tooltip("Duración del boost en segundos")]
    public float duration = 5f;

    public override void Activate(PowerUpContext ctx)
    {
        // Aquí arrancaremos el controlador que aplica y revierte el buff
        new SpeedBuffController(
            ctx.gameManager,
            ctx.gameManager.PaddleController,  // necesitarás exponer este getter
            speedMultiplier,
            duration
        );
    }
}
