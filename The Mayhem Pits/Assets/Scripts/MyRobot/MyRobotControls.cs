using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyRobotControls : MonoBehaviour {

    private int currentBodyIndex = -1;
    private int currentWeaponIndex = -1;

    private GameObject spawnedBodyPrefab;
    private GameObject spawnedWeaponPrefab;

    [SerializeField] private Text myBodyText;
    [SerializeField] private Text myWeaponText;

    private void Start()
    {
        currentBodyIndex = MyRobot.singleton.BodyDatas.IndexOf ( MyRobot.singleton.GetMyRobotData.BodyData );
        currentWeaponIndex = MyRobot.singleton.WeaponDatas.IndexOf ( MyRobot.singleton.GetMyRobotData.WeaponData );       
        SpawnBody();
    }

    private void Update ()
    {
        transform.GetChild ( 0 ).Rotate ( Vector3.up, Time.deltaTime * Input.GetAxis ( "XBO_RH" ) * 150.0f, Space.World );
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
            Debug.Log("Current Weapon Index Resetting");
        }        

        SpawnWeapon ();
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
                MyRobot.singleton.GetMyRobotData.SetWeaponPrefab(MyRobot.singleton.WeaponDatas[currentWeaponIndex], mount);
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
}
