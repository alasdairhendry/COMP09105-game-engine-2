using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Settings_Panel : MonoBehaviour {

    [SerializeField] private GameObject body;

    [SerializeField] private GameObject masterVolumeButton;
    [SerializeField] private GameObject musicVolumeButton;
    [SerializeField] private GameObject sfxVolumeButton;

    [SerializeField] private GameObject vSyncButton;
    [SerializeField] private GameObject bloomButton;
    [SerializeField] private GameObject ambientOcclusionButton;

    // Use this for initialization
    void Start () {
        SetCallbacks();
        SetDefaults();
        Close();
    }

    public void Open()
    {
        body.SetActive(true);
        body.GetComponent<HUDSelectionGroup>().SetActiveGroup();
        SetDefaults();
    }

    public void Close()
    {
        body.SetActive(false);        
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

        // ------------------------------------------------------------------------------------------------------------------

        vSyncButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            HUDAxisSlider a = vSyncButton.GetComponent<HUDAxisSlider>();

            if (f > 0)
            {
                Settings.Instance.VSync = true;
                vSyncButton.GetComponent<Text>().text = "VSYNC: On";
            }
            else if (f < 0)
            {
                Settings.Instance.VSync = false;
                vSyncButton.GetComponent<Text>().text = "VSYNC: Off";
            }
        };

        bloomButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            HUDAxisSlider a = bloomButton.GetComponent<HUDAxisSlider>();

            if (f > 0)
            {
                Settings.Instance.Bloom = true;
                bloomButton.GetComponent<Text>().text = "BLOOM: On";
            }
            else if (f < 0)
            {
                Settings.Instance.Bloom = false;
                bloomButton.GetComponent<Text>().text = "BLOOM: Off";
            }
        };

        ambientOcclusionButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            HUDAxisSlider a = ambientOcclusionButton.GetComponent<HUDAxisSlider>();

            if (f > 0)
            {
                Settings.Instance.AmbientOcclusion = true;
                ambientOcclusionButton.GetComponent<Text>().text = "AMBIENT OCCLUSION: On";
            }
            else if (f < 0)
            {
                Settings.Instance.AmbientOcclusion = false;
                ambientOcclusionButton.GetComponent<Text>().text = "AMBIENT OCCLUSION: Off";
            }
        };
    }

    private void SetDefaults()
    {
        SetVolumeButton(Settings.Instance.MasterVolume, masterVolumeButton.GetComponent<Text>(), "Master Volume");
        SetVolumeButton(Settings.Instance.MusicVolume, musicVolumeButton.GetComponent<Text>(), "Music Volume");
        SetVolumeButton(Settings.Instance.SfxVolume, sfxVolumeButton.GetComponent<Text>(), "Effects Volume");
        SetBoolButton(Settings.Instance.VSync, vSyncButton.GetComponent<Text>(), "VSYNC");
        SetBoolButton(Settings.Instance.Bloom, bloomButton.GetComponent<Text>(), "BLOOM");
        SetBoolButton(Settings.Instance.AmbientOcclusion, ambientOcclusionButton.GetComponent<Text>(), "AMBIENT OCCLUSION");
    }

    private void SetVolumeButton(float v, Text text, string s)
    {
        if (v <= 0) text.text = s + ": Off";
        else text.text = s + ": " + (v * 100.0f).ToString("00") + "%";
    }

    private void SetBoolButton(bool b, Text text, string s)
    {
        if (b)
        {
            text.text = s + ": On";
        }
        else
        {
            text.text = s + ": Off";
        }
    }

	public void OnClick_Back()
    {
        SceneLoader.Instance.LoadScene("Menu"); 
    }
}