using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Purchasable {

    [SerializeField] private int id = -1;
    [SerializeField] private string name;

    [SerializeField] private int cost = 100;
    [SerializeField] private bool unlocked = false;

    public int ID { get { return id; } }
    public string Name { get { return name; } }

    public int Cost { get { return cost; } }
    public bool Unlocked { get { return unlocked; } }

    public void Unlock ()
    {
        unlocked = true;
        cost = 0;
    }
}

