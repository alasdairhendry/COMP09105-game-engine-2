using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeed : MonoBehaviourPunCallbacks {

    public static KillFeed Instance;
    public enum InfoType { Joined, Disconnect, Killed, Spectate, Ability, Weapon, Flameout }

    private void Awake ()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    [SerializeField] private GameObject informationPrefab;
    [SerializeField] private List<InfoTypeSprite> infoTypeSpritePairs = new List<InfoTypeSprite> ();
    [SerializeField] private int maxInfoAtOnce = 6;

    private List<GameObject> infoObjects = new List<GameObject> ();

    [System.Serializable]
    public struct InfoTypeSprite
    {
        public InfoType type;
        public Sprite sprite;
    }

    private void Update ()
    {
        if (Input.GetKeyDown ( KeyCode.K ))
        {
            AddInfo ( (Random.value * 100.0f).ToString () );
        }
    }

    public void AddInfo (string info)
    {
        GameObject go = Instantiate ( informationPrefab );
        go.transform.SetParent ( this.transform );
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;

        go.GetComponentInChildren<Text> ().text = info;
        Destroy ( go.GetComponentInChildren<Image> ().transform.parent.gameObject );
        infoObjects.Add ( go );
        CheckMaxObjects ();
    }

    public void AddInfo (string info, InfoType type)
    {
        GameObject go = Instantiate ( informationPrefab );
        go.transform.SetParent ( this.transform );
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;

        go.GetComponentInChildren<Text> ().text = info;
        go.GetComponentInChildren<Image> ().sprite = GetSprite ( type );
        infoObjects.Add ( go );
        CheckMaxObjects ();
    }

    public void AddInfo (string info, InfoType type, Photon.Pun.RpcTarget target)
    {
        if (target == RpcTarget.Others || target == RpcTarget.OthersBuffered)
        {
            GameObject go = Instantiate ( informationPrefab );
            go.transform.SetParent ( this.transform );
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;

            go.GetComponentInChildren<Text> ().text = info;
            go.GetComponentInChildren<Image> ().sprite = GetSprite ( type );
            infoObjects.Add ( go );
            CheckMaxObjects ();
        }

        photonView.RPC ( "RPCAddInfo", target, info, (int)type );
    }

    [PunRPC]
    private void RPCAddInfo (string info, int intType)
    {
        InfoType type = (InfoType)intType;

        GameObject go = Instantiate ( informationPrefab );
        go.transform.SetParent ( this.transform );
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;

        go.GetComponentInChildren<Text> ().text = info;
        go.GetComponentInChildren<Image> ().sprite = GetSprite ( type );
        infoObjects.Add ( go );
        CheckMaxObjects ();        
    }

    private void CheckMaxObjects ()
    {
        if(infoObjects.Count > maxInfoAtOnce)
        {
            GameObject go = infoObjects[0];
            infoObjects.RemoveAt ( 0 );
            Destroy ( go );
        }
    }

    private Sprite GetSprite(InfoType type)
    {
        for (int i = 0; i < infoTypeSpritePairs.Count; i++)
        {
            if(type == infoTypeSpritePairs[i].type)
            {
                return infoTypeSpritePairs[i].sprite;
            }
        }

        Debug.LogError ( "Couldnt find sprite for given type" );
        return infoTypeSpritePairs[0].sprite;
    }
}
