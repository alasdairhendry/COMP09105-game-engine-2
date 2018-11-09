using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamifyButtons : MonoBehaviour {

    [SerializeField] private string format;

    [ContextMenu ( "Rename Buttons" )]
    public void Buttonify ()
    {
        Button[] b = GetComponentsInChildren<Button> ();

        for (int i = 0; i < b.Length; i++)
        {
            string name = format.Replace ( "$$NAME$$", b[i].GetComponentInChildren<Text> ().text );
            b[i].gameObject.name = name;
        }
    }

}
