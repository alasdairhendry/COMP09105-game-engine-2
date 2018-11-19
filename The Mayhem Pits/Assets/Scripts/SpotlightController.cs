using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightController : MonoBehaviour {

    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] private GameObject prefab;

    private Dictionary<NetworkGameRobot, int> indicesTaken = new Dictionary<NetworkGameRobot, int>();

    public void Add(NetworkGameRobot robot)
    {
        if (indicesTaken.ContainsKey(robot))
        {
            Debug.Log("Robot exists in light dictionary");
            return;
        }

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (!indicesTaken.ContainsValue(i))
            {
                indicesTaken.Add(robot, i);

                GameObject go = Instantiate(prefab);
                go.transform.SetParent(spawnPoints[i].transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;

                go.GetComponent<Spotlight>().SetTarget(robot.transform);
                break;
            }
        }
    }

    public void Remove(NetworkGameRobot robot)
    {
        if (indicesTaken.ContainsKey(robot))
        {
            GameObject spotlight = spawnPoints[indicesTaken[robot]].transform.GetChild(0).gameObject;
            if (spotlight != null) Destroy(spotlight);

            indicesTaken.Remove(robot);
        }
    }   
}
