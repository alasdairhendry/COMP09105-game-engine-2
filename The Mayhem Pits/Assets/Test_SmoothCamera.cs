using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SmoothCamera : MonoBehaviour {

    public enum TargetType { Robot, Pillars, Overview }
    [SerializeField] private TargetType targetType = TargetType.Robot;

    [SerializeField] private Transform targetRobot;
    [SerializeField] private float robotSmoothTime = 0.3f;
    [SerializeField] private float pillarSmoothTime = 0.3f;

    private Transform currentTarget;
    private CameraMount[] cameraMounts;
    private Vector3 initialCameraOffset = new Vector3();
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        cameraMounts = GameObject.FindObjectsOfType<CameraMount>();

        initialCameraOffset = new Vector3(0.0f, 2.0f, -4.0f);
    }

    private void Update()
    {
        if (targetRobot == null) return;
        if (targetType == TargetType.Robot)
        {
            currentTarget = targetRobot;
            TargetRobot();

            if (Input.GetButtonDown("XBO_A"))
            {
                targetType = TargetType.Pillars;
            }
        }
        else if (targetType == TargetType.Pillars)
        {
            FindNearestMount();
            TargetPillar();

            if (Input.GetButtonDown("XBO_A"))
            {
                targetType = TargetType.Overview;
            }
        }
        else if (targetType == TargetType.Overview)
        {
            FindNearestMount();
            TargetPillar();

            if (Input.GetButtonDown("XBO_A"))
            {
                targetType = TargetType.Robot;
            }
        }
    }

    private void TargetRobot()
    {
        // Define a target position above and behind the target transform        
        Vector3 targetPosition = currentTarget.TransformPoint(initialCameraOffset);

        targetPosition.y = currentTarget.position.y + initialCameraOffset.y;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, robotSmoothTime);

        Vector3 lookAtPos = targetRobot.position;
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);
    }

    private void TargetPillar()
    {
        Vector3 targetPosition = currentTarget.position;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, pillarSmoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentTarget.rotation, pillarSmoothTime);

        Vector3 lookAtPos = targetRobot.position;
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);
    }

    private void FindNearestMount()
    {
        float distance = Mathf.Infinity;
        Transform _target = currentTarget;

        foreach (CameraMount item in cameraMounts)
        {
            if (item.GetMountType != targetType) continue;
            float d = Vector3.Distance(item.transform.position, targetRobot.position);

            if (d < distance)
            {
                distance = d;
                _target = item.transform;
            }

        }

        currentTarget = _target;
    }

    public void SetTarget(Transform target)
    {
        targetRobot = target;
    }
}
