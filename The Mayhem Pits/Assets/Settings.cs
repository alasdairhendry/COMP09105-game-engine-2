using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

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
    [SerializeField] private PostProcessProfile[] processProfiles;

    private const string MASTER_VOLUME = "masterV";
    private const string MUSIC_VOLUME = "musicV";
    private const string SFX_VOLUME = "sfxV";

    private const string BLOOM = "bloom";
    private const string VSYNC = "vsync";
    private const string AMBIENTOCCLUSION = "ao";

    //----------------------------------------------------------

    private float masterVolume = 1.0f;
    public float MasterVolume { get { return masterVolume; } set { OnSet_MasterVolume(value); masterVolume = value; } }

    private float musicVolume = 1.0f;
    public float MusicVolume { get { return musicVolume; } set { OnSet_MusicVolume(value); musicVolume = value; } }

    private float sfxVolume = 1.0f;
    public float SfxVolume { get { return sfxVolume; } set { OnSet_SfxVolume(value); sfxVolume = value; } }

    private bool bloom = true;
    public bool Bloom { get { return bloom; } set { OnSet_Bloom(value); bloom = value; } }

    private bool vSync = true;
    public bool VSync { get { return vSync; } set { OnSet_VSync(value); vSync = value; } }

    private bool ambientOcclusion = true;
    public bool AmbientOcclusion { get { return ambientOcclusion; } set { OnSet_AmbientOcclusion(value); ambientOcclusion = value; } }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(MASTER_VOLUME)) MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME);
        else MasterVolume = 1.0f;

        if (PlayerPrefs.HasKey(MUSIC_VOLUME)) MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        else MusicVolume = 1.0f;

        if (PlayerPrefs.HasKey(SFX_VOLUME)) SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME);
        else SfxVolume = 1.0f;

        if (PlayerPrefs.HasKey(BLOOM)) Bloom = bool.Parse(PlayerPrefs.GetString(BLOOM));
        else Bloom = true;

        if (PlayerPrefs.HasKey(VSYNC)) VSync = bool.Parse(PlayerPrefs.GetString(VSYNC));
        else VSync = true;

        if (PlayerPrefs.HasKey(AMBIENTOCCLUSION)) AmbientOcclusion = bool.Parse(PlayerPrefs.GetString(AMBIENTOCCLUSION));
        else AmbientOcclusion = true;
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

    private void OnSet_VSync(bool value)
    {
        if (value)
            QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;

        PlayerPrefs.SetString(VSYNC, value.ToString());
    }

    private void OnSet_Bloom(bool value)
    {
        for (int i = 0; i < processProfiles.Length; i++)
        {
            if (processProfiles[i].HasSettings<Bloom>())
            {
                processProfiles[i].GetSetting<Bloom>().active = value;
            }
        }

        PlayerPrefs.SetString(BLOOM, value.ToString());
    }

    private void OnSet_AmbientOcclusion(bool value)
    {
        for (int i = 0; i < processProfiles.Length; i++)
        {
            if (processProfiles[i].HasSettings<AmbientOcclusion>())
            {
                processProfiles[i].GetSetting<AmbientOcclusion>().active = value;
            }
        }

        PlayerPrefs.SetString(AMBIENTOCCLUSION, value.ToString());
    }
}
