using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatable : MonoBehaviour {

    [SerializeField] protected float heat;

    [SerializeField] protected float heatMax;
    [SerializeField] protected float visualHeatMax;

    [SerializeField] protected float reductionRate;

    protected List<HeatActionPair> onHeatAdd = new List<HeatActionPair> ();
    protected List<HeatActionPair> onHeatSubtract = new List<HeatActionPair> ();

    [SerializeField] protected List<MeshRenderer> affectedMeshes = new List<MeshRenderer> ();
    [SerializeField] protected List<Material> materials = new List<Material>();

    [SerializeField] protected bool autoDetectMeshes;
    [SerializeField] protected bool detectChildren;
    [SerializeField] protected bool detectInactiveChildren;

    protected struct HeatActionPair
    {
        public System.Action action;
        public float value;
        public bool oneShot;
    }

    protected virtual void Start ()
    {
        if (autoDetectMeshes) DetectMeshes ();

        for (int i = 0; i < affectedMeshes.Count; i++)
        {
            materials.Add ( affectedMeshes[i].material );
        }
    }

    protected virtual void DetectMeshes ()
    {
        MeshRenderer m = GetComponent<MeshRenderer> ();
        if (m != null) AddRenderers ( m );

        if (!detectChildren) return;
        MeshRenderer[] ms = GetComponentsInChildren<MeshRenderer> ( detectInactiveChildren );
        AddRenderers ( ms );
    }

    protected virtual void Update ()
    {
        if (heat > 0) heat -= reductionRate * Time.deltaTime;
        if (heat < 0) heat = 0;
        Visuals ();
    }

    protected virtual void Visuals ()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetFloat ( "_Heat", Mathf.Lerp ( 0, visualHeatMax, heat / heatMax ) );
        }
    }

    protected virtual void SendAddCallbacks (float amount)
    {
        for (int i = 0; i < onHeatAdd.Count; i++)
        {
            if (onHeatAdd[i].value >= heat && onHeatAdd[i].value <= heat + amount)
            {
                onHeatAdd[i].action ();
                if (onHeatAdd[i].oneShot) onHeatAdd.RemoveAt ( i );
            }
        }
    }

    protected virtual void SendSubtractCallbacks (float amount)
    {
        for (int i = 0; i < onHeatAdd.Count; i++)
        {
            if (onHeatAdd[i].value >= heat - amount && onHeatAdd[i].value <= heat)
            {
                onHeatAdd[i].action ();
                if (onHeatAdd[i].oneShot) onHeatAdd.RemoveAt ( i );
            }
        }
    }

    public virtual void Add (float amount)
    {
        Debug.Log ( "Taking heat damage", this );
        SendAddCallbacks ( amount );
        heat += amount;
        heat = Mathf.Clamp ( heat, 0, heatMax );
    }

    public virtual void Subtract (float amount)
    {
        SendSubtractCallbacks ( amount );
        heat -= amount;
        heat = Mathf.Clamp ( heat, 0, heatMax );
    }

    public virtual void RegisterAddAction (System.Action action, float value, bool oneShot)
    {
        if (action == null) return;
        onHeatAdd.Add ( new HeatActionPair () { action = action, value = value, oneShot = oneShot } );
    }

    public virtual void AddRenderers (params MeshRenderer[] renderers)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            //Debug.Log ( "Adding Renderer: " + renderers[i].name, this );
            materials.Add ( renderers[i].material );
        }        
    }
}
