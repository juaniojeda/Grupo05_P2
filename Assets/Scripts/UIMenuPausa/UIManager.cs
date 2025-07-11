// UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YourGame.Utilities;  // Para ICustomUpdate, ITimeProvider y demás

public class UIManager : MonoBehaviour
{
    [Header("Pausa")]
    public Canvas pauseCanvas;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Temporizador")]
    public TextMeshProUGUI timerText;

    [Header("Ladrillos")]
    public TextMeshProUGUI brickCounterText;

    [Header("Rebotes en Paleta")]
    public TextMeshProUGUI paddleBounceText;

    [Header("Vidas")]
    public TextMeshProUGUI lifeText;

    private PauseMenuSystem _pauseMenu;
    private UITimerDisplay _timerDisplay;
    private UIBrickCounterDisplay _brickCounterDisplay;
    private UIPaddleBounceDisplay _paddleBounceDisplay;
    private LifeSystem _lifeSystem;

    public void Initialize(ITimeProvider timeProvider, BallController mainBall)
    {
        // 1) Desmontar cualquier PauseMenu previo
        _pauseMenu?.Dispose();

        // 2) Asegurar que el juego no arranque pausado
        Time.timeScale = 1f;

        // 3) Crear sistemas limpios
        _pauseMenu = new PauseMenuSystem(pauseCanvas, musicSlider, sfxSlider);
        _timerDisplay = new UITimerDisplay(timerText, timeProvider);
        _brickCounterDisplay = new UIBrickCounterDisplay(brickCounterText);
        _paddleBounceDisplay = new UIPaddleBounceDisplay(paddleBounceText, mainBall);
        _lifeSystem = new LifeSystem(3, lifeText);
        _lifeSystem.SetMainBall(mainBall);
        LifeSystemAccess.Register(_lifeSystem);
    }

    private void OnDestroy()
    {
        _pauseMenu?.Dispose();
        _timerDisplay?.Dispose();
        _brickCounterDisplay?.Dispose();
        _paddleBounceDisplay?.Dispose();
        _lifeSystem?.Dispose();
        LifeSystemAccess.Clear();
    }
}
