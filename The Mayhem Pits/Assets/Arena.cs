using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    [SerializeField] private GameObject[] grounds;
    [SerializeField] private GameObject[] quadrants;    

	// Use this for initialization
	void Start () {
        if (grounds[0].activeSelf)
        {

        }
        else if (grounds[1].activeSelf)
        {
            SpawnQuadrants();
        }
    }

    private void SpawnQuadrants()
    {
        for (int x = 0; x < 12; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                GameObject go = Instantiate(quadrants[Random.Range(0, quadrants.Length)], transform.Find("Quadrants"));
                go.transform.localPosition = new Vector3((x * 1.5f) - 8.25f, 0.0f, (z * 1.5f) - 11.25f);
                go.transform.localScale = new Vector3(100.0f, 100.0f, 100.0f);
                go.transform.localEulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
            }
        }
    }
}
