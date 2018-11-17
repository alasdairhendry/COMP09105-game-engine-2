using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : MonoBehaviour {

    [SerializeField] private GameObject replayableObjectPrefab;
    private ReplayController controller;
    private ReplayCanvas canvas;

    //Is the replayer playing
    private bool isPlaying = false;

    // Starts at 0 and counts through each register index
    private int baseReplayIndex = 0;

    // Starts at the triggered registered index
    //private int relativeReplayIndex = 0;

    // When will we stop replaying
    //private int targetReplayIndex = 0;

    // Current delay between frames
    private float currentDelay = 0.0f;

    // Time scale modifier
    private float delayModifier = 1.0f;

    // Is the Finish() method currently being invoked
    private bool isFinishing = false;

    private bool calledEndOnCanvas = false;

    // Replay queue
    private Queue<ReplayData> replayQueue = new Queue<ReplayData>();

    // The active replay data we are playing
    private ReplayData activeReplay = new ReplayData();

    // How long will this replay last?
    //private float replayLengthSeconds = 0.0f;

    // How far through the current replay are we?
    private float currentReplayTimeFrame = 0.0f;

    // Is this replay being displayed on the Replay Canvas?
    private bool isIntroPlay = false;

    // Are we current waiting for the Replay Canvas to play an intro?
    [HideInInspector] public bool introIsPlaying = false;

    // How long before the end of this Replay will we tell the Replay Canvas to "fade out"
    [SerializeField] private float endCanvasOffset = 0.5f;

    // The target index of replayData[] that we want the camera to follow
    //private int targetIndex = -1;

    // The offest we want the camera to have 
    //private Vector3 targetOffset = new Vector3();

    // The transform of the replay camera
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Vector3 defaultCameraPosition = new Vector3();

    [SerializeField] private Vector3 defaultCameraRotation = new Vector3();

    private void Start()
    {
        controller = GetComponent<ReplayController>();
        canvas = GetComponent<ReplayCanvas>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("XBO_Select")) RequestReplay();

        if (!isPlaying)
        {
            CheckQueue();
            return;
        }

        if (!isPlaying) return;
        if (introIsPlaying) return;
        IncrementReplayTimeframe();
        UpdateReplayObjectPositions();
    }

    private void LateUpdate()
    {
        SetCameraPosition();
    }

    private void CheckQueue()
    {
        //Debug.Log("Checking Queue");
        if (replayQueue.Count > 0)
        {
            //We have a queued replay, so lets play it.
            Debug.Log("Queue count is greater than 0");

            activeReplay = replayQueue.Dequeue();
            baseReplayIndex = 0;
            currentDelay = 0.0f;
            currentReplayTimeFrame = 0.0f;

            CreateGameObjects();
            canvas.Begin();

            introIsPlaying = true;
            isIntroPlay = true;
            isPlaying = true;
            calledEndOnCanvas = false;
        }
    }

    private void IncrementReplayTimeframe()
    {
        currentDelay += Time.deltaTime;
        currentReplayTimeFrame += Time.deltaTime;

        if (currentReplayTimeFrame + endCanvasOffset >= activeReplay.replayLengthSeconds)
        {
            if (!calledEndOnCanvas)
            {
                canvas.End();
                calledEndOnCanvas = true;
            }
        }

        if (currentDelay > controller.ReplayCollectionDelay)
        {
            currentDelay = currentDelay - controller.ReplayCollectionDelay;
            baseReplayIndex++;
            activeReplay.relativeReplayIndex++;

            if (activeReplay.relativeReplayIndex > activeReplay.targetReplayIndex + 1) { Finish(); return; }

            CreateGameObjects();
            CheckActions();
            IncreaseLocalIndices();
        }
    }

    private void UpdateReplayObjectPositions()
    {
        if (activeReplay == null) return;
        if (activeReplay.relativeReplayIndex > activeReplay.targetReplayIndex + 1) { Finish(); return; }

        for (int i = 0; i < activeReplay.data.Count; i++)
        {
            if (activeReplay.data[i].replayerGameobject == null) continue;

            ReplayableData currentDataObject = activeReplay.data[i];

            if (activeReplay.relativeReplayIndex >= currentDataObject.destroyIndex && currentDataObject.destroyIndex != -1)
            {
                DestroyReplayable(i);
            }

            if (currentDataObject.currentReplayIndex == 0) currentDataObject.currentReplayIndex++;
            if (currentDataObject.currentReplayIndex >= currentDataObject.registerIndex.Count) continue;

            currentDataObject.replayerGameobject.transform.position = Vector3.Lerp(currentDataObject.positions[currentDataObject.currentReplayIndex - 1], currentDataObject.positions[currentDataObject.currentReplayIndex], currentDelay / controller.ReplayCollectionDelay);
            currentDataObject.replayerGameobject.transform.rotation = Quaternion.Lerp(currentDataObject.rotations[currentDataObject.currentReplayIndex - 1], currentDataObject.rotations[currentDataObject.currentReplayIndex], currentDelay / controller.ReplayCollectionDelay);
            currentDataObject.replayerGameobject.transform.localScale = Vector3.Lerp(currentDataObject.scales[currentDataObject.currentReplayIndex - 1], currentDataObject.scales[currentDataObject.currentReplayIndex], currentDelay / controller.ReplayCollectionDelay);
        }
    }

    public void RequestReplay(int targetReplayID = -1, Vector3 offset = new Vector3())
    {
        ReplayData data = CreateNewReplayData();
        SetTarget(targetReplayID, offset, data);
        replayQueue.Enqueue(data);
    }

    private ReplayData CreateNewReplayData()
    {
        ReplayData data = new ReplayData();
        data.relativeReplayIndex = controller.ContinuousReplayableCount - controller.MaxReplayIndex;

        if (data.relativeReplayIndex < 0) data.relativeReplayIndex = 0;

        data.targetReplayIndex = controller.ContinuousReplayableCount;

        data.replayLengthSeconds = ((float)data.targetReplayIndex - (float)data.relativeReplayIndex) * controller.ReplayCollectionDelay;

        foreach (KeyValuePair<Replayable, ReplayableData> item in controller.RegisteredReplayables)
        {
            ReplayableData newData = new ReplayableData();

            if (item.Value.mesh != null)
            {
                newData.mesh = item.Value.mesh;
                newData.materials = item.Value.materials;
            }

            newData.name = item.Value.name;
            newData.ID = item.Value.ID;
            newData.destroyIndex = item.Value.destroyIndex;

            for (int i = 0; i < item.Value.positions.Count; i++)
            {
                newData.positions.Add(item.Value.positions[i]);
            }

            for (int i = 0; i < item.Value.rotations.Count; i++)
            {
                newData.rotations.Add(item.Value.rotations[i]);
            }

            for (int i = 0; i < item.Value.scales.Count; i++)
            {
                newData.scales.Add(item.Value.scales[i]);
            }

            for (int i = 0; i < item.Value.registerIndex.Count; i++)
            {
                newData.registerIndex.Add(item.Value.registerIndex[i]);
            }

            for (int i = 0; i < item.Value.actionIDs.Count; i++)
            {
                if (item.Value.actions[i] != null)
                {
                    newData.actions.Add(item.Value.actions[i]);
                    newData.actionIDs.Add(item.Value.actionIDs[i]);
                }
            }

            data.data.Add(newData);
        }

        return data;        
    }

    private void SetTarget(int targetReplayID, Vector3 offset, ReplayData data)
    {
        bool found = false;

        for (int i = 0; i < data.data.Count; i++)
        {
            if (data.data[i].ID == targetReplayID)
            {
                found = true;

                //if (targetIndex >= 0 && !isPlaying)   // Mid replay switch????
                //    cameraTransform.position = activeReplay[i].replayerGameobject.transform.position + offset;

                data.targetIndex = i;
                data.targetOffset = offset;
                break;
            }
        }

        if (!found)
        {
            data.targetIndex = -1;
            data.targetOffset = new Vector3();
        }
    }

    private void SetCameraPosition()
    {
        Vector3 targetPosition = new Vector3();
        Quaternion targetRotation = new Quaternion();

        if (activeReplay != null)
        {
            if (activeReplay.targetIndex >= 0 && activeReplay.targetIndex < activeReplay.data.Count)
            {
                if (activeReplay.data[activeReplay.targetIndex].replayerGameobject != null)
                {
                    targetPosition = activeReplay.data[activeReplay.targetIndex].replayerGameobject.transform.position + activeReplay.targetOffset;

                    Vector3 directionToTarget = activeReplay.data[activeReplay.targetIndex].replayerGameobject.transform.position - cameraTransform.position;
                    targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                }
                else
                {
                    targetPosition = defaultCameraPosition;
                    targetRotation = Quaternion.Euler(defaultCameraRotation);
                }
            }
            else
            {
                targetPosition = defaultCameraPosition;
                targetRotation = Quaternion.Euler(defaultCameraRotation);
            }
        }
        else
        {
            targetPosition = defaultCameraPosition;
            targetRotation = Quaternion.Euler(defaultCameraRotation);
        }

        cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * 15.0f);
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime * 4.0f);
    }

    private void CreateGameObjects()
    {
        for (int i = 0; i < activeReplay.data.Count; i++)
        {            
            if (activeReplay.data[i].replayerGameobject != null) continue;            

            if (activeReplay.relativeReplayIndex - 1 >= activeReplay.data[i].registerIndex[0])
            {
                GameObject go = Instantiate(replayableObjectPrefab);
                go.name = "ReplayerObject";

                if (activeReplay.data[i].mesh != null)
                {
                    MeshRenderer r = go.AddComponent<MeshRenderer>();
                    MeshFilter f = go.AddComponent<MeshFilter>();

                    f.mesh = activeReplay.data[i].mesh;
                    r.materials = activeReplay.data[i].materials;
                }

                go.transform.position = activeReplay.data[i].positions[0];
                go.transform.rotation = activeReplay.data[i].rotations[0];
                go.transform.localScale = activeReplay.data[i].scales[0];

                go.transform.SetParent(this.transform.Find("ReplayObjects"));

                activeReplay.data[i].replayerGameobject = go;
            }
        }
    }

    private void CheckActions()
    {
        for (int i = 0; i < activeReplay.data.Count; i++)
        {
            for (int x = 0; x < activeReplay.data[i].actionIDs.Count; x++)
            {
                if(activeReplay.data[i].actionIDs[x] == activeReplay.relativeReplayIndex - 1)
                {
                    if (activeReplay.data[i].actions[x] != null)
                        activeReplay.data[i].actions[x]();
                }
            }
        }
    }

    private void IncreaseLocalIndices()
    {
        for (int i = 0; i < activeReplay.data.Count; i++)
        {
            if (activeReplay.data[i].replayerGameobject == null) continue;

            activeReplay.data[i].currentReplayIndex++;
        }
    }

    private void DestroyReplayable(int index)
    {
        Destroy(activeReplay.data[index].replayerGameobject);
        activeReplay.data.RemoveAt(index);
    }

    public void Finish()
    {
        if (isFinishing) return;

        isFinishing = true;

        Debug.Log("Finish");

        // delete all gameobjects & reset    
        if (activeReplay != null)
        {
            for (int i = 0; i < activeReplay.data.Count; i++)
            {
                Destroy(activeReplay.data[i].replayerGameobject);
            }
        }

        if (!calledEndOnCanvas)
        {
            canvas.End();
            calledEndOnCanvas = true;
        }

        baseReplayIndex = 0;        
        currentDelay = 0.0f;
        delayModifier = 1.0f;
        currentReplayTimeFrame = 0.0f;        

        activeReplay = null;

        isPlaying = false;
        isIntroPlay = false;
        //calledEndOnCanvas = false;
        isFinishing = false;
    }
	
}

public class ReplayData
{
    public List<ReplayableData> data = new List<ReplayableData>();

    public int relativeReplayIndex;
    public int targetReplayIndex;

    public float replayLengthSeconds;

    public int targetIndex = -1;
    public Vector3 targetOffset;

}
