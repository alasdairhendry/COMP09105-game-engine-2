using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_DatabaseLogin_Panel : MonoBehaviour {

    private string insertedUsername = "";
    private string insertedPassword = "";

    [SerializeField] private GameObject bodyPanel;
    [SerializeField] private InfoButton usernamePanelButton;

    [SerializeField] private Text usernameText;
    [SerializeField] private Text passwordText;

    [SerializeField] private InfoButton loginButton;
    [SerializeField] private InfoButton createButton;
    [SerializeField] private InfoButton cancelButton;

    [SerializeField] private Text statusText;
    private HUDSelectionGroup previousSelection;

    public void Open ()
    {
        previousSelection = HUDSelectionManager.GetActiveGroup ();
        GetComponentInChildren<HUDSelectionGroup> (true).SetActiveGroup ();

        bodyPanel.SetActive ( true );
        loginButton.SetEnabled ( true );
        createButton.SetEnabled ( true );
        cancelButton.SetEnabled ( true );

        if (PlayerPrefs.HasKey ( "databaseUsername" ))
        {
            insertedUsername = PlayerPrefs.GetString ( "databaseUsername" );
            usernameText.text = insertedUsername;
        }

        statusText.text = "";
    }

    public void Close ()
    {
        bodyPanel.SetActive ( false );
        usernamePanelButton.SetEnabled ( true );

        usernameText.text = "";
        passwordText.text = "";
        insertedUsername = "";
        insertedPassword = "";

        if (previousSelection == null)
            FindObjectOfType<HUDSelectionGroup> ().SetActiveGroup ();
        else previousSelection.SetActiveGroup ();
    }

	public void OnPress_Username ()
    {
        FindObjectOfType<Keyboard> ().Open ( Keyboard.Mode.shift, (s) => { SetUsername (s); }, GetComponentInChildren<HUDSelectionGroup>(), 12 );
    }

    public void OnPress_Password ()
    {
        FindObjectOfType<Keyboard> ().Open ( Keyboard.Mode.lower, (s) => { SetPassword (s); }, GetComponentInChildren<HUDSelectionGroup> (), 12, true );
    }

    public void OnClick_Login ()
    {
        loginButton.SetEnabled ( false );
        createButton.SetEnabled ( false );
        cancelButton.SetEnabled ( false );

        DatabaseManager.Instance.TryLoginWithPassword ( insertedUsername, insertedPassword,
            (b, s) =>
            {
                if (!b)
                {
                    loginButton.SetEnabled ( true );
                    createButton.SetEnabled ( true );
                    cancelButton.SetEnabled ( true );

                    statusText.text = s;
                }
                else
                {
                    Close ();
                }
            } );            
    }

    public void OnClick_Create ()
    {
        loginButton.SetEnabled ( false );
        createButton.SetEnabled ( false );
        cancelButton.SetEnabled ( false );

        DatabaseManager.Instance.CreateDatabaseAccount ( insertedUsername, insertedPassword,
            (b, s) =>
            {
                if (!b)
                {
                    loginButton.SetEnabled ( true );
                    createButton.SetEnabled ( true );
                    cancelButton.SetEnabled ( true );

                    statusText.text = s;
                }
                else
                {
                    Close ();
                }
            } );
    }

    public void OnClick_Cancel ()
    {
        Close ();
    }

    private void SetUsername(string s)
    {
        if (string.IsNullOrEmpty ( s )) return;
        usernameText.text = s;
        insertedUsername = s;
    }

    private void SetPassword(string s)
    {
        if (string.IsNullOrEmpty ( s )) return;

        string v = "";

        for (int i = 0; i < s.Length; i++)
        {
            v += "*";
        }

        insertedPassword = s;
        passwordText.text = v;
    }
}
