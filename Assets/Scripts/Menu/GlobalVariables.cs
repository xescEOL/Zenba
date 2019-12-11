using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class GlobalVariables : MonoBehaviour
{
    private static Firebase.Auth.FirebaseAuth auth;

    public static double mUserPoints = -999999;
    public static double mUserGames = -999999;
    public static string mUserName = "";
    public static string mPinGame = "";
    public static int mNumQuizs20 = 0;
    public static List<int> mListQuizs = new List<int>();
    public static int mCurrentQuizs = 0;

    private void Awake()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        RetrieveInfo(auth.CurrentUser.UserId);
        RetrieveNumQuizs("quests20ESP");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RetrieveInfo(string pUserUID) //from the database (server)...
    {
        //Retrieve the data and convert it to string...
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result.Child(pUserUID);
            mUserPoints = double.Parse(snapshot.Child("points").Value.ToString());
            mUserGames = double.Parse(snapshot.Child("games").Value.ToString());
            mUserName = snapshot.Child("name").Value.ToString();
            Debug.Log("Retrive UserInfo: " + mUserPoints + ", " + mUserGames + ", " + mUserName);
        });
    }

    public void RetrieveNumQuizs(string pReference) //from the database (server)...
    {
        //Retrieve the data and convert it to string...
        FirebaseDatabase.DefaultInstance.GetReference(pReference).GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result;
            mNumQuizs20 = int.Parse(snapshot.ChildrenCount.ToString());
        });
    }

}
