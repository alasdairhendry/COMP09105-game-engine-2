using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Flip : Ability {

    public enum Side { left, right }
    [SerializeField] private Side side;
    [SerializeField] private float force;

    protected override void Update()
    {
        base.Update();
    }

    // The actual logic for the ability will go here
    protected override void OnActivate()
    {
        base.OnActivate();

        if (side == Side.left)
        {
            targetRobot.GetComponent<Rigidbody>().AddForceAtPosition(-targetRobot.transform.up * force * Time.fixedDeltaTime, targetRobot.transform.TransformPoint(new Vector3(-0.75f, 0.5f, 0.0f)), ForceMode.Impulse);
            GameObject go = new GameObject { name = "Anchor_Left" };
            go.transform.position = targetRobot.transform.TransformPoint(new Vector3(-0.75f, 0.5f, 0.0f));
            go.transform.SetParent(targetRobot.transform.Find("Anchors"));
        }
        else
        {
            targetRobot.GetComponent<Rigidbody>().AddForceAtPosition(-targetRobot.transform.up * force * Time.fixedDeltaTime, targetRobot.transform.TransformPoint(new Vector3(0.75f, 0.5f, 0.0f)), ForceMode.Impulse);
            GameObject go = new GameObject { name = "Anchor_Right" };
            go.transform.position = targetRobot.transform.TransformPoint(new Vector3(0.75f, 0.5f, 0.0f));
            go.transform.SetParent(targetRobot.transform.Find("Anchors"));
        }

        Finish();
    }
}
