using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GlobalAudioController : MonoBehaviour
{
    public static GlobalAudioController Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sliders (se asignan desde el Inspector)")]
    [SerializeField] private Slider[] musicSliders;
    [SerializeField] private Slider[] sfxSliders;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip paddleBounceClip;
    [SerializeField] private AudioClip wallBounceClip;
    [SerializeField] private AudioClip powerUpClip;
    [SerializeField] private AudioClip brickBreakClip;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inicializar sliders con valores actuales
        if (mixer.GetFloat("MUSICVolume", out float musicDb))
        {
            float value = Mathf.InverseLerp(-80, 0, musicDb);
            foreach (var s in musicSliders) s.value = value;
        }

        if (mixer.GetFloat("SFXVolume", out float sfxDb))
        {
            float value = Mathf.InverseLerp(-80, 0, sfxDb);
            foreach (var s in sfxSliders) s.value = value;
        }

        // Listeners
        foreach (var slider in musicSliders)
            slider.onValueChanged.AddListener(SetMusicVolumeFromSlider);

        foreach (var slider in sfxSliders)
            slider.onValueChanged.AddListener(SetSFXVolumeFromSlider);
    }

    public void SetMusicVolumeFromSlider(float sliderValue)
    {
        float db = Mathf.Lerp(-80f, 0f, sliderValue);
        mixer.SetFloat("MUSICVolume", db);
    }

    public void SetSFXVolumeFromSlider(float sliderValue)
    {
        float db = Mathf.Lerp(-80f, 0f, sliderValue);
        mixer.SetFloat("SFXVolume", db);
    }

    public bool TryGetMixerVolume(string parameter, out float db)
    {
        return mixer.GetFloat(parameter, out db);
    }

    public void UnregisterSliders(Slider musicSlider, Slider sfxSlider)
    {
        musicSlider.onValueChanged.RemoveListener(SetMusicVolumeFromSlider);
        sfxSlider.onValueChanged.RemoveListener(SetSFXVolumeFromSlider);
    }

    // Sonidos puntuales
    public void PlayPaddleBounce()
    {
        if (paddleBounceClip != null)
            sfxSource.PlayOneShot(paddleBounceClip);
    }
    public void PlayBrickBreak()
    {
        if (brickBreakClip != null)
            sfxSource.PlayOneShot(brickBreakClip);
    }
    public void PlayWallBounce()
    {
        if (wallBounceClip != null)
            sfxSource.PlayOneShot(wallBounceClip);
    }

    public void PlayPowerUp()
    {
        if (powerUpClip != null)
            sfxSource.PlayOneShot(powerUpClip);
    }
}
