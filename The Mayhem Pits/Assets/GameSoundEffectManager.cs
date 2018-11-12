using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundEffectManager : MonoBehaviourPunCallbacks {

    public static GameSoundEffectManager Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    public enum Effect { MetalImpact }

    [SerializeField] private List<EffectPair> pairs = new List<EffectPair> ();
    [SerializeField] private GameObject soundEffect;

    [System.Serializable]
    struct EffectPair
    {
        public Effect effect;
        public AudioClip clip;
    }

    private AudioClip GetClip (Effect effect)
    {
        for (int i = 0; i < pairs.Count; i++)
        {
            if (pairs[i].effect == effect)
            {
                return pairs[i].clip;
            }
        }

        Debug.LogError ( "Clip not found" );
        return pairs[0].clip;
    }

    private AudioClip GetClip (int index)
    {
        return pairs[index].clip;
    }

    private int GetIndex(Effect effect)
    {
        for (int i = 0; i < pairs.Count; i++)
        {
            if (pairs[i].effect == effect)
            {
                return i;
            }
        }

        Debug.LogError ( "Index not found" );
        return 0;
    }

    public void PlayLocalSound(Effect effect, float volume, float pitch, bool threeDimensional, Vector3 position)
    {
        GameObject go = Instantiate ( soundEffect );
        AudioSource audioSource = go.GetComponent<AudioSource> ();

        audioSource.spatialBlend = (threeDimensional) ? 1.0f : 0.0f;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        go.transform.position = position;
        audioSource.clip = GetClip ( effect );
        audioSource.Play ();
        go.GetComponent<SelfDestruct> ().SetLifetime ( audioSource.clip.length );
    }

    public void PlayNetworkSound(Effect effect, float volume, float pitch, bool threeDimensional, Vector3 position)
    {
        GameObject go = Instantiate ( soundEffect );
        AudioSource audioSource = go.GetComponent<AudioSource> ();

        audioSource.spatialBlend = (threeDimensional) ? 1.0f : 0.0f;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        go.transform.position = position;
        audioSource.clip = GetClip ( effect );
        audioSource.Play ();
        go.GetComponent<SelfDestruct> ().SetLifetime ( audioSource.clip.length );

        photonView.RPC ( "RPCPlaySound", RpcTarget.Others, GetIndex ( effect ), threeDimensional, position );
    }

    [PunRPC]
    private void RPCPlaySound(int index, float volume, float pitch, bool threeDimensional, Vector3 position)
    {
        GameObject go = Instantiate ( soundEffect );
        AudioSource audioSource = go.GetComponent<AudioSource> ();

        audioSource.spatialBlend = (threeDimensional) ? 1.0f : 0.0f;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        go.transform.position = position;
        audioSource.clip = GetClip ( index );
        audioSource.Play ();
        go.GetComponent<SelfDestruct> ().SetLifetime ( audioSource.clip.length );
    }
}
