using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class JoinGameScript : MonoBehaviour
{
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    public Button mJoinButton;
    public Text mPinInput;
    public int mPinCorrect;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if(mPinCorrect == 1)
        {
            Debug.Log("SI");
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(0);
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("name").SetValueAsync(GlobalVariables.mUserName);
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(0);
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(0);
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("bonus").SetValueAsync(false);
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("emoji").SetValueAsync(0);

            SceneManager.LoadScene("WaitingPlayers");
        }else if(mPinCorrect == 2)
        {
            Debug.Log("PARTIDA EMPEZADA");
        }
    }

    public void ShowWaitingPlayers()
    {
        //mJoinButton.enabled = false;
        mJoinButton.interactable = false;
        StartCoroutine(CourtineCheckDataSave());
        GlobalVariables.mPinGame = mPinInput.text;

        FirebaseDatabase.DefaultInstance.GetReference("games").Child(GlobalVariables.mPinGame).GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Child("start").Value.ToString().Equals("False")){
                GlobalVariables.mGameMode = int.Parse(snapshot.Child("playmode").Value.ToString());
                GlobalVariables.mGameWinMode = int.Parse(snapshot.Child("winmode").Value.ToString());
                GlobalVariables.mGameWinValue = int.Parse(snapshot.Child("winvalue").Value.ToString());
                GlobalVariables.mGameAdminUId = snapshot.Child("admin").Value.ToString();
                GlobalVariables.mGameName = snapshot.Child("name").Value.ToString();
                GlobalVariables.mCurrentPoints = 0;
                mPinCorrect = 1;
                GlobalVariables.mListQuizs.Clear();
                for (int i = 1; i<= GlobalVariables.mNumQuizs20; i++)
                {
                    GlobalVariables.mListQuizs.Add(snapshot.Child("list").Child(i.ToString()).Value.ToString());
                }
            }
            else
            {
                mPinCorrect = 2;
                //mJoinButton.interactable = true;
            }
        });
    }

    public void ActiveButtonJoin()
    {
        if(mPinInput.text.Length >= 7)
            mJoinButton.interactable = true;
    }

    IEnumerator CourtineCheckDataSave()
    {
        yield return new WaitForSeconds(7);
        mJoinButton.interactable = true;
    }
}
