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
    [SerializeField] private GameObject resolutionButton;
    [SerializeField] private GameObject texturesButton;
    [SerializeField] private GameObject antiAliasingButton;

    [SerializeField] private GameObject bloomButton;
    [SerializeField] private GameObject vignetteButton;

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
            if (f != 0)
                Settings.Instance.VSync = !Settings.Instance.VSync;

            SetBoolButton(Settings.Instance.VSync, vSyncButton.GetComponent<Text>(), "VSync");
        };

        resolutionButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            if (f == 0) return;

            HUDAxisSlider a = resolutionButton.GetComponent<HUDAxisSlider>();

            float v = Settings.Instance.Resolution;
            v += f * Time.deltaTime * a.speed;

            v = Mathf.Clamp(v, a.minValue, a.maxValue);

            SetStringButton(v * 100.0f, resolutionButton.GetComponent<Text>(), "Resolution", "%");

            Settings.Instance.Resolution = v;
        };

        texturesButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            if (f == 0) return;

            HUDAxisSlider a = texturesButton.GetComponent<HUDAxisSlider>();

            int v = Settings.Instance.Textures;
            v += (int)f * -1;

            v = Mathf.Clamp(v, (int)a.minValue, (int)a.maxValue);

            SetIntDescribedButton(v, texturesButton.GetComponent<Text>(), "Textures", new List<int>() { 3, 2, 1, 0 }, "Low", "Medium", "High", "Very High");

            Settings.Instance.Textures = v;
        };

        antiAliasingButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            if (f == 0) return;

            HUDAxisSlider a = antiAliasingButton.GetComponent<HUDAxisSlider>();

            int v = Settings.Instance.AntiAliasing;

            if(f > 0)
            {
                v *= 2;
            }
            else if(f < 0)
            {
                v /= 2;
            }            

            v = Mathf.Clamp(v, (int)a.minValue, (int)a.maxValue);

            SetIntDescribedButton(v, antiAliasingButton.GetComponent<Text>(), "Anti Aliasing", new List<int>() { 1, 2, 4, 8 }, "Off", "2x", "4x", "8x");

            Settings.Instance.AntiAliasing = v;
        };

        // ------------------------------------------------------------------------------------------------------------------

        bloomButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            if (f != 0)
                Settings.Instance.Bloom = !Settings.Instance.Bloom;

            SetBoolButton(Settings.Instance.Bloom, bloomButton.GetComponent<Text>(), "Bloom");
        };

        vignetteButton.GetComponent<HUDAxisSlider>().slide += (f) =>
        {
            if(f != 0)            
                Settings.Instance.Vignette = !Settings.Instance.Vignette;

            SetBoolButton(Settings.Instance.Vignette, vignetteButton.GetComponent<Text>(), "Vignette");
        };
    }

    private void SetDefaults()
    {
        SetStringButton(Settings.Instance.MasterVolume * 100.0f, masterVolumeButton.GetComponent<Text>(), "Master Volume", "%", true);
        SetStringButton(Settings.Instance.MusicVolume * 100.0f, musicVolumeButton.GetComponent<Text>(), "Music Volume", "%", true);
        SetStringButton(Settings.Instance.SfxVolume * 100.0f, sfxVolumeButton.GetComponent<Text>(), "Effects Volume", "%", true);

        SetBoolButton(Settings.Instance.VSync, vSyncButton.GetComponent<Text>(), "VSync");
        SetStringButton(Settings.Instance.Resolution * 100.0f, resolutionButton.GetComponent<Text>(), "Resolution", "%");
        SetIntDescribedButton(Settings.Instance.Textures, texturesButton.GetComponent<Text>(), "Textures", new List<int>() { 3, 2, 1, 0 }, "Low", "Medium", "High", "Very High");
        SetIntDescribedButton(Settings.Instance.AntiAliasing, antiAliasingButton.GetComponent<Text>(), "Anti Aliasing", new List<int>() { 1, 2, 4, 8 }, "Off", "2x", "4x", "8x");

        SetBoolButton(Settings.Instance.Bloom, bloomButton.GetComponent<Text>(), "Bloom");
        SetBoolButton(Settings.Instance.Vignette, vignetteButton.GetComponent<Text>(), "Vignette");
    }

    private void SetStringButton(float v, Text text, string prefix, string suffix = "", bool defaultOff = false)
    {
        if (v <= 0 && defaultOff)
            text.text = prefix + ": Off";
        else text.text = prefix + ": " + (v).ToString("00") + suffix;
    }

    private void SetBoolButton(bool b, Text text, string prefix, string suffix = "")
    {
        if (b)
        {
            text.text = prefix + ": On";
        }
        else
        {
            text.text = prefix + ": Off";
        }
    }

    private void SetIntDescribedButton(int v, Text text, string prefix, List<int> indices, params string[] descriptions)
    {
        if(indices.Count != descriptions.Length) { Debug.LogError("Index mismatch"); return; }
        if (!indices.Contains(v)) { Debug.LogError("Index missing - Looking for " + v); return; };

        text.text = prefix + ": " + descriptions[indices.IndexOf(v)];
    }

	public void OnClick_Back()
    {
        SceneLoader.Instance.LoadScene("Menu"); 
    }
}