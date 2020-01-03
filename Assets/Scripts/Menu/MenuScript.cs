using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    private bool mUserNameBool = false;
    private bool mLoading100 = false;
    private bool mCurrentVersionInstalled = false;

    public GameObject mLoading;
    public GameObject mPrincipalMenu;
    public GameObject mCreate;
    public GameObject mJoinGame;
    public GameObject mConfigMenu;
    public GameObject mBackMenu;
    public GameObject mTopBar;
    public GameObject mPopup;
    public InputField mUserNameInput;
    public Text mNameTopBar;
    public Text mLevelTopBar;
    public Text mAppInfo;
    Animator mPrincipalMenuAnimator;
    Animator mCreateAnimator;
    Animator mJoinGameAnimator;
    Animator mConfigMenuAnimator;
    Animator mBackMenuAnimator;

    public Texture2D[] frames;
    public int fps = 10;


    public RawImage LoadingImg;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        mAppInfo.text = "Zenba 2020 - Ver: " + Application.version;
        GetCurrentVersion();
        mLoading.SetActive(true);
        mPrincipalMenu.SetActive(false);
        mTopBar.SetActive(false);
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        mPrincipalMenuAnimator = mPrincipalMenu.GetComponent<Animator>();
        mCreateAnimator = mCreate.GetComponent<Animator>();
        mJoinGameAnimator = mJoinGame.GetComponent<Animator>();
        mConfigMenuAnimator = mConfigMenu.GetComponent<Animator>();
        mBackMenuAnimator = mBackMenu.GetComponent<Animator>();
        GlobalVariables.ResetVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mLoading100)
        {
            int index = (int)(Time.time * fps) % frames.Length;
            LoadingImg.texture = frames[index];
        }

        if (GlobalVariables.mUserGames != -999999 && GlobalVariables.mUserPoints != -999999 && !GlobalVariables.mUserName.Equals("") && !mUserNameBool && GlobalVariables.mNumQuizs20 != 0 && mCurrentVersionInstalled)
        {
            //Debug.Log("100%");
            mLoading100 = true;
        }

        if (mLoading100 && mLoading.activeSelf == true)
        {
            mUserNameInput.text = GlobalVariables.mUserName;
            mNameTopBar.text = GlobalVariables.mUserName;
            if(GlobalVariables.mUserGames < 1 || GlobalVariables.mUserPoints < 1)
                mLevelTopBar.text = "0";
            else
                mLevelTopBar.text = ((int)(GlobalVariables.mUserPoints / GlobalVariables.mUserGames)).ToString();

            mLoading.SetActive(false);
            mPrincipalMenu.SetActive(true);
            mTopBar.SetActive(true);
        }

    }

    public void ShowCreateGame()
    {
        mCreate.SetActive(true);
        mBackMenu.SetActive(true);
        if (mPrincipalMenuAnimator != null)
            mPrincipalMenuAnimator.SetBool("clickmenu", true);
        if (mCreateAnimator != null)
            mCreateAnimator.SetBool("clickcreate", true);
        if (mBackMenuAnimator != null)
            mBackMenuAnimator.SetBool("moveback", true);
        /*
        mLoading.SetActive(false);
        mPrincipalMenu.SetActive(false);
        mCreate.SetActive(true);
        mJoinGame.SetActive(false);
        mBackMenu.SetActive(true);
        mConfigMenu.SetActive(false);
        */
    }

    public void ShowJoinGame()
    {
        mJoinGame.SetActive(true);
        mBackMenu.SetActive(true);
        if (mPrincipalMenuAnimator != null)
            mPrincipalMenuAnimator.SetBool("clickmenu", true);
        if (mJoinGameAnimator != null)
            mJoinGameAnimator.SetBool("clickjoin", true);
        if (mBackMenuAnimator != null)
            mBackMenuAnimator.SetBool("moveback", true);
    }

    public void ShowConfigGame()
    {
        mBackMenu.SetActive(true);
        mConfigMenu.SetActive(true);
        if (mPrincipalMenuAnimator != null)
            mPrincipalMenuAnimator.SetBool("clickmenu", true);
        if (mConfigMenuAnimator != null)
            mConfigMenuAnimator.SetBool("clickconf", true);
        if (mBackMenuAnimator != null)
            mBackMenuAnimator.SetBool("moveback", true);
    }

    public void ShowBack()
    {
        GlobalVariables.mPinGame = "";
        if (mPrincipalMenuAnimator != null && mPrincipalMenuAnimator.GetBool("clickmenu") == true)
            mPrincipalMenuAnimator.SetBool("clickmenu", false);
        if (mCreateAnimator != null && mCreateAnimator.GetBool("clickcreate") == true)
            mCreateAnimator.SetBool("clickcreate", false);
        if (mBackMenuAnimator != null && mBackMenuAnimator.GetBool("moveback") == true)
            mBackMenuAnimator.SetBool("moveback", false);
        if (mJoinGameAnimator != null && mJoinGameAnimator.GetBool("clickjoin") == true)
            mJoinGameAnimator.SetBool("clickjoin", false);
        if (mConfigMenuAnimator != null && mConfigMenuAnimator.GetBool("clickconf") == true)
            mConfigMenuAnimator.SetBool("clickconf", false);

        mJoinGame.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>().enabled = true;
    }

    public void ShowFindGame()
    {
        SceneManager.LoadScene("FindGame");
    }

    public void SignOut()
    {
        auth.SignOut();
        SceneManager.LoadScene("Login");
    }

    public void ChangeNameConf()
    {
        reference.Child("users").Child(auth.CurrentUser.UserId).Child("name").SetValueAsync(mUserNameInput.text);
        GlobalVariables.mUserName = mUserNameInput.text;
        mNameTopBar.text = GlobalVariables.mUserName;
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void GetCurrentVersion()
    {
        FirebaseDatabase.DefaultInstance.GetReference("appinfo").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("error retrieve version");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot levelSnapshot = task.Result;
                if (levelSnapshot.Child("version").Value.ToString().Equals(Application.version))
                {
                    auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                    if (auth.CurrentUser != null)
                    {
                        mCurrentVersionInstalled = true;
                    }
                }
                else
                    mPopup.SetActive(true);
            }
        });
    }
}
