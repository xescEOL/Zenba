using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateScript : MonoBehaviour
{
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    public Toggle mRealTimeToggle;
    public Toggle mKidsToggle;
    public Image mPointsButton;
    public Image mQuestionsButton;
    public Dropdown mDropDown;
    public InputField mNameGame;
    public static bool mQuestionsWin;
    public static bool mRealTime;
    public static bool mKids;
    public static bool mAdmin = false;

    // Start is called before the first frame update
    void Start()
    {
        mAdmin = true;
        mQuestionsButton.color = Color.blue;
        mQuestionsWin = true;
        mRealTimeToggle.isOn = true;
        mRealTime = true;
        mKidsToggle.isOn = false;
        mKids = false;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PointsClick()
    {
        mPointsButton.color = Color.blue;
        mQuestionsButton.color = Color.gray;
        mQuestionsWin = false;
    }

    public void QuestionsClick()
    {
        mPointsButton.color = Color.gray;
        mQuestionsButton.color = Color.blue;
        mQuestionsWin = true;
    }

    public void ShowWaitingPlayers()
    {
        GlobalVariables.mPinGame = Random.Range(00000001, 99999999).ToString("D8");
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("admin").SetValueAsync(auth.CurrentUser.UserId);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("kids").SetValueAsync(mKids);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("playmode").SetValueAsync(mRealTime?0:1);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("winmode").SetValueAsync(mQuestionsWin?0:1);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("winvalue").SetValueAsync(int.Parse(mDropDown.options[mDropDown.value].text));
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("start").SetValueAsync(false);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(0);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("name").SetValueAsync(GlobalVariables.mUserName);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(0);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(0);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("bonus").SetValueAsync(false);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("emoji").SetValueAsync(0);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("boot").SetValueAsync(false);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("currentgame").SetValueAsync(0);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("name").SetValueAsync(mNameGame.text);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("bonus").SetValueAsync(0);
        GlobalVariables.mGameAdminUId = auth.CurrentUser.UserId;
        GlobalVariables.mGameMode = mRealTime ? 0 : 1;
        GlobalVariables.mGameName = mNameGame.text;
        GlobalVariables.mGameWinMode = mQuestionsWin ? 0 : 1;
        GlobalVariables.mGameWinValue = int.Parse(mDropDown.options[mDropDown.value].text);
        GlobalVariables.mCurrentPoints = 0;

        CreateListQuizs();
        SceneManager.LoadScene("WaitingPlayers");
    }

    public void RealTimeChange()
    {
        mRealTime = mRealTimeToggle.isOn;
    }

    public void KidsChange()
    {
        mKids = mKidsToggle.isOn;
    }

    public void CreateListQuizs()
    {
        int numlist = 1;
        GlobalVariables.mListQuizs.Clear();
        while (numlist <= int.Parse(mDropDown.options[mDropDown.value].text))
        {
            int rnd = new System.Random().Next(1, GlobalVariables.mNumQuizs20);
            if (!GlobalVariables.mListQuizs.Contains(rnd + "_0") && !GlobalVariables.mListQuizs.Contains(rnd + "_1"))
            {
                int rnd2 = new System.Random().Next(0, 2);
                reference.Child("games").Child(GlobalVariables.mPinGame).Child("list").Child(numlist.ToString()).SetValueAsync(rnd + "_" + rnd2).ToString();
                GlobalVariables.mListQuizs.Add(rnd + "_" + rnd2);
                numlist++;
            }
        }
    }

}
