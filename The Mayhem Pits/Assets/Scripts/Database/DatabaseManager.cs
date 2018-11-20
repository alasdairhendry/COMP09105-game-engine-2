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
    public bool StartupCheckFlag { set { startupCheckPerformed = value; } }

    [SerializeField] private DatabaseAccount currentAccount;
    [SerializeField] private bool databaseReady = false;
    private FirebaseDatabase database;

    private Text connectionStatusLabel;

    public System.Action<string> onLogin;
    public System.Action onLogout;

    public bool UserIsLoggedIn { get; protected set; }    

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

            //if (Input.GetKeyDown ( KeyCode.C ))
            //{
            //    if (UserIsLoggedIn)
            //        UpdateCoins ( currentAccount.coins + 20 );
            //}
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
            SetConnectionLabel ( "Found previously used account" );
            TryDefaultLogin ( PlayerPrefs.GetString ( "databaseUsername" ) );
        }
        else { OnDefaultLoginFailed (); }
    }    

    public void TryDefaultLogin (string username)
    {
        SetConnectionLabel ( "Connecting to previously used account" );

        FirebaseDatabase.DefaultInstance.GetReference ( "accounts" ).GetValueAsync ().ContinueWith ( task =>
        {
            if (task.IsFaulted || task.IsCanceled) { Debug.LogError ("(TryDefaultLogin) - Error retrieving database: " + task.Exception.Message ); OnDefaultLoginFailed(); }
            else if (task.IsCompleted)
            {                
                DataSnapshot snapshot = task.Result;

                bool foundChild = false;

                foreach (DataSnapshot data in snapshot.Children)
                {                    
                    if(data.Child("username").Value.ToString() == username)
                    {

                        foundChild = true;
                        SetCurrentAccount ( data );
                        
                        PlayerPrefs.SetString ( "databaseUsername", username );
                        OnDefaultLoginSuccess ();
                        break;
                    }
                }

                if (!foundChild)
                {
                    OnDefaultLoginFailed();
                }

            }
        } );
    }    

    private void OnDefaultLoginSuccess ()
    {
        UserIsLoggedIn = true;
        if (onLogin != null) onLogin (currentAccount.username);

        SetConnectionLabel ( "Successfully Logged In." );

        if (SceneManager.GetActiveScene ().name == "DatabaseConnection")
            Invoke ( "LoadNextScene", 1.0f );
    }

    private void OnDefaultLoginFailed ()
    {
        // Ask user to login.
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
                    account.unlocks.Add(0);
                    account.unlocks.Add(3);
                    account.unlocks.Add(5);
                    account.unlocks.Add(9);
                    account.id = database.GetReference ( "accounts" ).Push ().Key;

                    database.GetReference ( "accounts" ).Child ( account.id ).SetRawJsonValueAsync ( JsonUtility.ToJson ( account ) ).ContinueWith ( addTask =>
                    {

                        if (addTask.IsFaulted) { Debug.LogError ( "Error creating account: " + addTask.Exception.Message ); onReturn ( false, "Error connecting to server." ); if (!returned) returned = true; }
                        else if (addTask.IsCompleted)
                        {
                       
                            // Account created successfully
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
        currentAccount = new DatabaseAccount()
        {
            username = data.Child("username").Value.ToString(),
            coins = int.Parse(data.Child("coins").Value.ToString()),

            bodyIndex = int.Parse(data.Child("bodyIndex").Value.ToString()),
            weaponIndex = int.Parse(data.Child("weaponIndex").Value.ToString()),
            emblemIndex = int.Parse(data.Child("emblemIndex").Value.ToString()),
            skinIndex = int.Parse(data.Child("skinIndex").Value.ToString()),

            id = data.Key
            //unlocks = (List<int>)data.Child("unlocks").Value
        };
       
        foreach (DataSnapshot child in data.Child("unlocks").Children)
        {
            currentAccount.unlocks.Add(int.Parse(child.Value.ToString()));
        }   

        PurchasableManager.Instance.SetUnlocksFromDatabase(currentAccount.unlocks);  

        MyRobot.Instance.GetMyRobotData.SetBodyData(MyRobot.Instance.BodyDatas[currentAccount.bodyIndex]);
        MyRobot.Instance.GetMyRobotData.SetWeaponData(MyRobot.Instance.WeaponDatas[currentAccount.weaponIndex]);
        MyRobot.Instance.GetMyRobotData.SetEmblemData(MyRobot.Instance.EmblemDatas[currentAccount.emblemIndex]);
        MyRobot.Instance.GetMyRobotData.SetSkinData(MyRobot.Instance.SkinDatas[currentAccount.skinIndex]);
    }

    public void LogOut ()
    {
        currentAccount = null;
        UserIsLoggedIn = false;
        PurchasableManager.Instance.SetUnlocksFromDatabase(new List<int>() { 0, 3, 5, 9 });
        MyRobot.Instance.GetMyRobotData.SetBodyData(MyRobot.Instance.BodyDatas[0]);
        MyRobot.Instance.GetMyRobotData.SetWeaponData(MyRobot.Instance.WeaponDatas[0]);
        MyRobot.Instance.GetMyRobotData.SetEmblemData(MyRobot.Instance.EmblemDatas[0]);
        MyRobot.Instance.GetMyRobotData.SetSkinData(MyRobot.Instance.SkinDatas[0]);

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
        FirebaseDatabase.DefaultInstance.GetReference("accounts").Child(currentAccount.id).Child("coins").SetValueAsync(value);
    }

    public void AddCoins(int value)
    {
        if (!UserIsLoggedIn) return;

        currentAccount.coins += value;
        FirebaseDatabase.DefaultInstance.GetReference("accounts").Child(currentAccount.id).Child("coins").SetValueAsync(currentAccount.coins);
    }

    public void UpdateUnlocks(params int[] IDs)
    {
        if (!UserIsLoggedIn) return;
        if (IDs.Length <= 0) return;

        for (int i = 0; i < IDs.Length; i++)
        {
            currentAccount.unlocks.Add(IDs[i]);            
            FirebaseDatabase.DefaultInstance.GetReference("accounts").Child(currentAccount.id).Child("unlocks").Push().SetValueAsync(IDs[i]);            
        }        
    }

    public void UpdateRobotData()
    {
        if (!UserIsLoggedIn) return;

        currentAccount.bodyIndex = MyRobot.Instance.BodyDatas.IndexOf(MyRobot.Instance.GetMyRobotData.BodyData);
        currentAccount.weaponIndex = MyRobot.Instance.WeaponDatas.IndexOf(MyRobot.Instance.GetMyRobotData.WeaponData);
        currentAccount.emblemIndex = MyRobot.Instance.EmblemDatas.IndexOf(MyRobot.Instance.GetMyRobotData.EmblemData);
        currentAccount.skinIndex = MyRobot.Instance.SkinDatas.IndexOf(MyRobot.Instance.GetMyRobotData.SkinData);

        FirebaseDatabase.DefaultInstance.GetReference("accounts").Child(currentAccount.id).Child("bodyIndex").SetValueAsync(currentAccount.bodyIndex);
        FirebaseDatabase.DefaultInstance.GetReference("accounts").Child(currentAccount.id).Child("weaponIndex").SetValueAsync(currentAccount.weaponIndex);
        FirebaseDatabase.DefaultInstance.GetReference("accounts").Child(currentAccount.id).Child("emblemIndex").SetValueAsync(currentAccount.emblemIndex);
        FirebaseDatabase.DefaultInstance.GetReference("accounts").Child(currentAccount.id).Child("skinIndex").SetValueAsync(currentAccount.skinIndex);
    }
}

[System.Serializable]
public class DatabaseAccount
{    
    public string username;
    public string password;
    public string id;

    public int bodyIndex = 0;
    public int weaponIndex = 0;
    public int emblemIndex = 0;
    public int skinIndex = 0;

    public int coins = 0;        
    public List<int> unlocks = new List<int>();
}
