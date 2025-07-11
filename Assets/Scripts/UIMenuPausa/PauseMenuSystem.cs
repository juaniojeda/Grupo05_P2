// PauseMenuSystem.cs
using UnityEngine;
using UnityEngine.UI;
using YourGame.Utilities;  // para ICustomUpdate e IDisposable

public class PauseMenuSystem : ICustomUpdate, IDisposable, IPauseMenu
{
    private readonly Canvas _pauseCanvas;
    private readonly Slider _musicSlider;
    private readonly Slider _sfxSlider;

    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    public PauseMenuSystem(Canvas pauseCanvas, Slider musicSlider, Slider sfxSlider)
    {
        Debug.Log("[PauseMenu] Constructor", _pauseCanvas);
        Time.timeScale = 1f;
        // Siempre arrancar con el juego en marcha

        _pauseCanvas = pauseCanvas;
        _musicSlider = musicSlider;
        _sfxSlider = sfxSlider;

        // Inicio oculto
        _pauseCanvas.enabled = false;

        // Inicializar valores de los sliders desde GameManager
        if (GameManager.Instance.TryGetMixerVolume("MUSICVolume", out float musicDb))
            _musicSlider.value = Mathf.InverseLerp(-80f, 0f, musicDb);

        if (GameManager.Instance.TryGetMixerVolume("SFXVolume", out float sfxDb))
            _sfxSlider.value = Mathf.InverseLerp(-80f, 0f, sfxDb);

        // Conectar sliders a los métodos de GameManager
        _musicSlider.onValueChanged.AddListener(GameManager.Instance.SetMusicVolumeFromSlider);
        _sfxSlider.onValueChanged.AddListener(GameManager.Instance.SetSFXVolumeFromSlider);

        // Registrarnos para recibir CustomUpdate
        GameManager.Instance.Register(this);
    }

    public void CustomUpdate(float deltaTime)
    {
        // Alternar pausa con Escape
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        _pauseCanvas.enabled = _isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void Dispose()
    {
        Debug.Log("[PauseMenu] Dispose", _pauseCanvas);
        GameManager.Instance.Unregister(this);
        _musicSlider.onValueChanged.RemoveListener(GameManager.Instance.SetMusicVolumeFromSlider);
        _sfxSlider.onValueChanged.RemoveListener(GameManager.Instance.SetSFXVolumeFromSlider);
    }
}
