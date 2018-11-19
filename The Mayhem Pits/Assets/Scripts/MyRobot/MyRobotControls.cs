using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyRobotControls : MonoBehaviour {

    private int currentBodyIndex = -1;
    private int currentWeaponIndex = -1;
    private int currentEmblemIndex = -1;
    private int currentSkinIndex = -1;

    private GameObject spawnedBodyPrefab;
    private GameObject spawnedWeaponPrefab;
    private GameObject spawnedEmblemSpringPrefab;
    private GameObject spawnedEmblemPrefab;

    [SerializeField] private GameObject emblemSpringPrefab;

    [SerializeField] private Color green = Color.white;
    [SerializeField] private Color red = Color.white;

    [SerializeField] private Text notLoggedInText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text costText;

    [SerializeField] private Text myBodyText;
    [SerializeField] private Text myWeaponText;
    [SerializeField] private Text myEmblemText;
    [SerializeField] private Text mySkinText;
    [SerializeField] private Text actionText;    

    private float currentZoom = 0.0f;
    private Camera mainCamera;
    private Vector3 originalRootPosition = new Vector3();

    private bool hideInfoText = false;
    private float hideInfoTextCounter = 0.0f;

    private void HideCoinText ()
    {
        hideInfoText = true;
        hideInfoTextCounter = 0.0f;
    }

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();

        if (ClientMode.Instance.GetMode == ClientMode.Mode.Normal)
            transform.Find("Graphics").transform.localPosition = Vector3.zero;
        else transform.Find("Graphics").transform.localPosition = new Vector3(-2.5f, 0.0f, 0.0f);

        originalRootPosition = transform.Find("Graphics").transform.position;

        currentBodyIndex = MyRobot.Instance.BodyDatas.IndexOf ( MyRobot.Instance.GetMyRobotData.BodyData );
        currentWeaponIndex = MyRobot.Instance.WeaponDatas.IndexOf(MyRobot.Instance.GetMyRobotData.WeaponData);
        currentEmblemIndex = MyRobot.Instance.EmblemDatas.IndexOf(MyRobot.Instance.GetMyRobotData.EmblemData);
        currentSkinIndex = MyRobot.Instance.SkinDatas.IndexOf(MyRobot.Instance.GetMyRobotData.SkinData);

        SetActionText_Body ();

        if (!DatabaseManager.Instance.UserIsLoggedIn)
        {
            notLoggedInText.text = "You are not currently logged in. \nPlease create an account to customise your robot." ;
        }

        SpawnBody();
    }

    private void Update ()
    {
        transform.GetChild ( 0 ).Rotate ( Vector3.up, Time.deltaTime * Input.GetAxis ( "XBO_RH" ) * -150.0f, Space.World );
        GetZoom();

        if (hideInfoText)
        {
            hideInfoTextCounter += Time.deltaTime;
            if(hideInfoTextCounter>= 1.5f)
            {
                hideInfoText = false;
                hideInfoTextCounter = 0.0f;
                notLoggedInText.text = "";
            }
        }

        if (DatabaseManager.Instance.UserIsLoggedIn)
        {
            coinsText.text = "£" + DatabaseManager.Instance.AccountCoins.ToString("00");
        }
        else { coinsText.text = "£00"; }
    }

    public void OnClick_Body()
    {
        if (!DatabaseManager.Instance.UserIsLoggedIn) return;

        int id = MyRobot.Instance.BodyDatas[currentBodyIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            // Body is purchased
            MyRobot.Instance.GetMyRobotData.SetBodyData ( MyRobot.Instance.BodyDatas[currentBodyIndex] );
            DatabaseManager.Instance.UpdateRobotData();
        }
        else
        {
            // Body is not purchased
            int coins = DatabaseManager.Instance.AccountCoins;

            Purchasable p = PurchasableManager.Instance.GetPurchasable ( id );
            if (p == null || p.ID < 0) return;

            if(coins >= p.Cost)
            {
                DatabaseManager.Instance.UpdateCoins ( coins - p.Cost );
                PurchasableManager.Instance.UnlockPurchasable ( id );
            }
            else
            {
                notLoggedInText.text = "Not Enough Coins";
                HideCoinText ();
            }
        }
    }

    public void OnClick_Weapon()
    {
        if (!DatabaseManager.Instance.UserIsLoggedIn) return;

        int id = MyRobot.Instance.WeaponDatas[currentWeaponIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            // Body is purchased
            MyRobot.Instance.GetMyRobotData.SetWeaponData ( MyRobot.Instance.WeaponDatas[currentWeaponIndex] );
            DatabaseManager.Instance.UpdateRobotData();
        }
        else
        {
            // Body is not purchased
            int coins = DatabaseManager.Instance.AccountCoins;

            Purchasable p = PurchasableManager.Instance.GetPurchasable ( id );
            if (p == null || p.ID < 0) return;

            if (coins >= p.Cost)
            {
                DatabaseManager.Instance.UpdateCoins ( coins - p.Cost );
                PurchasableManager.Instance.UnlockPurchasable ( id );
            }
            else
            {
                notLoggedInText.text = "Not Enough Coins";
                HideCoinText ();
            }
        }
    }

    public void OnClick_Emblem()
    {
        if (!DatabaseManager.Instance.UserIsLoggedIn) return;

        int id = MyRobot.Instance.EmblemDatas[currentEmblemIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            // Body is purchased
            MyRobot.Instance.GetMyRobotData.SetEmblemData ( MyRobot.Instance.EmblemDatas[currentEmblemIndex] );
            DatabaseManager.Instance.UpdateRobotData();
        }
        else
        {
            // Body is not purchased
            int coins = DatabaseManager.Instance.AccountCoins;

            Purchasable p = PurchasableManager.Instance.GetPurchasable ( id );
            if (p == null || p.ID < 0) return;

            if (coins >= p.Cost)
            {
                DatabaseManager.Instance.UpdateCoins ( coins - p.Cost );
                PurchasableManager.Instance.UnlockPurchasable ( id );
            }
            else
            {
                notLoggedInText.text = "Not Enough Coins";
                HideCoinText ();
            }
        }
    }

    public void OnClick_Skin()
    {
        if (!DatabaseManager.Instance.UserIsLoggedIn) return;

        int id = MyRobot.Instance.SkinDatas[currentSkinIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            // Body is purchased
            MyRobot.Instance.GetMyRobotData.SetSkinData ( MyRobot.Instance.SkinDatas[currentSkinIndex] );
            DatabaseManager.Instance.UpdateRobotData();
        }
        else
        {
            // Body is not purchased
            int coins = DatabaseManager.Instance.AccountCoins;

            Purchasable p = PurchasableManager.Instance.GetPurchasable ( id );
            if (p == null || p.ID < 0) return;

            if (coins >= p.Cost)
            {
                DatabaseManager.Instance.UpdateCoins ( coins - p.Cost );
                PurchasableManager.Instance.UnlockPurchasable ( id );
            }
            else
            {
                notLoggedInText.text = "Not Enough Coins";
                HideCoinText ();
            }
        }
    }

    public void OnClick_Back()
    {
        SceneManager.LoadScene("Menu");
    }

    public void IncrementBody (float direction)
    {
        currentBodyIndex += 1 * Mathf.RoundToInt ( direction );
        if (currentBodyIndex >= MyRobot.Instance.BodyDatas.Count)
            currentBodyIndex = 0;
        if (currentBodyIndex < 0)
            currentBodyIndex = MyRobot.Instance.BodyDatas.Count - 1;

        SetActionText_Body ();

        SpawnBody ();
    }

    public void IncrementWeapon (float direction)
    {
        currentWeaponIndex += 1 * Mathf.RoundToInt ( direction );
        if (currentWeaponIndex >= MyRobot.Instance.WeaponDatas.Count)
            currentWeaponIndex = 0;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = MyRobot.Instance.WeaponDatas.Count - 1;

        SetActionText_Weapon ();

        SpawnWeapon ();
    }

    public void IncrementEmblem (float direction)
    {
        currentEmblemIndex += 1 * Mathf.RoundToInt ( direction );
        if (currentEmblemIndex >= MyRobot.Instance.EmblemDatas.Count)
            currentEmblemIndex = 0;
        if (currentEmblemIndex < 0)
            currentEmblemIndex = MyRobot.Instance.EmblemDatas.Count - 1;

        SetActionText_Emblem ();

        SpawnEmblem ();
    }

    public void IncrementSkin (float direction)
    {
        currentSkinIndex += 1 * Mathf.RoundToInt ( direction );
        if (currentSkinIndex >= MyRobot.Instance.SkinDatas.Count)
            currentSkinIndex = 0;
        if (currentSkinIndex < 0)
            currentSkinIndex = MyRobot.Instance.SkinDatas.Count - 1;

        SetActionText_Skin ();

        SpawnSkin ();
    }

    private void SpawnBody()
    {
        if (spawnedBodyPrefab != null)
            Destroy(spawnedBodyPrefab);

        spawnedBodyPrefab = Instantiate(MyRobot.Instance.BodyDatas[currentBodyIndex].prefab, transform.Find("Graphics"));
        spawnedBodyPrefab.transform.localPosition = Vector3.zero;

        //else spawnedBodyPrefab.transform.localPosition = Vector3.zero + new Vector3(2.5f, 0.0f, 0.0f);

        spawnedBodyPrefab.transform.localEulerAngles = Vector3.zero;        

        myBodyText.text = "BODY: " + MyRobot.Instance.BodyDatas[currentBodyIndex].name.ToUpper ();

        SpawnWeapon ();
        SpawnEmblem();
        SpawnSkin();
    }

    private void SpawnWeapon()
    {
        Debug.Log("Spawn Weapon");
        if (spawnedWeaponPrefab != null)
            Destroy(spawnedWeaponPrefab);

        List<WeaponMount> mounts = spawnedBodyPrefab.GetComponent<RobotBody>().WeaponMounts;

        if(mounts.Count == 0)
        {
            Debug.Log("No mounts, add in inspector");
            return;
        }

        bool found = false;

        foreach (WeaponMount mount in mounts)
        {
            if(mount.AcceptedWeaponPrefabs.Contains(MyRobot.Instance.WeaponDatas[currentWeaponIndex].prefab))
            {
                found = true;
                spawnedWeaponPrefab = Instantiate(MyRobot.Instance.WeaponDatas[currentWeaponIndex].prefab, transform.Find("Graphics"));
                spawnedWeaponPrefab.transform.position = mount.transform.position;
                spawnedWeaponPrefab.transform.rotation = mount.transform.rotation;                
                myWeaponText.text = "WEAPON: " + MyRobot.Instance.WeaponDatas[currentWeaponIndex].name.ToUpper ();
                return;
            }
        }

        if (!found)
        {
            OnClick_Weapon();
            Debug.Log("Weapon not found");
        }
    }

    private void SpawnEmblem()
    {
        Debug.Log("SpawnEmblem");

        if (spawnedEmblemPrefab != null)
            Destroy(spawnedEmblemPrefab);

        if (spawnedEmblemSpringPrefab != null)
            Destroy(spawnedEmblemSpringPrefab);

        EmblemMount mount = spawnedBodyPrefab.GetComponent<RobotBody>().EmblemMount;

        if(mount == null)
        {
            Debug.Log("No mounts, add in inspector");
            return;
        }

        spawnedEmblemSpringPrefab = Instantiate(emblemSpringPrefab);
        spawnedEmblemSpringPrefab.transform.SetParent(spawnedBodyPrefab.GetComponentInChildren<EmblemMount>().transform);
        spawnedEmblemSpringPrefab.transform.localPosition = Vector3.zero;
        spawnedEmblemSpringPrefab.transform.localEulerAngles = Vector3.zero;

        spawnedEmblemPrefab = Instantiate(MyRobot.Instance.EmblemDatas[currentEmblemIndex].prefab);
        spawnedEmblemPrefab.transform.SetParent(spawnedEmblemSpringPrefab.transform.Find("Root").Find("Mount"));
        spawnedEmblemPrefab.transform.localPosition = Vector3.zero;
        spawnedEmblemPrefab.transform.localRotation = Quaternion.identity;
        
        myEmblemText.text = "EMBLEM: " + MyRobot.Instance.EmblemDatas[currentEmblemIndex].name.ToUpper();    
    }

    private void SpawnSkin()
    {
        spawnedBodyPrefab.GetComponent<MeshRenderer>().material.SetTexture( "_MainTex", MyRobot.Instance.SkinDatas[currentSkinIndex].texture);        
        mySkinText.text = "SKIN: " + MyRobot.Instance.SkinDatas[currentSkinIndex].name.ToUpper();
    }

    public void SetActionText_Body ()
    {
        int id = MyRobot.Instance.BodyDatas[currentBodyIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            if (MyRobot.Instance.BodyDatas[currentBodyIndex] == MyRobot.Instance.GetMyRobotData.BodyData)
            {
                actionText.text = "Applied";
                costText.text = "Applied";
            }
            else
            {
                actionText.text = "Apply Body";
                costText.text = "Purchased";
            }            
        }
        else
        {
            actionText.text = "Purchase Body";
            costText.text = "Cost: £" + PurchasableManager.Instance.GetPurchasable ( id ).Cost.ToString ( "00" );
        }        
    }

    public void SetActionText_Weapon ()
    {
        int id = MyRobot.Instance.WeaponDatas[currentWeaponIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            if (MyRobot.Instance.WeaponDatas[currentWeaponIndex] == MyRobot.Instance.GetMyRobotData.WeaponData)
            {
                actionText.text = "Applied";
                costText.text = "Applied";
            }
            else
            {
                actionText.text = "Apply Weapon";
                costText.text = "Purchased";
            }                       
        }
        else
        {
            actionText.text = "Purchase Weapon";
            costText.text = "Cost: £" + PurchasableManager.Instance.GetPurchasable ( id ).Cost.ToString ( "00" );
        }
    }

    public void SetActionText_Emblem ()
    {
        int id = MyRobot.Instance.EmblemDatas[currentEmblemIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            if (MyRobot.Instance.EmblemDatas[currentEmblemIndex] == MyRobot.Instance.GetMyRobotData.EmblemData)
            {
                actionText.text = "Applied";
                costText.text = "Applied";
            }
            else
            {
                actionText.text = "Apply Emblem";
                costText.text = "Purchased";
            }
        }
        else
        {
            actionText.text = "Purchase Emblem";
            costText.text = "Cost: £" + PurchasableManager.Instance.GetPurchasable ( id ).Cost.ToString ( "00" );
        }
    }

    public void SetActionText_Skin ()
    {
        int id = MyRobot.Instance.SkinDatas[currentSkinIndex].ID;
        if (PurchasableManager.Instance.CheckUnlocked ( id ))
        {
            if (MyRobot.Instance.SkinDatas[currentSkinIndex] == MyRobot.Instance.GetMyRobotData.SkinData)
            {
                actionText.text = "Applied";
                costText.text = "Applied";
            }
            else
            {
                actionText.text = "Apply Skin";
                costText.text = "Purchased";
            }            
        }
        else
        {
            actionText.text = "Purchase Skin";
            costText.text = "Cost: £" + PurchasableManager.Instance.GetPurchasable ( id ).Cost.ToString ( "00" );
        }
    }

    public void SetActionText_Back ()
    {
        actionText.text = "Select";
        costText.text = "";
    }

    public void ResetGraphics ()
    {
        currentBodyIndex = MyRobot.Instance.BodyDatas.IndexOf ( MyRobot.Instance.GetMyRobotData.BodyData );
        currentWeaponIndex = MyRobot.Instance.WeaponDatas.IndexOf ( MyRobot.Instance.GetMyRobotData.WeaponData );
        currentEmblemIndex = MyRobot.Instance.EmblemDatas.IndexOf ( MyRobot.Instance.GetMyRobotData.EmblemData );
        currentSkinIndex = MyRobot.Instance.SkinDatas.IndexOf ( MyRobot.Instance.GetMyRobotData.SkinData );
        SpawnBody ();
    }

    private void GetZoom()
    {
        currentZoom = SmoothLerp.Lerp(currentZoom, Input.GetAxis("XBO_RT"), Time.deltaTime * 0.5f);
        currentZoom = Mathf.Clamp(currentZoom, 0.0f, 1.0f);

        Vector3 targetPosition = new Vector3();

        //if (ClientMode.Instance.GetMode == ClientMode.Mode.Normal)
            targetPosition = mainCamera.transform.position + -new Vector3(0.0f, 0.5f, -2.0f);
        //else targetPosition = mainCamera.transform.position + -new Vector3(0.0f, 0.5f, -2.0f);

        transform.Find("Graphics").transform.position = Vector3.Lerp(originalRootPosition, targetPosition, currentZoom);      
    }

}
