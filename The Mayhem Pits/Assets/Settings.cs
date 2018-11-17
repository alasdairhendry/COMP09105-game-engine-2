using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.Rendering.LightweightPipeline;
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
    [SerializeField] private LightweightPipelineAsset renderPipeline;

    private const string MASTER_VOLUME = "masterV";
    private const string MUSIC_VOLUME = "musicV";
    private const string SFX_VOLUME = "sfxV";

    private const string VSYNC = "vsync";
    private const string RESOLUTION = "resolution";
    private const string TEXTURES = "textures";
    private const string ANTIALIASING = "antialiasing";

    private const string BLOOM = "bloom";
    private const string VIGNETTE = "vignette";

    //----------------------------------------------------------

    private float masterVolume = 1.0f;
    public float MasterVolume { get { return masterVolume; } set { OnSet_MasterVolume(value); masterVolume = value; } }

    private float musicVolume = 1.0f;
    public float MusicVolume { get { return musicVolume; } set { OnSet_MusicVolume(value); musicVolume = value; } }

    private float sfxVolume = 1.0f;
    public float SfxVolume { get { return sfxVolume; } set { OnSet_SfxVolume(value); sfxVolume = value; } }

    private bool vSync = true;
    public bool VSync { get { return vSync; } set { OnSet_VSync(value); vSync = value; } }

    private float resolution = 1.0f;
    public float Resolution { get { return resolution; } set { OnSet_Resolution(value); resolution = value; } }

    private int textures = 2;
    public int Textures { get { return textures; } set { OnSet_Textures(value); textures = value; } }

    private int antiAliasing = 1;
    public int AntiAliasing { get { return antiAliasing; } set { OnSet_AntiAliasing(value); antiAliasing = value; } }

    private bool bloom = true;
    public bool Bloom { get { return bloom; } set { OnSet_Bloom(value); bloom = value; } }

    private bool vignette = true;
    public bool Vignette { get { return vignette; } set { OnSet_VignetteOcclusion(value); vignette = value; } }    

    private void LoadSettings()
    {        
        if (PlayerPrefs.HasKey(MASTER_VOLUME)) MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME);
        else MasterVolume = 1.0f;

        if (PlayerPrefs.HasKey(MUSIC_VOLUME)) MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        else MusicVolume = 1.0f;

        if (PlayerPrefs.HasKey(SFX_VOLUME)) SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME);
        else SfxVolume = 1.0f;

        if (PlayerPrefs.HasKey(VSYNC)) VSync = bool.Parse(PlayerPrefs.GetString(VSYNC));
        else VSync = true;

        if (PlayerPrefs.HasKey(RESOLUTION)) Resolution = PlayerPrefs.GetFloat(RESOLUTION); 
        else Resolution = 1.0f;        

        if (PlayerPrefs.HasKey(TEXTURES)) Textures = PlayerPrefs.GetInt(TEXTURES);
        else Textures = 2;

        if (PlayerPrefs.HasKey(ANTIALIASING)) AntiAliasing = PlayerPrefs.GetInt(ANTIALIASING);
        else AntiAliasing = 1;

        if (PlayerPrefs.HasKey(BLOOM)) Bloom = bool.Parse(PlayerPrefs.GetString(BLOOM));
        else Bloom = true;

        if (PlayerPrefs.HasKey(VIGNETTE)) Vignette = bool.Parse(PlayerPrefs.GetString(VIGNETTE));
        else Vignette = true;        
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

    private void OnSet_Resolution(float value)
    {
        value = Mathf.Clamp(value, 0.25f, 2.0f);
        renderPipeline.renderScale = value;

        PlayerPrefs.SetFloat(RESOLUTION, value);
    }

    private void OnSet_Textures(int value)
    {
        value = Mathf.Clamp(value, 0, 3);
        //QualitySettings.masterTextureLimit = value;

        PlayerPrefs.SetInt(TEXTURES, value);
    }

    private void OnSet_AntiAliasing(int value)
    {
        value = Mathf.Clamp(value, 1, 8);

        renderPipeline.msaaSampleCount = value;
        PlayerPrefs.SetInt(ANTIALIASING, value);
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

    private void OnSet_VignetteOcclusion(bool value)
    {
        for (int i = 0; i < processProfiles.Length; i++)
        {
            if (processProfiles[i].HasSettings<Vignette>())
            {
                processProfiles[i].GetSetting<Vignette>().active = value;
            }
        }

        PlayerPrefs.SetString(VIGNETTE, value.ToString());
    }
}
