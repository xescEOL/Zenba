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

public class WaitingPlayersScript : MonoBehaviour
{
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    public Text mPinText;
    public Button mStartButton;
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
    public bool mChangeListPlayers = false;
    public static List<PlayerInfo> mPlayerList = new List<PlayerInfo>();
    public bool mCurrentUserAdmin = false;

    public bool mStart = false;
    public int mGameMode;
    public int mWinMode;
    public bool mkids;
    public string mAdminUId;
    public string mNameGame;
    public bool mPublicGame = false;


    private void Awake()
    {
        mStartButton.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
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

        if (GlobalVariables.mGameName.Equals("_findgame"))
        {
            mPublicGame = true;
            GetInfoGame();
        }

        mPinText.text = "PIN: " + GlobalVariables.mPinGame;
        if (GlobalVariables.mGameAdminUId.Equals(auth.CurrentUser.UserId) && !mPublicGame)
        {
            mCurrentUserAdmin = true;
            mStartButton.gameObject.SetActive(true);
        }

        RetrieveGameValue();
        RetrievePlayersList();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshList();
        if (mStart)
        {
            reference.Child("findgames").Child(GlobalVariables.mPinGame).RemoveValueAsync();
            SceneManager.LoadScene("LobbyGame");
        }
    }
    private void RefreshList()
    {
        if (mChangeListPlayers)
        {
            ResetListPlayers();
            int cont = 0;
            //mPlayerList.Sort(new Comparison<PlayerInfo>((x, y) => y.mPoints.CompareTo(x.mPoints)));
            //Debug.Log("Add player: " + pUid + " index: " + pIndex);
            foreach (PlayerInfo item in mPlayerList)
            {
                Debug.Log("Switch...UID: " + item.mUid + " points: " + item.mPoints);
                transform.GetChild(cont).gameObject.SetActive(true);
                transform.GetChild(cont).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = item.mName;
                cont++;
            }
            mChangeListPlayers = false;
        }
    }

    public void CreateClick()
    {
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("start").SetValueAsync(true);
    }

    public void Exit()
    {
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).RemoveValueAsync();
        SceneManager.LoadScene("Menu");
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
        foreach (DataSnapshot user in args.Snapshot.Children)
        {
            Debug.Log("PlayerData: " + user.Key + ":" + user.Child("points").Value);
            PlayerInfo player = new PlayerInfo();
            player.mUid = user.Key;
            player.mName = user.Child("name").Value.ToString();
            mPlayerList.Add(player);
            mChangeListPlayers = true;
        }
    }

    public void RetrieveGameValue() //from the database (server)...
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
        Debug.Log("GameData: " + " - kids - " + game.Child("kids").Value + " - playmode - " + game.Child("playmode").Value + " - start - " + game.Child("start").Value + " - winmode - " + game.Child("winmode").Value + " - winvalue - " + game.Child("winvalue").Value);

        mStart = game.Child("start").Value.ToString().Equals("True");
        mGameMode = int.Parse(game.Child("playmode").Value.ToString());
        mWinMode = int.Parse(game.Child("winmode").Value.ToString());
        mkids = game.Child("kids").Value.ToString().Equals("True");
        mAdminUId = game.Child("admin").Value.ToString();
        mNameGame = game.Child("name").Value.ToString();
        if (mAdminUId.Equals(auth.CurrentUser.UserId))
            mCurrentUserAdmin = true;
    }

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

    public void GetInfoGame()
    {
            FirebaseDatabase.DefaultInstance.GetReference("games").Child(GlobalVariables.mPinGame).GetValueAsync().ContinueWith(task =>
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Child("start").Value.ToString().Equals("False"))
                {
                    GlobalVariables.mGameMode = int.Parse(snapshot.Child("playmode").Value.ToString());
                    GlobalVariables.mGameWinMode = int.Parse(snapshot.Child("winmode").Value.ToString());
                    GlobalVariables.mGameWinValue = int.Parse(snapshot.Child("winvalue").Value.ToString());
                    GlobalVariables.mGameAdminUId = snapshot.Child("admin").Value.ToString();
                    GlobalVariables.mGameName = snapshot.Child("name").Value.ToString();
                    GlobalVariables.mCurrentPoints = 0;
                    GlobalVariables.mListQuizs.Clear();
                    for (int i = 1; i <= GlobalVariables.mNumQuizs20; i++)
                    {
                        GlobalVariables.mListQuizs.Add(snapshot.Child("list").Child(i.ToString()).Value.ToString());
                    }
                }
            });
    }
}
