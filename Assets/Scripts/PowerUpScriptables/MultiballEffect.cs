// MultiballEffect.cs
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Multiball", fileName = "New Multiball Effect")]
public class MultiballEffect : PowerUpEffect
{
    [Header("Multiball Settings")]
    [Tooltip("Número de bolas extra a generar")]
    public int count = 2;
    [Tooltip("Ángulo total del abanico en grados")]
    public float spreadAngle = 45f;
    [Tooltip("Altura relativa sobre la pala donde nacen las bolas")]
    public float spawnHeight = 0.6f;

    public override void Activate(PowerUpContext ctx)
    {
        // Posición base justo encima de la pala
        Vector3 basePos = ctx.paddle.position + Vector3.up * spawnHeight;

        // Calcula la mitad del abanico para distribuir ángulos
        float halfAngle = spreadAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            // 't' va de 0 a 1 para interpolar ángulo
            float t = (count == 1) ? 0.5f : i / (float)(count - 1);
            float angleDeg = -halfAngle + t * spreadAngle;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // Dirección en el plano XY
            Vector3 dir = new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad), 0f);

            // Lanza la bola extra
            ctx.gameManager.SpawnExtraBall(basePos, dir);
        }
    }
}