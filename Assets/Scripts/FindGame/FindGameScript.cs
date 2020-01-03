using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FindGameScript : MonoBehaviour
{
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu");
    }

    public void FindGame()
    {
        Debug.Log("Buscando partida");
        RetrieveInfo();
    }

    public void RetrieveInfo() //from the database (server)...
    {
        //Retrieve the data and convert it to string...
        FirebaseDatabase.DefaultInstance.GetReference("findgames").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot levelSnapshot = task.Result;
                if (task.Result.ChildrenCount != 0)
                {
                    foreach (DataSnapshot childSnapshot in levelSnapshot.Children)
                    {
                        GlobalVariables.mPinGame = childSnapshot.Key;
                        reference.Child("findgames").Child(GlobalVariables.mPinGame).Child("players").SetValueAsync(int.Parse(childSnapshot.Child("players").Value.ToString()) + 1);
                        if (int.Parse(childSnapshot.Child("players").Value.ToString()) > 10)
                        {
                            reference.Child("findgames").Child(GlobalVariables.mPinGame).RemoveValueAsync();
                        }
                        GlobalVariables.mGameName = "_findgame";
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(0);
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("name").SetValueAsync(GlobalVariables.mUserName);
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(0);
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(0);
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("bonus").SetValueAsync(false);
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("emoji").SetValueAsync(0);
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("boot").SetValueAsync(false);
                        SceneManager.LoadScene("WaitingPlayers");
                    }
                }
                else
                {
                    GlobalVariables.mPinGame = Random.Range(00000001, 99999999).ToString("D8");
                    reference.Child("findgames").Child(GlobalVariables.mPinGame).Child("players").SetValueAsync(1);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("admin").SetValueAsync(auth.CurrentUser.UserId);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("kids").SetValueAsync(false);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("playmode").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("winmode").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("winvalue").SetValueAsync(10);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("start").SetValueAsync(false);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("name").SetValueAsync(GlobalVariables.mUserName);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("bonus").SetValueAsync(false);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("emoji").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("boot").SetValueAsync(false);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("currentgame").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("name").SetValueAsync("_findgame");
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("bonus").SetValueAsync(0);
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("findgame").SetValueAsync(true);
                    GlobalVariables.mGameAdminUId = auth.CurrentUser.UserId;
                    GlobalVariables.mGameMode = 0;
                    GlobalVariables.mGameName = "_findgame";
                    GlobalVariables.mGameWinMode = 0;
                    GlobalVariables.mGameWinValue = 10;
                    CreateListQuizs();
                    SceneManager.LoadScene("WaitingPlayers");
                }
            }
        });
    }

    public void CreateListQuizs()
    {
        int numlist = 1;
        GlobalVariables.mListQuizs.Clear();
        while (numlist <= 10)
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
