using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class LobbyScript : MonoBehaviour
{
    private static Firebase.Auth.FirebaseAuth auth;
    public System.Random rnd = new System.Random();

    public static string mQuizTxt = "";
    public static double mCorrectAnswer = -999999;
    public static double mVariant = -999999;
    public static double mMaxOption = -999999;
    public static double mMinOption = -999999;
    public bool mRetrieveDB = false;
    public long mCountQuizs = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        RetrieveData("quests20ESP");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (mQuizTxt != "" & mCorrectAnswer != -999999 & mVariant != -999999 & mMaxOption != -999999 & mMinOption != -999999 & mRetrieveDB == false)
        {
            mRetrieveDB = true;
        }
    }

    public void StartQuiz()
    {
        if (mRetrieveDB & CorrectQuestion())
            SceneManager.LoadScene("Quiz20");
        else if (mRetrieveDB & !CorrectQuestion())
            Debug.Log("Question NOT correct");
    }

    public void RetrieveData(string pReference) //from the database (server)...
    {
        //Retrieve the data and convert it to string...
        FirebaseDatabase.DefaultInstance.GetReference(pReference).GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result;
            mQuizTxt = snapshot.Child(GlobalVariables.mListQuizs[GlobalVariables.mCurrentQuizs].ToString()).Child("question").Value.ToString();
            mCorrectAnswer = double.Parse(snapshot.Child(GlobalVariables.mListQuizs[GlobalVariables.mCurrentQuizs].ToString()).Child("correct").Value.ToString(), CultureInfo.InvariantCulture);
            mVariant = double.Parse(snapshot.Child(GlobalVariables.mListQuizs[GlobalVariables.mCurrentQuizs].ToString()).Child("variant").Value.ToString(), CultureInfo.InvariantCulture);
            if (snapshot.Child(GlobalVariables.mListQuizs[GlobalVariables.mCurrentQuizs].ToString()).Child("maxValue").Value.ToString() == "")
                mMaxOption = 99999999;
            else
                mMaxOption = double.Parse(snapshot.Child(GlobalVariables.mListQuizs[GlobalVariables.mCurrentQuizs].ToString()).Child("maxValue").Value.ToString(), CultureInfo.InvariantCulture);
            if (snapshot.Child(GlobalVariables.mListQuizs[GlobalVariables.mCurrentQuizs].ToString()).Child("minValue").Value.ToString() == "")
                mMinOption = -99999999;
            else
                mMinOption = double.Parse(snapshot.Child(GlobalVariables.mListQuizs[GlobalVariables.mCurrentQuizs].ToString()).Child("minValue").Value.ToString(), CultureInfo.InvariantCulture);
            Debug.Log("Retrive Info: " + mQuizTxt + ", " + mCorrectAnswer + ", " + mVariant + ", " + mMaxOption + ", " + mMinOption + ".");
        });
    }

    public void SignOut()
    {
        auth.SignOut();
    }

    public bool CorrectQuestion()
    {
        return ((((mMaxOption - mCorrectAnswer) / mVariant) + ((mCorrectAnswer - mMinOption) / mVariant)) >= 20) & mMaxOption > mCorrectAnswer & mMinOption < mCorrectAnswer;
    }

}
