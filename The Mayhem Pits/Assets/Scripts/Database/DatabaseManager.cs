using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour {

    public static DatabaseManager Instance;

    private bool startupCheckPerformed = false;

    [SerializeField] private DatabaseAccount currentAccount;
    [SerializeField] private bool databaseReady = false;
    private FirebaseDatabase database;

    private Text connectionStatusLabel;

    public System.Action<string> onLogin;
    public System.Action onLogout;

    public bool UserIsLoggedIn { get; protected set; }
    //public DatabaseAccount CurrentAccount { get { return currentAccount; } }

    public string AccountUsername { get { return currentAccount.username; } }
    public int AccountCoins { get { return currentAccount.coins; } }

    void Awake () {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy ( this.gameObject );
        DontDestroyOnLoad ( this.gameObject );        

        StartupCheck ();
    }

    private void StartupCheck ()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync ().ContinueWith ( task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp, i.e.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                // where app is a Firebase.FirebaseApp property of your application class.

                // Set a flag here indicating that Firebase is ready to use by your
                // application.

                databaseReady = true;                
            }
            else
            {
                UnityEngine.Debug.LogError ( System.String.Format (
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus ) );
                // Firebase Unity SDK is not safe to use here.
            }
        } );
    }

    private void Update ()
    {
        if (databaseReady)
        {
            if (!startupCheckPerformed)
            {
                startupCheckPerformed = true;
                GetDatabaseReferences ();
                CheckStoredLogin ();
            }

            if (Input.GetKeyDown ( KeyCode.C ))
            {
                if (UserIsLoggedIn)
                    UpdateCoins ( currentAccount.coins + 20 );
            }
        }    
    }

    private void GetDatabaseReferences ()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ( "https://the-mayhem-pits.firebaseio.com/" );

        // Get the root reference location of the database.
        database = FirebaseDatabase.DefaultInstance;
    }   

    public void CheckStoredLogin ()
    {
        if (PlayerPrefs.HasKey ( "databaseUsername" ))
        {
            Debug.Log ( "Found stored login " + PlayerPrefs.GetString ( "databaseUsername" ) );
            TryDefaultLogin ( PlayerPrefs.GetString ( "databaseUsername" ) );
            SetConnectionLabel ( "Found previously used account" );
        }
        else { OnDefaultLoginFailed (); }
    }    

    public void TryDefaultLogin (string username)
    {
        SetConnectionLabel ( "Connecting to previously used account" );

        Debug.Log ( "Try default login" );

        FirebaseDatabase.DefaultInstance.GetReference ( "accounts" ).GetValueAsync ().ContinueWith ( task =>
        {
            if (task.IsFaulted || task.IsCanceled) { Debug.LogError ( "Error retrieving database: " + task.Exception.Message ); OnDefaultLoginFailed(); }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot data in snapshot.Children)
                {                    
                    if(data.Child("username").Value.ToString() == username)
                    {                       
                        SetCurrentAccount ( data );
                        
                        PlayerPrefs.SetString ( "databaseUsername", username );
                        OnDefaultLoginSuccess ();
                        break;
                    }
                }

            }
        } );
    }    

    private void OnDefaultLoginSuccess ()
    {
        Debug.Log ( "Successfully logged in with stored details" );

        UserIsLoggedIn = true;
        if (onLogin != null) onLogin (currentAccount.username);

        SetConnectionLabel ( "Successfully Logged In." );

        if (SceneManager.GetActiveScene ().name == "DatabaseConnection")
            Invoke ( "LoadNextScene", 1.0f );
    }

    private void OnDefaultLoginFailed ()
    {
        // Ask user to login.
        Debug.Log ( "Failed to login with stored details - maybe the user hasnt logged in from this computer before" );
        UserIsLoggedIn = false;
        SetConnectionLabel ( "No Previous Accounts Found." );
        Invoke ( "LoadNextScene", 1.0f );
    }

    public void TryLoginWithPassword (string username, string password, System.Action<bool, string> onReturn)
    {
        Debug.Log ( username + " - " + password );
        FirebaseDatabase.DefaultInstance.GetReference ( "accounts" ).GetValueAsync ().ContinueWith ( task =>
        {
            if (task.IsFaulted || task.IsCanceled) { Debug.LogError ( "Error retrieving database: " + task.Exception.Message ); onReturn ( false, "Error connecting to server." ); }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                bool found = false;
                bool returned = false;


                foreach (DataSnapshot data in snapshot.Children)
                {
                    Debug.Log ( data );

                    if (data.Child ( "username" ).Value.ToString () == username)
                    {
                        
                        if(data.Child("password").Value.ToString() == password)
                        {
                            onReturn ( true, "Login Success" );

                            SetCurrentAccount ( data );

                            UserIsLoggedIn = true;
                            if (onLogin != null) onLogin ( currentAccount.username );

                            PlayerPrefs.SetString ( "databaseUsername", username );

                            returned = true;
                        }
                        else
                        {
                            onReturn ( false, "Incorrect Password." );
                            returned = true;
                        }
                        break;
                    }
                }

                if (!found && !returned)
                {
                    onReturn ( false, "User Account not found." );
                }

            }
        } );
    }

    public void CreateDatabaseAccount (string username, string password, System.Action<bool, string> onReturn)
    {
        bool returned = false;
        bool foundExisting = false;

        FirebaseDatabase.DefaultInstance.GetReference ( "accounts" ).GetValueAsync ().ContinueWith ( checkTask =>
        {
            if (checkTask.IsFaulted || checkTask.IsCanceled) { Debug.LogError ( "Error retrieving database: " + checkTask.Exception.Message ); onReturn ( false, "Error connecting to server." ); if (!returned) returned = true; }
            else if (checkTask.IsCompleted)
            {
                DataSnapshot snapshot = checkTask.Result;

                foreach (DataSnapshot data in snapshot.Children)
                {
                    if(data.Child("username").Value.ToString() == username)
                    {
                        foundExisting = true;
                        if (!returned)
                            onReturn ( false, "Username already exists" );
                        returned = true;
                    }
                }

                if (!foundExisting)
                {
                    DatabaseAccount account = new DatabaseAccount ();
                    account.username = username;
                    account.password = password;
                    account.unlocks.Add ( "0", false );
                    account.id = database.GetReference ( "accounts" ).Push ().Key;

                    database.GetReference ( "accounts" ).Child ( account.id ).SetRawJsonValueAsync ( JsonUtility.ToJson ( account ) ).ContinueWith ( addTask =>
                    {

                        if (addTask.IsFaulted) { Debug.LogError ( "Error creating account: " + addTask.Exception.Message ); onReturn ( false, "Error connecting to server." ); if (!returned) returned = true; }
                        else if (addTask.IsCompleted)
                        {
                       
                            // Account created successfully
                            Debug.Log ( "Account created successfully" );
                            PlayerPrefs.SetString ( "databaseUsername", username );
                            CheckStoredLogin ();

                            if (!returned)
                                onReturn ( true, "Account Created Successfully" );
                            returned = true;
                                             
                        }

                    } );
                }
            }
        } );
    }

    private void SetCurrentAccount (DataSnapshot data)
    {
        currentAccount = new DatabaseAccount ()
        {
            username = data.Child ( "username" ).Value.ToString (),
            coins = int.Parse ( data.Child ( "coins" ).Value.ToString () ),
            id = data.Key
        };
    }

    public void LogOut ()
    {
        currentAccount = null;
        UserIsLoggedIn = false;

        if (onLogout != null)
            onLogout ();
    }

    private void SetConnectionLabel(string status)
    {
        if (SceneManager.GetActiveScene ().name != "DatabaseConnection") return;
        if (connectionStatusLabel == null) connectionStatusLabel = GameObject.Find ( "ConnectionStatus_Label" ).GetComponent<Text>();
        if (connectionStatusLabel == null) return;

        connectionStatusLabel.text = status;
    }

    private void LoadNextScene ()
    {
        SceneLoader.Instance.LoadScene ( "ModeSelect" );
    }

    public void UpdateCoins(int value)
    {
        if (!UserIsLoggedIn) return;

        currentAccount.coins = value;
        FirebaseDatabase.DefaultInstance.GetReference ( "accounts" ).Child ( currentAccount.id ).Child ( "coins" ).SetValueAsync ( value );
    }

    //private void OnSceneChanged(Scene current, Scene next)
    //{
    //    if (!startupCheckPerformed) return;
    //    Debug.Log ( "boop" );
    //    if(next.name == "NetworkConnection")
    //    {
    //        if (UserIsLoggedIn)
    //        {
    //            SceneLoader.Instance.LoadScene ( "ModeSelect" );
    //        }
    //        else
    //        {
    //            GetDatabaseReferences ();
    //            CheckStoredLogin ();
    //        }
    //    }
    //}
}

[System.Serializable]
public class DatabaseAccount
{    
    public string username;
    public string password;
    public string id;
    public int coins = 0;
    public Dictionary<string, object> unlocks = new Dictionary<string, object> ();
}
