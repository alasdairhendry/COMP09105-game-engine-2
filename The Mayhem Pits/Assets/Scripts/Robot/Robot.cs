using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Robot : MonoBehaviour {

    [SerializeField] private GameObject ability_FlipLeftPrefab;
    [SerializeField] private GameObject ability_FlipRightPrefab;

    private List<Ability> abilities = new List<Ability>();    

    private void Start()
    {
        CreateFlipAbilities();
        CreateCOMAnchor();
    }

    private void Update()
    {
        MonitorAbilityInput();
    }

    private void CreateFlipAbilities()
    {
        abilities.Add(Instantiate(ability_FlipLeftPrefab, Vector3.zero, Quaternion.identity, this.transform.Find("Abilities")).GetComponent<Ability>());
        abilities.Add(Instantiate(ability_FlipRightPrefab, Vector3.zero, Quaternion.identity, this.transform.Find("Abilities")).GetComponent<Ability>());
        abilities[0].SetTargetRobot(this);
        abilities[1].SetTargetRobot(this);
    }

    private void CreateCOMAnchor()
    {
        GameObject go = new GameObject { name = "COM" };
        go.transform.SetParent(this.transform.Find("Anchors"));
        go.transform.position = GetComponent<Rigidbody>().worldCenterOfMass;        
    }

    private void MonitorAbilityInput()
    {
        if (Input.GetAxis("XBO_DPAD_Horizontal") > 0)
        {
            abilities[1].Activate();
        }
        else if (Input.GetAxis("XBO_DPAD_Horizontal") < 0)
        {
            abilities[0].Activate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawIcon(GetComponent<Rigidbody>().worldCenterOfMass, "Icon", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Mag: " + collision.relativeVelocity.magnitude);
        if(collision.relativeVelocity.magnitude > 10)
        {
            GetComponent<RobotHealth>().TakeDamage(Mathf.Lerp(5.0f, 15.0f, (collision.relativeVelocity.magnitude - 10) / (25.0f - 10)));
        }
    }
}
