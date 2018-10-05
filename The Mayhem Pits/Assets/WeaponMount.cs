using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponMount : MonoBehaviour {

    [SerializeField] private List<GameObject> acceptedWeaponPrefabs = new List<GameObject>();
    public List<GameObject> AcceptedWeaponPrefabs { get { return acceptedWeaponPrefabs; } }

    //public int index;
    //public Vector3 position;
    //public Vector3 rotation;    
}
