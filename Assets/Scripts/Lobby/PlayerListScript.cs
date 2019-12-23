using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerListScript : MonoBehaviour
{
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    public GameObject mSecondsTextGame;
    public GameObject mInfoTextGame;
    public LobbyScript mLobby;
    public GameObject mPlayer1;
    public GameObject mPlayer2;
    public GameObject mPlayer3;
    public GameObject mPlayer4;
    public GameObject mPlayer5;
    public GameObject mPlayer6;
    public GameObject mPlayer7;
    public GameObject mPlayer8;
    public GameObject mPlayer9;
    public GameObject mPlayer10;
    public GameObject mPlayer11;
    public GameObject mPlayer12;
    public GameObject mPlayer13;
    public GameObject mPlayer14;
    public GameObject mPlayer15;
    public GameObject mListPlayers;
    public GameObject mNumQuizTxt;
    public bool mChangeListPlayers = false;
    public static List<PlayerInfo> mPlayerList = new List<PlayerInfo>();
    public bool mCurrentUserAdmin = false;

    public bool mStart = false;
    public static int mCurrentQuestionNoRefresh = -1;
    /*public int mGameMode = 0;
    public int mWinMode = 0;
    public int mWinValue = 99999;
    public string mAdminUId;
    public string mNameGame;*/
    private bool mprova = false;
    public float mTimeLeft = 9;

    public bool mStartTime = false;


    private void Awake()
    {
        mChangeListPlayers = false;
        List<PlayerInfo> mPlayerList = new List<PlayerInfo>();
        mCurrentUserAdmin = false;
        mStart = false;
        mCurrentQuestionNoRefresh = -1;
        mTimeLeft = 9;
        mInfoTextGame.SetActive(true);
        

        /*if (!CreateScript.mPin.Equals(""))
        {
            mPinGame = CreateScript.mPin;
            mCurrentUserAdmin = true;
        }
        else if (!JoinGameScript.mPin.Equals(""))
        {
            mPinGame = JoinGameScript.mPin;
        }*/
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        mPlayer1.SetActive(false);
        mPlayer2.SetActive(false);
        mPlayer3.SetActive(false);
        mPlayer4.SetActive(false);
        mPlayer5.SetActive(false);
        mPlayer6.SetActive(false);
        mPlayer7.SetActive(false);
        mPlayer8.SetActive(false);
        mPlayer9.SetActive(false);
        mPlayer10.SetActive(false);
        mPlayer11.SetActive(false);
        mPlayer12.SetActive(false);
        mPlayer13.SetActive(false);
        mPlayer14.SetActive(false);
        mPlayer15.SetActive(false);

        //RetrieveGameValue();
        RetrievePlayersList();
        
        Time.timeScale = 1;
        mprova = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (mCurrentQuestionNoRefresh == -1 && GlobalVariables.mCurrentQuizs != -1) {
            mCurrentQuestionNoRefresh = GlobalVariables.mCurrentQuizs;
            mNumQuizTxt.GetComponent<UnityEngine.UI.Text>().text = mCurrentQuestionNoRefresh + "/" + GlobalVariables.mGameWinValue;
        }
        if (mChangeListPlayers)
        {
            Debug.Log("mChangeListPlayers");
            RefreshList();
            SaveCurrentGame();
            if (AllUsersPlay())
            {
                if (!Winner().Equals(""))
                {
                    mInfoTextGame.GetComponent<UnityEngine.UI.Text>().text = Winner() + " ES EL GANADOR!";
                    reference.Child("users").Child(auth.CurrentUser.UserId).Child("currentgames").Child(GlobalVariables.mPinGame).Child("finish").SetValueAsync(true);
                }
                else
                {
                    if (mTimeLeft < 1)
                    {
                        //mCurrentQuestion++;
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("currentgame").SetValueAsync(mCurrentQuestionNoRefresh + 1);
                        //StopAllCoroutines();
                        mprova = false;
                        mLobby.StartQuiz();
                    }
                    mSecondsTextGame.SetActive(true);
                    mSecondsTextGame.GetComponent<UnityEngine.UI.Text>().text = mTimeLeft.ToString("0");
                    //mTimeLeft -= Time.fixedDeltaTime;
                    if (!mStartTime)
                    {
                        StartCoroutine(LoseTime());
                        mStartTime = true;
                    }
                    mChangeListPlayers = true;
                }
            }
        }
    }
    private void RefreshList()
    {
            ResetListPlayers();
            int cont = 0;
            mPlayerList.Sort(new Comparison<PlayerInfo>((x, y) => y.mPoints.CompareTo(x.mPoints)));
            //Debug.Log("Add player: " + pUid + " index: " + pIndex);
            foreach (PlayerInfo item in mPlayerList)
            {
                transform.GetChild(cont).gameObject.SetActive(true);
                transform.GetChild(cont).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = item.mName;
                transform.GetChild(cont).transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = item.mPoints.ToString();
                transform.GetChild(cont).transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = item.mName.Substring(0, 1);
            cont++;
            }
            mChangeListPlayers = false;
    }

    public void RetrievePlayersList() //from the database (server)...
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("games").Child(GlobalVariables.mPinGame).Child("players")
        .ValueChanged += PlayerListHandleValueChanged;
    }

    void PlayerListHandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        mPlayerList.Clear();
        if (mprova)
        {
            foreach (DataSnapshot user in args.Snapshot.Children)
            {
                
                Debug.Log("PlayerData: " + user.Key + ":" + user.Child("points").Value);
                PlayerInfo player = new PlayerInfo();
                player.mUid = user.Key;
                if (user.Child("points").Value != null)
                    player.mPoints = int.Parse(user.Child("points").Value.ToString());
                player.mName = user.Child("name").Value.ToString();
                player.mCurrentQuestion = int.Parse(user.Child("currentquestion").Value.ToString());
                if (user.Child("bonus").Value != null)
                    player.mBonus = user.Child("bonus").Value.ToString().Equals("True");
                if (user.Child("racha").Value != null)
                    player.mRacha = int.Parse(user.Child("racha").Value.ToString());
                mPlayerList.Add(player);
                mChangeListPlayers = true;
            }
        }
    }

    /*public void RetrieveGameValue() //from the database (server)...
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("games").Child(GlobalVariables.mPinGame)
        .ValueChanged += GameValuesHandleValueChanged;
    }
    void GameValuesHandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        DataSnapshot game = args.Snapshot;
        Debug.Log("GameData: currentgame - " + game.Child("currentgame").Value + " - kids - " + game.Child("kids").Value + " - playmode - " + game.Child("playmode").Value + " - start - " + game.Child("start").Value + " - winmode - " + game.Child("winmode").Value + " - winvalue - " + game.Child("winvalue").Value);

        mStart = game.Child("start").Value.ToString().Equals("True");
        mGameMode = int.Parse(game.Child("playmode").Value.ToString());
        mWinMode = int.Parse(game.Child("winmode").Value.ToString());
        mWinValue = int.Parse(game.Child("winvalue").Value.ToString());
        mAdminUId = game.Child("admin").Value.ToString();
        mNameGame = game.Child("name").Value.ToString();
        if (mAdminUId.Equals(auth.CurrentUser.UserId))
            mCurrentUserAdmin = true;
    }*/

    public void ResetListPlayers()
    {
        mPlayer1.SetActive(false);
        mPlayer2.SetActive(false);
        mPlayer3.SetActive(false);
        mPlayer4.SetActive(false);
        mPlayer5.SetActive(false);
        mPlayer6.SetActive(false);
        mPlayer7.SetActive(false);
        mPlayer8.SetActive(false);
        mPlayer9.SetActive(false);
        mPlayer10.SetActive(false);
        mPlayer11.SetActive(false);
        mPlayer12.SetActive(false);
        mPlayer13.SetActive(false);
        mPlayer14.SetActive(false);
        mPlayer15.SetActive(false);
    }

    public bool AllUsersPlay()
    {
        bool ret = true;
        if (mPlayerList.Count > 0)
        {
            foreach (PlayerInfo item in mPlayerList)
            {
                Debug.Log("item: " + item.mName + " current: " + item.mCurrentQuestion + " mCurrentQuestion: " + mCurrentQuestionNoRefresh + " start:" + mStart);
                if (item.mCurrentQuestion < mCurrentQuestionNoRefresh)
                    ret = false;
            }
        }
        else
        {
            ret = false;
        }
        return ret;
    }

    public string Winner()
    {
        string ret = "";
        int value = 0;
        foreach (PlayerInfo item in mPlayerList)
        {
            if (GlobalVariables.mGameWinMode == 0 & mCurrentQuestionNoRefresh >= GlobalVariables.mGameWinValue & item.mPoints > value)
            {
                ret = item.mName;
                value = item.mPoints;
            }else if (GlobalVariables.mGameWinMode == 1 & item.mPoints >= GlobalVariables.mGameWinValue & item.mPoints > value)
            {
                ret = item.mName;
                value = item.mPoints;
            }
        }
        return ret;
    }

    public void SaveCurrentGame()
    {
        //reference.Child("users").Child(auth.CurrentUser.UserId).Child("currentgames").Child(GlobalVariables.mPinGame).Child("name").SetValueAsync(mNameGame);
        reference.Child("users").Child(auth.CurrentUser.UserId).Child("currentgames").Child(GlobalVariables.mPinGame).Child("currentquestion").SetValueAsync(mCurrentQuestionNoRefresh);
        reference.Child("users").Child(auth.CurrentUser.UserId).Child("currentgames").Child(GlobalVariables.mPinGame).Child("finish").SetValueAsync(false);
        reference.Child("users").Child(auth.CurrentUser.UserId).Child("currentgames").Child(GlobalVariables.mPinGame).Child("players").SetValueAsync(mPlayerList.Count);
    }

    //Countdown Thread
    IEnumerator LoseTime()
    {
        while (mTimeLeft>0.9)
        {
            yield return new WaitForSeconds(1);
            mTimeLeft--;
        }
    }

}