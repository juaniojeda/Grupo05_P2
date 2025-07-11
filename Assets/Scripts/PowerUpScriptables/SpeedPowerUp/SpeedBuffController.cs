using YourGame.Utilities;  // para ICustomUpdate

public class SpeedBuffController : ICustomUpdate
{
    private readonly PaddleController _paddle;
    private readonly float _originalSpeed;
    private readonly float _duration;
    private float _elapsedTime;

    /// <summary>
    /// Aplica el buff multiplicando la velocidad y, tras duration segundos, la restaura.
    /// </summary>
    public SpeedBuffController(
        GameManager gm,
        PaddleController paddle,
        float multiplier,
        float duration)
    {
        _paddle = paddle;
        _originalSpeed = paddle.Speed;
        _paddle.Speed = _originalSpeed * multiplier;
        _duration = duration;
        _elapsedTime = 0f;

        gm.Register(this);
    }

    public void CustomUpdate(float deltaTime)
    {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= _duration)
        {
            // Tiempo cumplido: restauramos velocidad y nos desregistramos
            _paddle.Speed = _originalSpeed;
            GameManager.Instance.Unregister(this);
        }
    }
}
