﻿using System;
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

    private bool allowModeSwitch = true;
    private bool allowTargeting = false;

    private void Start()
    {
        cameraMounts = GameObject.FindObjectsOfType<CameraMount>();

        if (ClientMode.Instance.GetMode == ClientMode.Mode.Normal)
        {
            transform.GetChild(1).SetParent(transform.GetChild(0));
        }

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
                if (allowModeSwitch && !FindObjectOfType<HUD_PauseMenu_Panel>().IsPaused)
                    targetType = TargetType.Pillars;
            }
        }
        else if (targetType == TargetType.Pillars)
        {
            FindNearestMount();
            TargetPillar();

            if (Input.GetButtonDown("XBO_A"))
            {
                if (allowModeSwitch && !FindObjectOfType<HUD_PauseMenu_Panel>().IsPaused)
                    targetType = TargetType.Overview;
            }
        }
        else if (targetType == TargetType.Overview)
        {
            FindNearestMount();
            TargetPillar();

            if (Input.GetButtonDown("XBO_A"))
            {
                if (allowModeSwitch && !FindObjectOfType<HUD_PauseMenu_Panel>().IsPaused)
                    targetType = TargetType.Robot;
            }
        }

        AxisRotate ();
    }

    public void SetMode (TargetType type)
    {
        targetType = type;
    }

    private void AxisRotate ()
    {
        if (ClientMode.Instance.GetMode == ClientMode.Mode.Normal)
        {
            Transform t = transform.GetChild ( 0 );

            if ((Input.GetAxis ( "XBO_RH" ) != 0.0f || Input.GetAxis ( "XBO_RV" ) != 0.0f))
            {
                //Debug.Log ( "Greater" );
                Vector3 newRot = t.transform.localEulerAngles;

                //newRot.x += Input.GetAxis ( "XBO_RV" ) * 20.0f;
                //newRot.y += Input.GetAxis ( "XBO_RH" ) * 20.0f;
                //newRot.z = 0.0f;

                newRot.x = Input.GetAxis ( "XBO_RV" ) * 40.0f;
                newRot.y = Input.GetAxis ( "XBO_RH" ) * 40.0f;
                newRot.z = 0.0f;

                t.transform.localRotation = Quaternion.Slerp ( Quaternion.Euler ( t.transform.localEulerAngles ), Quaternion.Euler ( newRot ), Time.deltaTime * 2.0f );
            }
            else
            {
                t.transform.localRotation = Quaternion.Slerp ( t.transform.localRotation, Quaternion.Euler ( Vector3.zero ), Time.deltaTime * 4.0f );
            }

            //if ((Input.GetAxis ( "XBO_RH" ) >= 0.2f || Input.GetAxis ( "XBO_RH" ) <= -0.2f) || (Input.GetAxis ( "XBO_RV" ) >= 0.2f || Input.GetAxis ( "XBO_RV" ) <= -0.2f))
            //{
            //    //Debug.Log ( "Greater" );
            //    Vector3 newRot = t.transform.localEulerAngles;

            //    //newRot.x += Input.GetAxis ( "XBO_RV" ) * 20.0f;
            //    //newRot.y += Input.GetAxis ( "XBO_RH" ) * 20.0f;
            //    //newRot.z = 0.0f;

            //    newRot.x = Input.GetAxis ( "XBO_RV" ) * 40.0f;
            //    newRot.y = Input.GetAxis ( "XBO_RH" ) * 40.0f;
            //    newRot.z = 0.0f;

            //    t.transform.localRotation = Quaternion.Slerp ( Quaternion.Euler ( t.transform.localEulerAngles ), Quaternion.Euler ( newRot ), Time.deltaTime * 2.0f );
            //}
            //else
            //{
            //    t.transform.localRotation = Quaternion.Slerp ( t.transform.localRotation, Quaternion.Euler ( Vector3.zero ), Time.deltaTime * 4.0f );
            //}      
        }
    }

    private void TargetRobot()
    {
        // Define a target position above and behind the target transform        
        Vector3 targetPosition = currentTarget.TransformPoint(initialCameraOffset);

        targetPosition.y = currentTarget.position.y + initialCameraOffset.y;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, robotSmoothTime);

        if (allowTargeting)
        {
            Vector3 lookAtPos = targetRobot.position;
            if (ClientMode.Instance.GetMode == ClientMode.Mode.VR)
                lookAtPos.y = transform.position.y;
            transform.LookAt(lookAtPos);
        }
    }

    private void TargetPillar()
    {
        Vector3 targetPosition = currentTarget.position;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, pillarSmoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentTarget.rotation, pillarSmoothTime);

        if (allowTargeting)
        {
            Vector3 lookAtPos = targetRobot.position;
            if (ClientMode.Instance.GetMode == ClientMode.Mode.VR)
                lookAtPos.y = transform.position.y;
            transform.LookAt(lookAtPos);
        }
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
        if (target.gameObject.tag != "LocalGameRobot" && FindObjectOfType<OfflineRobotCreator>().active && FindObjectOfType<OfflineRobotCreator>().ai) return;
        targetRobot = target;
    }

    public void DisableModeSwitch ()
    {
        allowModeSwitch = false;   
    }

    public void SetAllowTarget(bool state)
    {
        allowTargeting = state;
    }
}
