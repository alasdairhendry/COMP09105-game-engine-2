﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour {

    [SerializeField] private float m_DefaultDistance = 5f;      // The default distance away from the camera the reticle is placed.
    [SerializeField] private bool m_UseNormal;                  // Whether the reticle should be placed parallel to a surface.
    [SerializeField] private Image m_Image;                     // Reference to the image component that represents the reticle.
    [SerializeField] private Transform m_ReticleTransform;      // We need to affect the reticle's transform.
    [SerializeField] private Transform m_Camera;                // The reticle is always placed relative to the camera.

    [SerializeField] private Color normalColour;
    [SerializeField] private Color hoverColour;

    private Vector3 m_OriginalScale;                            // Since the scale of the reticle changes, the original scale needs to be stored.
    private Quaternion m_OriginalRotation;                      // Used to store the original rotation of the reticle.

    private void Awake()
    {
        m_OriginalScale = m_ReticleTransform.localScale;
        m_OriginalRotation = m_ReticleTransform.localRotation;
    }

    public void Show()
    {
        m_Image.GetComponent<Image>().enabled = true;
    }

    public void Hide()
    {
        m_Image.GetComponent<Image>().enabled  = false;
    }

    // This overload of SetPosition is used when the the VREyeRaycaster hasn't hit anything.
    public void SetPosition()
    {
        // Set the position of the reticle to the default distance in front of the camera.
        m_ReticleTransform.position = m_Camera.position + m_Camera.forward * m_DefaultDistance;

        // Set the scale based on the original and the distance from the camera.
        m_ReticleTransform.localScale = m_OriginalScale * m_DefaultDistance;

        // The rotation should just be the default.
        m_ReticleTransform.localRotation = m_OriginalRotation;

        m_Image.color = normalColour;
    }


    // This overload of SetPosition is used when the VREyeRaycaster has hit something.
    public void SetPosition(RaycastHit hit)
    {        
        m_ReticleTransform.position = hit.point;
        m_ReticleTransform.localScale = m_OriginalScale * hit.distance /** -1.0f*/;

        m_Image.color = hoverColour;

        // If the reticle should use the normal of what has been hit...
        if (m_UseNormal)
            // ... set it's rotation based on it's forward vector facing along the normal.
            m_ReticleTransform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
        else
            // However if it isn't using the normal then it's local rotation should be as it was originally.
            m_ReticleTransform.localRotation = m_OriginalRotation;
    }
}
