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
    private int relativeReplayIndex = 0;

    // When will we stop replaying
    private int targetReplayIndex = 0;

    // Current delay between frames
    private float currentDelay = 0.0f;

    // Time scale modifier
    private float delayModifier = 1.0f;

    // Is the Finish() method currently being invoked
    private bool isFinishing = false;

    // Replay data info (positions, rotations)
    private List<ReplayableData> replayData = new List<ReplayableData>();

    // How long will this replay last?
    private float replayLengthSeconds = 0.0f;

    // How far through the current replay are we?
    private float currentReplayTimeFrame = 0.0f;

    // Is this replay being displayed on the Replay Canvas?
    private bool canvasPlay = false;

    // Are we current waiting for the Replay Canvas to play an intro?
    [HideInInspector] public bool canvasIsIntro = false;

    // How long before the end of this Replay will we tell the Replay Canvas to "fade out"
    [SerializeField] private float endCanvasOffset = 0.5f;

    // The target index of replayData[] that we want the camera to follow
    private int targetIndex = -1;

    // The offest we want the camera to have 
    private Vector3 targetOffset = new Vector3();

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
        if (Input.GetButtonDown("XBO_Select")) ReplayWithCanvas();

        if (!isPlaying) return;
        if (canvasIsIntro) return;        

        currentDelay += Time.deltaTime /** delayModifier*/;
        currentReplayTimeFrame += Time.deltaTime/* * delayModifier*/;

        if(currentReplayTimeFrame + endCanvasOffset >= replayLengthSeconds)
        {
            canvas.End();
        }

        if(currentDelay > controller.ReplayCollectionDelay)
        {
            currentDelay = currentDelay - controller.ReplayCollectionDelay;        
            baseReplayIndex++;
            relativeReplayIndex++;

            if (relativeReplayIndex > targetReplayIndex - 2)
                delayModifier *= 0.75f;
            if (relativeReplayIndex > targetReplayIndex - 1)
                delayModifier *= 0.75f;

            if (relativeReplayIndex > targetReplayIndex + 1) { Finish(); return; }

            CreateGameObjects();
            CheckActions();
            IncreaseLocalIndices();
        }

        for (int i = 0; i < replayData.Count; i++)
        {
            if (replayData[i].replayerGameobject == null) continue;

            ReplayableData d = replayData[i];

            if (relativeReplayIndex >= d.destroyIndex && d.destroyIndex != -1)
            {
                DestroyReplayable(i);
            }

            if (d.currentReplayIndex == 0) d.currentReplayIndex++;
            if (d.currentReplayIndex >= d.registerIndex.Count) continue;

            d.replayerGameobject.transform.position = Vector3.Lerp(d.positions[d.currentReplayIndex - 1], d.positions[d.currentReplayIndex], currentDelay / controller.ReplayCollectionDelay);
            d.replayerGameobject.transform.rotation = Quaternion.Lerp(d.rotations[d.currentReplayIndex - 1], d.rotations[d.currentReplayIndex], currentDelay / controller.ReplayCollectionDelay);
            d.replayerGameobject.transform.localScale = Vector3.Lerp(d.scales[d.currentReplayIndex - 1], d.scales[d.currentReplayIndex], currentDelay / controller.ReplayCollectionDelay);
        }

    }

    private void LateUpdate()
    {
        SetCameraPosition();
    }

    private void SetCameraPosition()
    {
        Vector3 targetPosition = new Vector3();
        Quaternion targetRotation = new Quaternion();

        if (targetIndex >= 0)
        {
            if (replayData[targetIndex].replayerGameobject != null)
            {
                targetPosition = replayData[targetIndex].replayerGameobject.transform.position + targetOffset;

                Vector3 directionToTarget = replayData[targetIndex].replayerGameobject.transform.position - cameraTransform.position;
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

        cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * 15.0f);
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime * 4.0f);
    }

    private void CreateGameObjects()
    {
        for (int i = 0; i < replayData.Count; i++)
        {
            //if (relativeReplayIndex - 2 >= replayData.Count) continue;

            if (replayData[i].replayerGameobject != null) continue;

            //if(relativeReplayIndex - 1 )

            if (relativeReplayIndex - 1 >= replayData[i].registerIndex[/*baseReplayIndex - 1*/ 0])
            {
                GameObject go = Instantiate(replayableObjectPrefab);
                go.name = "ReplayerObject";

                if (replayData[i].mesh != null)
                {
                    MeshRenderer r = go.AddComponent<MeshRenderer>();
                    MeshFilter f = go.AddComponent<MeshFilter>();

                    f.mesh = replayData[i].mesh;
                    r.materials = replayData[i].materials;
                }

                go.transform.position = replayData[i].positions[/*baseReplayIndex - 1*/ 0];
                go.transform.rotation = replayData[i].rotations[/*baseReplayIndex - 1*/ 0];
                go.transform.localScale = replayData[i].scales[/*baseReplayIndex - 1*/ 0];

                go.transform.SetParent(this.transform.Find("ReplayObjects"));

                replayData[i].replayerGameobject = go;

            }
        }
    }

    private void CheckActions()
    {
        for (int i = 0; i < replayData.Count; i++)
        {
            for (int x = 0; x < replayData[i].actionIDs.Count; x++)
            {
                if(replayData[i].actionIDs[x] == relativeReplayIndex - 1)
                {
                    if (replayData[i].actions[x] != null)
                        replayData[i].actions[x]();
                }
            }
        }
    }

    private void IncreaseLocalIndices()
    {
        for (int i = 0; i < replayData.Count; i++)
        {
            if (replayData[i].replayerGameobject == null) continue;

            replayData[i].currentReplayIndex++;
        }
    }

    public void Replay()
    {
        if (isPlaying) return;

        relativeReplayIndex = controller.ContinuousReplayableCount - controller.MaxReplayIndex;
        if (relativeReplayIndex < 0) relativeReplayIndex = 0;

        targetReplayIndex = controller.ContinuousReplayableCount;

        replayLengthSeconds = ((float)targetReplayIndex - (float)relativeReplayIndex) * controller.ReplayCollectionDelay;

        currentDelay = 0.0f;

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

            replayData.Add(newData);
        }
        
        isPlaying = true;
    }

    public void ReplayWithCanvas()
    {
        if (isPlaying) return;

        Replay();
        canvasIsIntro = true;
        CreateGameObjects();
        canvas.Begin();
        canvasPlay = true;
    }

    public void SetTarget(int replayID, Vector3 offset)
    {
        bool found = false;

        for (int i = 0; i < replayData.Count; i++)
        {
            if(replayData[i].ID == replayID)
            {
                found = true;

                if (targetIndex >= 0 && !isPlaying)   // Mid replay switch????
                    cameraTransform.position = replayData[i].replayerGameobject.transform.position + offset;

                targetIndex = i;
                targetOffset = offset;                
                break;
            }
        }

        if (!found)
        {
            targetIndex = -1;
            targetOffset = new Vector3();
        }
    }

    private void DestroyReplayable(int index)
    {
        Destroy(replayData[index].replayerGameobject);
        replayData.RemoveAt(index);
    }

    public void Finish()
    {
        if (isFinishing) return;

        isFinishing = true;

        // delete all gameobjects & reset        
        for (int i = 0; i < replayData.Count; i++)
        {
            Destroy(replayData[i].replayerGameobject);
        }

        if (canvasPlay)
        {
            canvas.End();
        }

        baseReplayIndex = 0;
        relativeReplayIndex = 0;
        targetReplayIndex = 0;
        currentDelay = 0.0f;
        delayModifier = 1.0f;

        targetIndex = -1;
        targetOffset = new Vector3();

        replayData = new List<ReplayableData>();

        currentReplayTimeFrame = 0.0f;
        replayLengthSeconds = 0.0f;

        isPlaying = false;
        canvasPlay = false;

        isFinishing = false;
    }
	
}
