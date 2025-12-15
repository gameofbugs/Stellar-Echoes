// ============================================
// AudioManager.cs (Optimized)
// ============================================

using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmAudioSource;
    public AudioSource sfxSource;
    public AudioSource buttonSfxSource;

    [Header("Audio Clips")]
    public AudioClip bgm;
    public AudioClip damage;
    public AudioClip fire;
    public AudioClip buttonClick;
    public AudioClip powerUp;
    public AudioClip death;

    [Header("UI Sliders")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    // OPTIMIZED: PlayerPrefs keys as constants
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        // OPTIMIZED: Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Start()
    {
        // OPTIMIZED: Initialize BGM
        if (bgmAudioSource != null && bgm != null)
        {
            bgmAudioSource.clip = bgm;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }

        LoadVolumeSettings();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void ButtonSfx()
    {
        if (buttonSfxSource != null && buttonClick != null)
        {
            buttonSfxSource.PlayOneShot(buttonClick); // OPTIMIZED: Use buttonSfxSource, not bgm
        }
    }

    public void SetBGMVolume()
    {
        if (bgmAudioSource == null || bgmSlider == null) return;

        bgmAudioSource.volume = bgmSlider.value;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmSlider.value);
        PlayerPrefs.Save(); // OPTIMIZED: Explicit save
    }

    public void SetSFXVolume()
    {
        if (sfxSlider == null) return;

        float volume = sfxSlider.value;

        if (sfxSource != null) sfxSource.volume = volume;
        if (buttonSfxSource != null) buttonSfxSource.volume = volume;

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save(); // OPTIMIZED: Explicit save
    }

    public void LoadVolumeSettings()
    {
        // OPTIMIZED: Load BGM volume
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume;
        }
        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVolume;
        }

        // OPTIMIZED: Load SFX volume
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (buttonSfxSource != null) buttonSfxSource.volume = sfxVolume;
        if (sfxSlider != null) sfxSlider.value = sfxVolume;
    }
}
