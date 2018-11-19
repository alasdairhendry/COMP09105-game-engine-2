using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableTarget : MonoBehaviour {

    [SerializeField] private float lockTime = 1.0f;
    public float LockTime { get { return lockTime; } }

}
