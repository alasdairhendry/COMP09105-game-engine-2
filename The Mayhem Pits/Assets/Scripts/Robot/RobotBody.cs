using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotBody : MonoBehaviour {

    [SerializeField] private List<WeaponMount> weaponMounts = new List<WeaponMount>();
    public List<WeaponMount> WeaponMounts { get { return weaponMounts; } }

    private void Awake()
    {
        weaponMounts = GetComponentsInChildren<WeaponMount>().ToList();
    }    
}
