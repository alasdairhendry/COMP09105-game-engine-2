using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : MonoBehaviour {

    [SerializeField] private GameObject replayableObjectPrefab;
    private ReplayController controller;

    private bool isPlaying = false;
    private int baseReplayIndex = 0;
    private int relativeReplayIndex = 0;
    private int targetReplayIndex = 0;

    private float currentDelay = 0.0f;
    private float delayModifier = 1.0f;

    private List<ReplayableData> replayData = new List<ReplayableData>();

    private void Start()
    {
        controller = GetComponent<ReplayController>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Replay();
        if (Input.GetButtonDown("XBO_Start")) Replay();

        if (!isPlaying) return;

        currentDelay += Time.deltaTime * delayModifier;

        if(currentDelay >= controller.ReplayCollectionDelay)
        {
            currentDelay = 0.0f;
            baseReplayIndex++;
            relativeReplayIndex++;

            if (relativeReplayIndex > targetReplayIndex - 2)
                delayModifier *= 0.75f;
            if (relativeReplayIndex > targetReplayIndex - 1)
                delayModifier *= 0.75f;

            if (relativeReplayIndex > targetReplayIndex + 1) { Finish(); return; }

            CheckGameObjects();
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

    private void CheckGameObjects()
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
                MeshRenderer r = go.AddComponent<MeshRenderer>();
                MeshFilter f = go.AddComponent<MeshFilter>();

                f.mesh = replayData[i].mesh;
                r.materials = replayData[i].materials;

                go.transform.position = replayData[i].positions[/*baseReplayIndex - 1*/ 0];
                go.transform.rotation = replayData[i].rotations[/*baseReplayIndex - 1*/ 0];
                go.transform.localScale = replayData[i].scales[/*baseReplayIndex - 1*/ 0];

                replayData[i].replayerGameobject = go;

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

        currentDelay = 0.0f;

        foreach (KeyValuePair<Replayable, ReplayableData> item in controller.RegisteredReplayables)
        {            
            ReplayableData newData = new ReplayableData();

            newData.mesh = item.Value.mesh;
            newData.materials = item.Value.materials;
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

            replayData.Add(newData);
        }
        
        isPlaying = true;
    }

    private void DestroyReplayable(int index)
    {
        Debug.Log("DESTROYING");
        Destroy(replayData[index].replayerGameobject);
        replayData.RemoveAt(index);
    }

    private void Finish()
    {
        // delete all gameobjects & reset        
        for (int i = 0; i < replayData.Count; i++)
        {
            Destroy(replayData[i].replayerGameobject);
        }

        baseReplayIndex = 0;
        relativeReplayIndex = 0;
        targetReplayIndex = 0;
        currentDelay = 0.0f;
        delayModifier = 1.0f;
        replayData = new List<ReplayableData>();

        isPlaying = false;
    }
	
}
