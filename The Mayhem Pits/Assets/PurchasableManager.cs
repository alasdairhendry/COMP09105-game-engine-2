using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchasableManager : MonoBehaviour {

    [SerializeField] private List<Purchasable> purchasables = new List<Purchasable> ();    

    public static PurchasableManager Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( gameObject );

        DontDestroyOnLoad ( this.gameObject );

        CheckIDs ();
        AssignIDs ();
    }

    private void CheckIDs ()
    {
        for (int i = 0; i < purchasables.Count; i++)
        {
            for (int x = 0; x < purchasables.Count; x++)
            {
                if (x == i) continue;

                if (purchasables[i].ID == purchasables[x].ID)
                {
                    Debug.LogError ( "Duplicate ID found " + i );
                }

                if (purchasables[i].Name == purchasables[x].Name)
                {
                    Debug.LogError ( "Duplicate Name found " + purchasables[i].Name + " - " + i + " - " + x );
                }
            }
        }
    }

    private void AssignIDs ()
    {
        List<CustomRobotData> datas = MyRobot.Instance.GetCustomDatas;       

        for (int i = 0; i < datas.Count; i++)
        {
            for (int x = 0; x < purchasables.Count; x++)
            {
                if(datas[i].name == purchasables[x].Name)
                {
                    datas[i].SetID ( purchasables[x].ID );
                }
            }
        }
    }

    public Purchasable GetPurchasable(int id)
    {
        for (int i = 0; i < purchasables.Count; i++)
        {
            if (id == purchasables[i].ID)
            {
                return purchasables[i];
            }
        }

        return null;
    }

    public bool UnlockPurchasable(int id)
    {
        for (int i = 0; i < purchasables.Count; i++)
        {
            if(id == purchasables[i].ID)
            {
                if (purchasables[i].Unlocked) return false;
                else { purchasables[i].Unlock (); return true; }
            }
        }

        return false;
    }

    public bool CheckUnlocked(int id)
    {
        for (int i = 0; i < purchasables.Count; i++)
        {
            if (id == purchasables[i].ID)
            {
                if (purchasables[i].Unlocked) return true;
                else { return false; }
            }
        }

        return false;
    }
}
