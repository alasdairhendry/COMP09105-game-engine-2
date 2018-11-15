using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RobotBody : MonoBehaviour {

    [SerializeField] private List<WeaponMount> weaponMounts = new List<WeaponMount>();
    public List<WeaponMount> WeaponMounts { get { return weaponMounts; } }

    [SerializeField] private EmblemMount emblemMount;
    public EmblemMount EmblemMount { get { return emblemMount; } }
    
    private void Awake()
    {
        //weaponMounts = GetComponentsInChildren<WeaponMount>().ToList();
        //emblemMounts = GetComponentsInChildren<EmblemMount>().ToList();
    }   
    
    [ContextMenu("Set")]
    public void SetTexture()
    {
        Texture2D albedo = GetComponent<MeshRenderer>().material.GetTexture("_Albedo") as Texture2D;
        Texture2D metallic = GetComponent<MeshRenderer>().material.GetTexture("_Metallic") as Texture2D;

        Color[] albedoColours = albedo.GetPixels();
        Color[] metallicColours = metallic.GetPixels();
        bool found = false;
        for (int i = 0; i < albedoColours.Length; i++)
        {
            if(albedoColours[i].grayscale == 1.0f)
            {
                if (!found)
                {
                    found = true;
                    Debug.Log(albedoColours[i]);
                }
                metallicColours[i] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }

        Texture2D tex = new Texture2D(1024, 1024);
        tex.SetPixels(metallicColours);

        metallic.SetPixels(metallicColours);

        AssetDatabase.CreateAsset(tex, "Assets/texture.asset");

        Debug.Log(albedo.name);
    }
}
