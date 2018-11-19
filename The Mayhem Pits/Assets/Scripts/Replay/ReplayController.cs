using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayController : MonoBehaviour {

    private Action replayUpdate;

    [SerializeField] private float holdReplayDataFor = 15.0f;
    [SerializeField] private float replayCollectionDelay = 0.25f;
    [SerializeField] private List<ReplayableData> debugdatas = new List<ReplayableData>();

    private int maxReplayIndex = 0;    
    private float currentCollectionDelay = 0.0f;

    private Dictionary<Replayable, ReplayableData> registeredReplayables = new Dictionary<Replayable, ReplayableData>();
    private int continuousReplayableCount = 0;
    private int IDCounter = 0;

    public float HoldReplayDataFor { get { return holdReplayDataFor; } }
    public float ReplayCollectionDelay { get { return replayCollectionDelay; } }
    public int MaxReplayIndex { get { return maxReplayIndex; } }
    public int ContinuousReplayableCount { get { return continuousReplayableCount; } }
    public Dictionary<Replayable, ReplayableData> RegisteredReplayables { get { return registeredReplayables; } }

    private void Start()
    {
        maxReplayIndex = Mathf.FloorToInt(holdReplayDataFor / replayCollectionDelay);
    }

    private void Update () {
        DoReplayUpdate();        
	}

    private void DoReplayUpdate()
    {
        currentCollectionDelay += Time.deltaTime;

        if(currentCollectionDelay >= replayCollectionDelay)
        {
            currentCollectionDelay = 0.0f;

            if (replayUpdate != null)
                replayUpdate();

            continuousReplayableCount++;
        }

        debugdatas.Clear();

        foreach (KeyValuePair<Replayable, ReplayableData> item in registeredReplayables)
        {
            debugdatas.Add(item.Value);
        }
    }

    public int RegisterAsReplayable(Replayable from)
    {
        if (registeredReplayables.ContainsKey(from)) { Debug.Log("Key already registered"); return - 1; }

        ReplayableData data = new ReplayableData();

        data.name = IDCounter + "  -  " + from.gameObject.name;
        data.ID = IDCounter;
        data.mesh = from.GetComponent<MeshFilter>().mesh;
        data.materials = from.GetComponent<MeshRenderer>().materials;

        registeredReplayables.Add(from, data);
        replayUpdate += from.OnUpdate;
        IDCounter++;
        return IDCounter - 1;
    }

    public void UnRegisterAsReplayable(Replayable from)
    {
        if (!registeredReplayables.ContainsKey(from)) { Debug.Log("Key isnt registered"); return; }

        registeredReplayables[from].destroyIndex = continuousReplayableCount;
        replayUpdate -= from.OnUpdate;        
    }

    public void CollectData(Replayable from, System.Action action = null)
    {
        if (!registeredReplayables.ContainsKey(from)) return;

        if(registeredReplayables[from].positions.Count >= maxReplayIndex)
        {
            registeredReplayables[from].positions.RemoveAt(0);
            registeredReplayables[from].rotations.RemoveAt(0);
            registeredReplayables[from].scales.RemoveAt(0);
            registeredReplayables[from].registerIndex.RemoveAt(0);
        }

        if (registeredReplayables[from].actions.Count >= maxReplayIndex)
        {
            registeredReplayables[from].actions.RemoveAt(0);
        }

        registeredReplayables[from].positions.Add(from.transform.position);
        registeredReplayables[from].rotations.Add(from.transform.rotation);
        registeredReplayables[from].scales.Add(from.transform.lossyScale);
        registeredReplayables[from].registerIndex.Add(continuousReplayableCount);

        if(action != null)
        {
            Debug.Log("Collected action from " + from.gameObject.name);
            registeredReplayables[from].actions.Add(action);
            registeredReplayables[from].actionIDs.Add(continuousReplayableCount);
        }
    }
}

[System.Serializable]
public class ReplayableData
{
    public string name;
    public int ID;
    public Mesh mesh;
    public Material[] materials;

   /* [HideInInspector] */public List<Vector3> positions = new List<Vector3>();
   /* [HideInInspector] */public List<Quaternion> rotations = new List<Quaternion>();
   /* [HideInInspector] */public List<Vector3> scales = new List<Vector3>();
    public List<int> registerIndex = new List<int>();

    public List<Action> actions = new List<Action>();
    public List<int> actionIDs = new List<int>();

    public GameObject replayerGameobject;
    public int currentReplayIndex = 0;
    public int destroyIndex = -1;
}
