using UnityEngine;

public class ObjectDisplayMode : MonoBehaviour {

    [SerializeField] private Vector3 normalPosition;
    [SerializeField] private Vector3 normalRotation;

    [SerializeField] private Vector3 vrPosition;
    [SerializeField] private Vector3 vrRotation;

	// Use this for initialization
	void Start () {
        if (ClientMode.Instance.GetMode == ClientMode.Mode.Normal)
            SetDisplayNormal();
        else SetDisplayVR();
	}

    private void SetDisplayNormal()
    {
        transform.localPosition = normalPosition;
        transform.localEulerAngles = normalRotation;
    }

    private void SetDisplayVR()
    {
        transform.localPosition = vrPosition;
        transform.localEulerAngles = vrRotation;
    }
}
