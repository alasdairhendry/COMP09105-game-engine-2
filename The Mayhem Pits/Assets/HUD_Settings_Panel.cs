using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Settings_Panel : MonoBehaviour {

    [SerializeField] private GameObject masterVolumeButton;
    [SerializeField] private GameObject musicVolumeButton;
    [SerializeField] private GameObject sfxVolumeButton;

    // Use this for initialization
    void Start () {
        SetCallbacks();
        SetDefaults();
       
    }

    private void SetCallbacks()
    {
        masterVolumeButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            HUDAxisSlider a = masterVolumeButton.GetComponent<HUDAxisSlider>();

            float v = Settings.Instance.MasterVolume;
            v += f * Time.deltaTime * a.speed;

            v = Mathf.Clamp(v, a.minValue, a.maxValue);

            if (v <= 0)
                masterVolumeButton.GetComponent<Text>().text = "Master Volume: Off";
            else masterVolumeButton.GetComponent<Text>().text = "Master Volume: " + (v * 100.0f).ToString("00") + "%";

            Settings.Instance.MasterVolume = v;
        };

        musicVolumeButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            HUDAxisSlider a = musicVolumeButton.GetComponent<HUDAxisSlider>();

            float v = Settings.Instance.MusicVolume;
            v += f * Time.deltaTime * a.speed;

            v = Mathf.Clamp(v, a.minValue, a.maxValue);

            if (v <= 0)
                musicVolumeButton.GetComponent<Text>().text = "Music Volume: Off";
            else musicVolumeButton.GetComponent<Text>().text = "Music Volume: " + (v * 100.0f).ToString("00") + "%";

            Settings.Instance.MusicVolume = v;
        };

        sfxVolumeButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            HUDAxisSlider a = sfxVolumeButton.GetComponent<HUDAxisSlider>();

            float v = Settings.Instance.SfxVolume;
            v += f * Time.deltaTime * a.speed;

            v = Mathf.Clamp(v, a.minValue, a.maxValue);

            if (v <= 0)
                sfxVolumeButton.GetComponent<Text>().text = "Effects Volume: Off";
            else sfxVolumeButton.GetComponent<Text>().text = "Effects Volume: " + (v * 100.0f).ToString("00") + "%";

            Settings.Instance.SfxVolume = v;
        };
    }

    private void SetDefaults()
    {
        SetVolumeButton(Settings.Instance.MasterVolume, masterVolumeButton.GetComponent<Text>(), "Master Volume");
        SetVolumeButton(Settings.Instance.MusicVolume, musicVolumeButton.GetComponent<Text>(), "Music Volume");
        SetVolumeButton(Settings.Instance.SfxVolume, sfxVolumeButton.GetComponent<Text>(), "Effects Volume");
    }

    private void SetVolumeButton(float v, Text text, string s)
    {
        if (v <= 0) text.text = s + ": Off";
        else text.text = s + ": " + (v * 100.0f).ToString("00") + "%";
    }

	public void OnClick_Back()
    {
        SceneLoader.Instance.LoadScene("Menu"); 
    }
}