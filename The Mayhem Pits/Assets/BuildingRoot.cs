using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingRoot : MonoBehaviour {

    [SerializeField] private List<GameObject> enableOnFinish = new List<GameObject>();

    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private float timePerSegmenet = 1.25f;
    private float currentTime = 0.0f;

	// Use this for initialization
	void Start () {
        timePerSegmenet += UnityEngine.Random.Range(-0.25f, 0.25f);
        GetRenderers();
        StartCoroutine(Display());
	}

    private void GetRenderers()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();

        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i].materials.Length == 1)
                meshRenderers[i].material.SetFloat("_alpha", 1.0f);
            else
            {
                meshRenderers[i].materials[0].SetFloat("_alpha", 1.0f);
                meshRenderers[i].materials[1].SetFloat("_alpha", 1.0f);
            }
        }

        meshRenderers = meshRenderers.OrderBy(x => x.transform.position.y).ToList();
    }

    IEnumerator Display()
    {
        int i = 0;

        while(i < meshRenderers.Count)
        {
            while(currentTime < timePerSegmenet)
            {                
                currentTime += Time.deltaTime * UnityEngine.Random.Range(0.9f, 1.1f); ;

                if(meshRenderers[i].materials.Length == 1)
                    meshRenderers[i].material.SetFloat("_alpha", Mathf.Lerp(1.0f, 0.0f, currentTime / timePerSegmenet));
                else
                {
                    meshRenderers[i].materials[0].SetFloat("_alpha", Mathf.Lerp(1.0f, 0.0f, currentTime / timePerSegmenet));
                    meshRenderers[i].materials[1].SetFloat("_alpha", Mathf.Lerp(1.0f, 0.0f, currentTime / timePerSegmenet));
                }  

                yield return null;
            }

            currentTime = 0.0f;

            i++;
            yield return null;
        }

        for (int x = 0; x < enableOnFinish.Count; x++)
        {
            enableOnFinish[x].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
