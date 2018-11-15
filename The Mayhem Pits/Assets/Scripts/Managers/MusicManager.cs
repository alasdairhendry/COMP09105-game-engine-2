using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager Instance;

    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private AudioClip defaultClip;
    [SerializeField] private float delay = 7.5f;

    private AudioSource source;
    private bool active = true;
    private bool hasDefault = false;
    private float currentDelay = 0.0f;
    private int currentIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        source = GetComponent<AudioSource>();
        currentDelay = delay;
    }

    public void SetActive(bool state)
    {
        if (state)
        {
            active = true;
            if (source.isPlaying) return;  
        }
        else
        {
            active = false;
            source.Stop();
        }
    }

    private void Update()
    {
        if (!active) return;

        if (!source.isPlaying)
        {
            currentDelay += Time.deltaTime;

            if(currentDelay>= delay)
            {
                currentDelay = 0.0f;
                Switch();
            }
        }
    }

    private void Switch()
    {
        if (CheckDefault()) return;

        List<AudioClip> _clips = new List<AudioClip>();

        for (int i = 0; i < clips.Count; i++)
        {
            if (i == currentIndex) continue;
            _clips.Add(clips[i]);
        }

        Play(_clips[UnityEngine.Random.Range(0, _clips.Count)]);
    }

    private bool CheckDefault()
    {
        if (!hasDefault)
        {
            hasDefault = true;
            Play(defaultClip);
            return true;
        }

        return false;
    }

    private void Play(AudioClip clip)
    {
        currentIndex = clips.IndexOf(clip);
        source.clip = clip;
        source.Play();
    }
}
