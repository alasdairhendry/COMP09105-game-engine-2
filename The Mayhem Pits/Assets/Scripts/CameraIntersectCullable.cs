using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraIntersectCullable : MonoBehaviour {

    [SerializeField] private List<MeshRenderer> meshRenderers = new List<MeshRenderer> ();
    [SerializeField] private bool autoCollectRenderers = true;
    [SerializeField] private bool includeChildren = true;    
    
    [SerializeField] private bool switchToAlphaMaterial = true;    
    [SerializeField] private List<Material> alphaMaterials = new List<Material> ();
    private List<Material> opaqueMaterials = new List<Material> ();

	// Use this for initialization
	void Start () {        
        CollectRenderers ();
        CollectMaterials ();
	}

    private void CollectRenderers ()
    {
        if (autoCollectRenderers)
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer> ( includeChildren ).ToList ();
        }
    }

    private void CollectMaterials ()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            opaqueMaterials.Add ( meshRenderers[i].material );
        }
    }

    public void Show ()
    {
        if (switchToAlphaMaterial) ShowAlpha ();
        else ShowDefault ();
    }

    private void ShowDefault ()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            meshRenderers[i].enabled = true;
        }
    }

    private void ShowAlpha ()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            meshRenderers[i].material = opaqueMaterials[i];
        }
    }

    public void Hide ()
    {
        if (switchToAlphaMaterial) HideAlpha ();
        else HideDefault ();
    }   
    
    public void HideDefault ()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            meshRenderers[i].enabled = false;
        }
    }

    public void HideAlpha ()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if(i < alphaMaterials.Count)
            {
                meshRenderers[i].material = alphaMaterials[i];                
            }
            else
            {
                meshRenderers[i].material = alphaMaterials[0];
            }
        }
    }
}
