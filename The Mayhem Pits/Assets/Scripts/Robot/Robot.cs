using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Robot : NetworkBehaviour {

    public override void OnStartAuthority()
    {        
        if (!hasAuthority) return;
        CmdSpawnRobotGraphics(  MyRobot.singleton.RobotBodyPrefabs.IndexOf(MyRobot.singleton.GetMyRobotData.BodyPrefab),
                                MyRobot.singleton.RobotWeaponPrefabs.IndexOf(MyRobot.singleton.GetMyRobotData.WeaponPrefab),
                                MyRobot.singleton.GetMyRobotData.WeaponMountPosition,
                                MyRobot.singleton.GetMyRobotData.WeaponMountRotation);
    }

    [Command]
    public void CmdSpawnRobotGraphics(int bodyIndex, int weaponIndex, Vector3 weaponPosition, Vector3 weaponRotation)
    {
        MyRobot mr = MyRobot.singleton;
        GameObject bodyGraphics = Instantiate(mr.RobotBodyPrefabs[bodyIndex], this.transform.Find("Graphics"));
        bodyGraphics.transform.localPosition = Vector3.zero;
        bodyGraphics.transform.localEulerAngles = Vector3.zero;
        bodyGraphics.transform.name = "Body";
        NetworkServer.SpawnWithClientAuthority(bodyGraphics, connectionToClient);

        GameObject weaponGraphics = Instantiate(mr.RobotWeaponPrefabs[weaponIndex], this.transform.Find("Graphics"));
        weaponGraphics.transform.localPosition = weaponPosition;
        weaponGraphics.transform.localEulerAngles = weaponRotation;
        weaponGraphics.transform.name = "Weapon";
        NetworkServer.SpawnWithClientAuthority(weaponGraphics, connectionToClient);
    }
}
