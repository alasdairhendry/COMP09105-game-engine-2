using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMount : MonoBehaviour {

    [SerializeField] private Test_SmoothCamera.TargetType targetType;
    public Test_SmoothCamera.TargetType GetMountType { get { return targetType; } }
}
