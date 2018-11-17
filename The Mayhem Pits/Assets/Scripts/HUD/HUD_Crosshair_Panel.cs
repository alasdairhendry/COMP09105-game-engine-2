using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Crosshair_Panel : MonoBehaviour {

    [SerializeField] private GameObject bodyPanel;
    [SerializeField] private RectTransform crosshairRect;
    [SerializeField] private RectTransform contraintsRect;
    [SerializeField] private GameObject firePanel;
    [SerializeField] private Slider lockSlider;
    
    [SerializeField] private float targetDistance = 10.0f;
    [SerializeField] private Color lockedColour;    

    private bool active;
    private List<LockableTarget> lockableTargets = new List<LockableTarget> ();
    private LockableTarget currentTarget;
    private bool isLocking = false;
    private bool isLocked = false;
    private float lockCounter = 0.0f;

    private Vector2 uiOffset = new Vector2 ();
    private RectTransform canvasRect;

    private Action<LockableTarget> onLockedToTarget;

    private void Start ()
    {
        canvasRect = GetComponentInParent<Canvas> ().GetComponent<RectTransform> ();
        uiOffset = new Vector2 ( (float)canvasRect.sizeDelta.x / 2f, (float)canvasRect.sizeDelta.y / 2f );
        Hide ();
    }

    private void Update ()
    {
        if (!active) return;

        FindClosestTarget ();
        //MoveCrosshair ();
        //MonitorLocking ();
        UpdateSlider ();
        //ConstrainHUD ();

        //if (!isLocked)
        //{
        if (currentTarget != null)
            crosshairRect.Rotate ( new Vector3 ( 0.0f, 0.0f, 1.0f ) * (Mathf.Lerp ( 0.0f, 1.0f, lockCounter / currentTarget.LockTime )) * Time.deltaTime * 300.0f );
        //}

    }

    private void LateUpdate()
    {
        MoveCrosshair();
        MonitorLocking();
        ConstrainHUD ();
    }

    private void ConstrainHUD ()
    {
        float crosshairX = Mathf.Clamp ( crosshairRect.anchoredPosition.x, -(canvasRect.sizeDelta.x / 2.0f), (canvasRect.sizeDelta.x / 2.0f) );
        float crosshairY = Mathf.Clamp ( crosshairRect.anchoredPosition.y, -(canvasRect.sizeDelta.y / 2.0f), (canvasRect.sizeDelta.y / 2.0f) );
        crosshairRect.anchoredPosition = new Vector2 ( crosshairX, crosshairY );

        float constraintX = Mathf.Clamp ( contraintsRect.anchoredPosition.x, -(canvasRect.sizeDelta.x / 2.0f), (canvasRect.sizeDelta.x / 2.0f) );
        float constraintY = Mathf.Clamp ( contraintsRect.anchoredPosition.y, -(canvasRect.sizeDelta.y / 2.0f), (canvasRect.sizeDelta.y / 2.0f) );
        contraintsRect.anchoredPosition = new Vector2 ( constraintX, constraintY );
    }

    private void FindClosestTarget ()
    {
        if (lockableTargets.Count <= 0) return;
        if (isLocked) return;

        LockableTarget closestTarget = lockableTargets[0];
        float maxDistance = float.MaxValue;

        for (int i = 0; i < lockableTargets.Count; i++)
        {
            Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint ( lockableTargets[i].transform.GetComponentInChildren<Renderer>().bounds.center, Camera.MonoOrStereoscopicEye.Mono );
            Vector3 crosshairScreenPosition = Camera.main.WorldToScreenPoint ( contraintsRect.transform.position, Camera.MonoOrStereoscopicEye.Mono );

            float distance = Vector3.Distance ( targetScreenPosition, crosshairScreenPosition );

            if(distance<= maxDistance)
            {
                maxDistance = distance;
                closestTarget = lockableTargets[i];
            }

            //Debug.Log ( "Target " + lockableTargets[i].name + ": Position " + targetScreenPosition );
            //Debug.Log ( "Crosshair: Position " + crosshairScreenPosition );
        }

        currentTarget = maxDistance <= targetDistance ? closestTarget : null;
    }

    private void MoveCrosshair ()
    {
        if (isLocked && currentTarget != null)
        {
            MoveLockedCrosshair ();
            return;
        }

        contraintsRect.anchoredPosition = Vector2.zero;

        if (currentTarget == null)
        {
            float x = SmoothLerp.Lerp ( crosshairRect.anchoredPosition.x, 0.0f, Time.deltaTime * 50.0f );
            float y = SmoothLerp.Lerp ( crosshairRect.anchoredPosition.y, 0.0f, Time.deltaTime * 50.0f );

            x = Mathf.Clamp(x, Mathf.Min(crosshairRect.anchoredPosition.x, 0.0f), Mathf.Max(crosshairRect.anchoredPosition.x, 0.0f));
            y = Mathf.Clamp(y, Mathf.Min(crosshairRect.anchoredPosition.y, 0.0f), Mathf.Max(crosshairRect.anchoredPosition.y, 0.0f));

            crosshairRect.anchoredPosition = new Vector2 ( x, y );
            isLocking = false;
        }
        else
        {
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint ( currentTarget.transform.GetComponentInChildren<Renderer> ().bounds.center );
            Vector2 proportionalPosition = new Vector2 ( viewportPosition.x * canvasRect.sizeDelta.x, viewportPosition.y * canvasRect.sizeDelta.y );

            Vector3 targetScreenPosition = proportionalPosition - uiOffset;
            float x = SmoothLerp.Lerp(crosshairRect.anchoredPosition.x, targetScreenPosition.x, Time.deltaTime * 150.0f);
            float y = SmoothLerp.Lerp(crosshairRect.anchoredPosition.y, targetScreenPosition.y, Time.deltaTime * 150.0f);
         
            x = Mathf.Clamp(x, Mathf.Min( crosshairRect.anchoredPosition.x, targetScreenPosition.x), Mathf.Max(crosshairRect.anchoredPosition.x, targetScreenPosition.x));
            y = Mathf.Clamp(y, Mathf.Min(crosshairRect.anchoredPosition.y, targetScreenPosition.y), Mathf.Max(crosshairRect.anchoredPosition.y, targetScreenPosition.y));

            crosshairRect.anchoredPosition = new Vector2(x, y);
            
            isLocking = true;
        }
    }

    private void MoveLockedCrosshair ()
    {
        //Vector2 viewportPosition = Camera.main.WorldToViewportPoint ( currentTarget.transform.GetComponentInChildren<Renderer> ().bounds.center );
        //Vector2 proportionalPosition = new Vector2 ( viewportPosition.x * canvasRect.sizeDelta.x, viewportPosition.y * canvasRect.sizeDelta.y );

        //Vector3 targetScreenPosition = proportionalPosition - uiOffset;
        //float x = SmoothLerp.Lerp ( contraintsRect.anchoredPosition.x, targetScreenPosition.x, Time.deltaTime * 1500.0f );
        //float y = SmoothLerp.Lerp ( contraintsRect.anchoredPosition.y, targetScreenPosition.y, Time.deltaTime * 1500.0f );

        //contraintsRect.anchoredPosition = targetScreenPosition;
        //crosshairRect.anchoredPosition = targetScreenPosition;        

        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(currentTarget.transform.GetComponentInChildren<Renderer>().bounds.center);
        Vector2 proportionalPosition = new Vector2(viewportPosition.x * canvasRect.sizeDelta.x, viewportPosition.y * canvasRect.sizeDelta.y);

        Vector3 targetScreenPosition = proportionalPosition - uiOffset;
        float x = SmoothLerp.Lerp(crosshairRect.anchoredPosition.x, targetScreenPosition.x, Time.deltaTime * 1500.0f);
        float y = SmoothLerp.Lerp(crosshairRect.anchoredPosition.y, targetScreenPosition.y, Time.deltaTime * 1500.0f);

        x = Mathf.Clamp(x, Mathf.Min(crosshairRect.anchoredPosition.x, targetScreenPosition.x), Mathf.Max(crosshairRect.anchoredPosition.x, targetScreenPosition.x));
        y = Mathf.Clamp(y, Mathf.Min(crosshairRect.anchoredPosition.y, targetScreenPosition.y), Mathf.Max(crosshairRect.anchoredPosition.y, targetScreenPosition.y));

        contraintsRect.anchoredPosition = new Vector2(x, y);
        crosshairRect.anchoredPosition = new Vector2(x, y);
    }

    private void MonitorLocking ()
    {
        if (!isLocking || currentTarget == null) { lockCounter = 0.0f; return; }
        if (isLocked) return;

        lockCounter += Time.deltaTime;
        if (lockCounter >= currentTarget.LockTime)
        {            
            isLocked = true;
            OnLock ();
        }
    }

    private void UpdateSlider ()
    {
        if (isLocking)
            lockSlider.value = SmoothLerp.Lerp ( lockSlider.value, lockCounter, Time.deltaTime * 1.0f );
        else lockSlider.value = SmoothLerp.Lerp ( lockSlider.value, lockCounter, Time.deltaTime * 3.0f );
    }

    private void OnLock ()
    {
        contraintsRect.GetComponent<Image> ().color = lockedColour;
        crosshairRect.GetComponent<Image> ().color = lockedColour;
        lockSlider.gameObject.SetActive ( false );
        
        if(onLockedToTarget != null)
        {
            onLockedToTarget ( currentTarget );
        }

        firePanel.SetActive(true);
    }

    private void ClearLock ()
    {
        isLocking = false;
        isLocked = false;
        lockCounter = 0.0f;
        contraintsRect.GetComponent<Image> ().color = Color.white;
        crosshairRect.GetComponent<Image> ().color = Color.white;
        contraintsRect.anchoredPosition = Vector2.zero;
        crosshairRect.anchoredPosition = Vector2.zero;
        lockSlider.value = 0.0f;
        lockSlider.gameObject.SetActive ( true );

        firePanel.SetActive(false);
    }

    public bool Show (Action<LockableTarget> onLocked)
    {        
        active = true;
        bodyPanel.SetActive ( active );

        GetTargets ();
        ClearLock ();
        onLockedToTarget += onLocked;
        return true;
    }    

    public void Hide ()
    {        
        active = false;
        bodyPanel.SetActive ( active );
        onLockedToTarget = null;
        crosshairRect.anchoredPosition = Vector2.zero;
        contraintsRect.anchoredPosition = Vector2.zero;
        ClearLock ();
    }

    private void GetTargets ()
    {
        lockableTargets = FindObjectsOfType<LockableTarget> ().ToList ();
        
        for (int i = 0; i < lockableTargets.Count; i++)
        {
            if (lockableTargets[i].GetComponent<PhotonView> ().Owner == null) continue;
            if (PhotonNetwork.LocalPlayer == null) continue;

            //if (lockableTargets[i].GetComponent<PhotonView>().Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            //{
            //    lockableTargets.RemoveAt(i);
            //}
        }
    }

    public LockableTarget GetTarget ()
    {
        if (isLocked && currentTarget != null)
        {
            return currentTarget;
        }
        else return null;
    }
}
