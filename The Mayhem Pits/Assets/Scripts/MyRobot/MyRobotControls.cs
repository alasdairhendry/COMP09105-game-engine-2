using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyRobotControls : MonoBehaviour {

    private int currentBodyIndex = -1;
    private int currentWeaponIndex = 0;

    private GameObject spawnedBodyPrefab;
    private GameObject spawnedWeaponPrefab;

    private void Start()
    {        
        currentBodyIndex = MyRobot.singleton.RobotBodyPrefabs.IndexOf(MyRobot.singleton.GetMyRobotData.BodyPrefab);
        currentWeaponIndex = MyRobot.singleton.RobotWeaponPrefabs.IndexOf(MyRobot.singleton.GetMyRobotData.WeaponPrefab);
        SpawnBody();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnClick_NextBody();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnClick_NextWeapon();
        }
    }

    public void OnClick_NextBody()
    {
        currentBodyIndex++;
        if (currentBodyIndex >= MyRobot.singleton.RobotBodyPrefabs.Count)
            currentBodyIndex = 0;

        SpawnBody();
    }

    public void OnClick_NextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= MyRobot.singleton.RobotWeaponPrefabs.Count)
        {
            currentWeaponIndex = 0;
            Debug.Log("Current Weapon Index Resetting");
        }

        SpawnWeapon();
    }

    public void OnClick_Back()
    {
        SceneManager.LoadScene("Menu");
    }

    private void SpawnBody()
    {
        if (spawnedBodyPrefab != null)
            Destroy(spawnedBodyPrefab);

        spawnedBodyPrefab = Instantiate(MyRobot.singleton.RobotBodyPrefabs[currentBodyIndex], transform.Find("Graphics"));
        spawnedBodyPrefab.transform.localPosition = Vector3.zero;
        spawnedBodyPrefab.transform.localEulerAngles = Vector3.zero;

        MyRobot.singleton.GetMyRobotData.SetBodyPrefab(MyRobot.singleton.RobotBodyPrefabs[currentBodyIndex]);

        SpawnWeapon();
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
            if(mount.AcceptedWeaponPrefabs.Contains(MyRobot.singleton.RobotWeaponPrefabs[currentWeaponIndex]))
            {
                found = true;
                spawnedWeaponPrefab = Instantiate(MyRobot.singleton.RobotWeaponPrefabs[currentWeaponIndex], transform.Find("Graphics"));
                spawnedWeaponPrefab.transform.position = mount.transform.position;
                spawnedWeaponPrefab.transform.rotation = mount.transform.rotation;
                MyRobot.singleton.GetMyRobotData.SetWeaponPrefab(MyRobot.singleton.RobotWeaponPrefabs[currentWeaponIndex], mount);
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
