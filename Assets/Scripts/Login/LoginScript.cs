using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    public Text mEmailInput;
    public Text mPassInput;
    public Text mNameInputTxt;
    public Text mDebugText;
    public GameObject mNameInput;

    private static Firebase.Auth.FirebaseAuth auth;
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseUser user;


    private void Awake()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            Debug.Log("session abierta");
            Debug.Log("Application Version : " + Application.version);
            SceneManager.LoadScene("Menu");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mNameInput.SetActive(false);
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        InitializeFirebase();
        


    }

    // Update is called once per frame
    void Update()
    {
    }
    public void SignOut()
    {
        auth.SignOut();
    }

    public void LoginPush()
    {
        auth.SignInWithEmailAndPasswordAsync(mEmailInput.text, mPassInput.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            SceneManager.LoadScene("Menu");

        });

    }


        public void SignUpPush()
    {
        if (!mNameInput.activeSelf)
        {
            mNameInput.SetActive(true);
        }
        else
        {
            auth.CreateUserWithEmailAndPasswordAsync(mEmailInput.text, mPassInput.text).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                reference.Child("users").Child(newUser.UserId).Child("name").SetValueAsync(mNameInputTxt.text);
                reference.Child("users").Child(newUser.UserId).Child("games").SetValueAsync(0);
                reference.Child("users").Child(newUser.UserId).Child("points").SetValueAsync(0);
                Debug.Log("session abierta");
                SceneManager.LoadScene("Menu");
            });
        }
    }

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                //mDebugText.text = "Signed out " + user.UserId;
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                //mDebugText.text = "Signed out " + user.UserId;
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

}
