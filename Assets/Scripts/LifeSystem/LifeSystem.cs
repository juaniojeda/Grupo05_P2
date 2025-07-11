using UnityEngine;
using UnityEngine.SceneManagement;
using YourGame.Utilities;  // ICustomUpdate + IDisposable
using TMPro;

public class LifeSystem : ICustomUpdate, IDisposable
{
    private float _lives;
    private TextMeshProUGUI _lifeText;
    private BallController _mainBall;

  
    public LifeSystem(float initialLives, TextMeshProUGUI lifeText)
    {
        _lives = initialLives;
        _lifeText = lifeText;

        // Registro en el bucle de updates de GameManager
        GameManager.Instance.Register(this);
        UpdateUI();
    }


    public void SetMainBall(BallController ball)
    {
        _mainBall = ball;
    }

 
    public void LoseLife()
    {
        _lives--;

        if (_lives <= 0)
        {
            Debug.Log("Game Over — sin vidas.");
            SceneManager.LoadScene("GameOverScreen");
        }
        else
        {
            Debug.Log($"Vida perdida. Quedan: {_lives}");

            // Resetea la bola principal sin recargar escena
            _mainBall?.ResetOnPaddle();

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (_lifeText != null)
            _lifeText.text = $"Lives: {_lives}";
    }

  
    public void CustomUpdate(float deltaTime)
    {
        // Intencionadamente vacío
    }


    public void Dispose()
    {
        GameManager.Instance.Unregister(this);
    }
}

