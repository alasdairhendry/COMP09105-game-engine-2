using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour {

    public static Settings Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject); 
    }

    private void Start()
    {
        LoadSettings();
    }

    [SerializeField] private AudioMixer audioMixer;

    private const string MASTER_VOLUME = "masterV";
    private const string MUSIC_VOLUME = "musicV";
    private const string SFX_VOLUME = "sfxV";

    //----------------------------------------------------------

    private float masterVolume = 1.0f;
    public float MasterVolume { get { return masterVolume; } set { OnSet_MasterVolume(value); masterVolume = value; } }

    private float musicVolume = 1.0f;
    public float MusicVolume { get { return musicVolume; } set { OnSet_MusicVolume(value); musicVolume = value; } }

    private float sfxVolume = 1.0f;
    public float SfxVolume { get { return sfxVolume; } set { OnSet_SfxVolume(value); sfxVolume = value; } }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(MASTER_VOLUME)) MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME);
        else MasterVolume = 1.0f;

        if (PlayerPrefs.HasKey(MUSIC_VOLUME)) MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        else MusicVolume = 1.0f;

        if (PlayerPrefs.HasKey(SFX_VOLUME)) SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME);
        else SfxVolume = 1.0f;
    }

    private void OnSet_MasterVolume(float value)
    {
        //Debug.Log("Boop1232: " + value);
        audioMixer.SetFloat("MasterVol", Mathf.Lerp(-80.0f, 0.0f, value));
        PlayerPrefs.SetFloat(MASTER_VOLUME, value);
    }

    private void OnSet_MusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Lerp(-80.0f, -20.0f, value));
        PlayerPrefs.SetFloat(MUSIC_VOLUME, value);
    }

    private void OnSet_SfxVolume(float value)
    {
        audioMixer.SetFloat("SfxVol", Mathf.Lerp(-80.0f, 10.0f, value));
        PlayerPrefs.SetFloat(SFX_VOLUME, value);
    }
}
