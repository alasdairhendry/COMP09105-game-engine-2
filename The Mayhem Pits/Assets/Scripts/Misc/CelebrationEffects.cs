using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelebrationEffects : MonoBehaviour {

    [SerializeField] private GameObject particlePrefab;
    private List<Transform> children = new List<Transform> ();

    private void Start ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add ( transform.GetChild ( i ) );
        }
    }

    public void Activate ()
    {
        for (int i = 0; i < children.Count; i++)
        {
            GameObject go = Instantiate ( particlePrefab );
            go.transform.SetParent ( children[i] );
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
        }
    }
}
