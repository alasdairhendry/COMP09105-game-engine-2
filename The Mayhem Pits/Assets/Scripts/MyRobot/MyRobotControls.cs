using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyRobotControls : MonoBehaviour {

    [SerializeField] private GameObject emblemSpringPrefab;

    private int currentBodyIndex = -1;
    private int currentWeaponIndex = -1;
    private int currentEmblemIndex = -1;
    private int currentSkinIndex = -1;

    private GameObject spawnedBodyPrefab;
    private GameObject spawnedWeaponPrefab;
    private GameObject spawnedEmblemSpringPrefab;
    private GameObject spawnedEmblemPrefab;

    [SerializeField] private Text myBodyText;
    [SerializeField] private Text myWeaponText;
    [SerializeField] private Text myEmblemText;
    [SerializeField] private Text mySkinText;

    private float currentZoom = 0.0f;
    private Camera camera;
    private Vector3 originalRootPosition = new Vector3();

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        originalRootPosition = transform.Find("Graphics").transform.position;

        currentBodyIndex = MyRobot.singleton.BodyDatas.IndexOf ( MyRobot.singleton.GetMyRobotData.BodyData );
        currentWeaponIndex = MyRobot.singleton.WeaponDatas.IndexOf(MyRobot.singleton.GetMyRobotData.WeaponData);
        currentEmblemIndex = MyRobot.singleton.EmblemDatas.IndexOf(MyRobot.singleton.GetMyRobotData.EmblemData);
        currentSkinIndex = MyRobot.singleton.SkinDatas.IndexOf(MyRobot.singleton.GetMyRobotData.SkinData);
        SpawnBody();
    }

    private void Update ()
    {
        transform.GetChild ( 0 ).Rotate ( Vector3.up, Time.deltaTime * Input.GetAxis ( "XBO_RH" ) * 150.0f, Space.World );
        GetZoom();
    }

    public void OnClick_NextBody()
    {
        currentBodyIndex++;
        if (currentBodyIndex >= MyRobot.singleton.BodyDatas.Count)
            currentBodyIndex = 0;        

        SpawnBody();
    }

    public void OnClick_NextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= MyRobot.singleton.WeaponDatas.Count)
        {
            currentWeaponIndex = 0;            
        }

        SpawnWeapon();
    }

    public void OnClick_NextEmblem()
    {
        currentEmblemIndex++;
        if (currentEmblemIndex >= MyRobot.singleton.EmblemDatas.Count)
        {
            currentEmblemIndex = 0;            
        }

        SpawnEmblem();
    }

    public void OnClick_NextSkin()
    {
        currentSkinIndex++;
        if (currentSkinIndex >= MyRobot.singleton.SkinDatas.Count)
        {
            currentSkinIndex = 0;            
        }

        SpawnSkin();
    }

    public void OnClick_Back()
    {
        SceneManager.LoadScene("Menu");
    }

    private void SpawnBody()
    {
        if (spawnedBodyPrefab != null)
            Destroy(spawnedBodyPrefab);

        spawnedBodyPrefab = Instantiate(MyRobot.singleton.BodyDatas[currentBodyIndex].prefab, transform.Find("Graphics"));
        spawnedBodyPrefab.transform.localPosition = Vector3.zero;
        spawnedBodyPrefab.transform.localEulerAngles = Vector3.zero;

        MyRobot.singleton.GetMyRobotData.SetBodyData(MyRobot.singleton.BodyDatas[currentBodyIndex]);

        myBodyText.text = "BODY: " + MyRobot.singleton.BodyDatas[currentBodyIndex].name.ToUpper ();

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
            if(mount.AcceptedWeaponPrefabs.Contains(MyRobot.singleton.WeaponDatas[currentWeaponIndex].prefab))
            {
                found = true;
                spawnedWeaponPrefab = Instantiate(MyRobot.singleton.WeaponDatas[currentWeaponIndex].prefab, transform.Find("Graphics"));
                spawnedWeaponPrefab.transform.position = mount.transform.position;
                spawnedWeaponPrefab.transform.rotation = mount.transform.rotation;
                MyRobot.singleton.GetMyRobotData.SetWeaponData(MyRobot.singleton.WeaponDatas[currentWeaponIndex], mount);
                myWeaponText.text = "WEAPON: " + MyRobot.singleton.WeaponDatas[currentWeaponIndex].weaponName.ToUpper ();
                return;
            }
        }

        if (!found)
        {
            OnClick_NextWeapon();
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

        spawnedEmblemPrefab = Instantiate(MyRobot.singleton.EmblemDatas[currentEmblemIndex].prefab);
        spawnedEmblemPrefab.transform.SetParent(spawnedEmblemSpringPrefab.transform.Find("Root").Find("Mount"));
        spawnedEmblemPrefab.transform.localPosition = Vector3.zero;
        spawnedEmblemPrefab.transform.localRotation = Quaternion.identity;

        MyRobot.singleton.GetMyRobotData.SetEmblemData(MyRobot.singleton.EmblemDatas[currentEmblemIndex], mount);
        myEmblemText.text = "EMBLEM: " + MyRobot.singleton.EmblemDatas[currentEmblemIndex].emblemName.ToUpper();    
    }

    private void SpawnSkin()
    {
        spawnedBodyPrefab.GetComponent<MeshRenderer>().material.SetTexture("_Albedo", MyRobot.singleton.SkinDatas[currentSkinIndex].texture);
        MyRobot.singleton.GetMyRobotData.SetSkinData(MyRobot.singleton.SkinDatas[currentSkinIndex]);
        mySkinText.text = "SKIN: " + MyRobot.singleton.SkinDatas[currentSkinIndex].skinName.ToUpper();
    }

    private void GetZoom()
    {
        currentZoom = SmoothLerp.Lerp(currentZoom, Input.GetAxis("XBO_RT"), Time.deltaTime * 0.5f);
        currentZoom = Mathf.Clamp(currentZoom, 0.0f, 1.0f);

        Vector3 targetPosition = camera.transform.position +- new Vector3(0.0f, 0.5f, -2.0f);
        transform.Find("Graphics").transform.position = Vector3.Lerp(originalRootPosition, targetPosition, currentZoom);      
    }

}
