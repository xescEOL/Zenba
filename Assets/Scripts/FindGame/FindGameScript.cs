using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FindGameScript : MonoBehaviour
{
    private static Firebase.Auth.FirebaseAuth auth;
    public GameObject[] mCurrentGames;
    public List<GameInfo> mGamesList = new List<GameInfo>();

    private void Awake()
    {
        foreach(GameObject item in mCurrentGames)
        {
            item.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        //reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        GetCurrentGames();
    }

    // Update is called once per frame
    void Update()
    {
        int cont = 0;
        foreach (GameInfo item in mGamesList)
        {
            mCurrentGames[cont].SetActive(true);
            mCurrentGames[cont].transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = item.mName;
            mCurrentGames[cont].transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = item.mPlayers.ToString();
            mCurrentGames[cont].transform.GetChild(5).GetComponent<UnityEngine.UI.Text>().text = item.mCurrentQuestion.ToString();
            cont++;
        }

    }

    public void GetCurrentGames()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(auth.CurrentUser.UserId).Child("currentgames").GetValueAsync().ContinueWith(task =>
        {
            foreach (DataSnapshot user in task.Result.Children)
            {
                GameInfo game = new GameInfo();
                game.mPin = user.Key;
                game.mName = user.Child("name").Value.ToString();
                game.mFinish = user.Child("name").Value.ToString() == "True";
                game.mCurrentQuestion = int.Parse(user.Child("currentquestion").Value.ToString());
                game.mPlayers = int.Parse(user.Child("players").Value.ToString());
                Debug.Log(game.mPin + " - " + game.mName);
                mGamesList.Add(game);
            }
        });

    }

    public void ShowGame(int pGameNumber)
    {
        GlobalVariables.mPinGame = mGamesList[pGameNumber].mPin;
        SceneManager.LoadScene("LobbyGame");
    }
}
