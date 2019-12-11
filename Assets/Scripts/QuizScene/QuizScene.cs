using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class QuizScene : MonoBehaviour
{
    [SerializeField] List<GameObject> mListAnswersButtons = new List<GameObject>();
    [SerializeField] GameObject mButtonAcceptAnswer;
    [SerializeField] GameObject mButtonBonus50;
    [SerializeField] GameObject mButtonBonus15;
    [SerializeField] GameObject mButtonBonusRnd;
    [SerializeField] GameObject mResult;
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    public double mFirstElement;
    public double mAnswerUser;
    public bool mExit = false;
    public float mTimeLeft = 10;

    private void Awake()
    {
        mFirstElement = GenerateOptions(LobbyScript.mCorrectAnswer);
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        mResult.SetActive(false);
        mAnswerUser = -12345678;
        mButtonAcceptAnswer.SetActive(false);
        if(GetCurrentRacha() > 1){ //2
            mButtonBonus50.GetComponent<UnityEngine.UI.Button>().interactable = true;
            mButtonBonus15.GetComponent<UnityEngine.UI.Button>().interactable = true;
            mButtonBonusRnd.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnswerUser != -12345678)
        {
            mButtonAcceptAnswer.SetActive(true);
        }
        if (mExit)
        {
            Debug.Log("EXIT " + mTimeLeft);
            mTimeLeft = mTimeLeft - Time.fixedDeltaTime;
            if (mTimeLeft < 0)
            {
                Debug.Log("lOBBY");
                //reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(GetCurrentQuestion() + 1);
                //reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(GetPoints() + GetPointsAnswer());
                SceneManager.LoadScene("LobbyGame");
            }
            
        }
        
    }
    public double GenerateOptions(double correctAnswer)
    {
        double ret = -12345678;
        while (ret == -12345678 | LobbyScript.mMaxOption < (ret + (LobbyScript.mVariant * 20)) | LobbyScript.mMinOption > ret)
        {
            int randomPosition = Random.Range(1, 20);
            ret = correctAnswer - (LobbyScript.mVariant * randomPosition);
        }
        return ret;
    }

    public string GetQuizTxt()
    {
        return LobbyScript.mQuizTxt;
    }
   

    public void SetAnswerUser(double pAnswerUser)
    {
        mAnswerUser = pAnswerUser;
    }

    public void ConfirmAnswer()
    {
        mResult.SetActive(true);
        if (mAnswerUser == LobbyScript.mCorrectAnswer)
        {
            Debug.Log("CORRECT!!!!!");
            mResult.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = "HAS ACERTADO!";
            if (GetCurrentRacha() < 3)
                reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(GetCurrentRacha() + 1);
            else
            {
                reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("bonus").SetValueAsync(true);
                reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(0);
            }
        }
        else
        {
            Debug.Log("WRONG ANSWER!!!!!");
            mResult.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = "La respuesta correcta es:";
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(0);
        }
        mResult.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = LobbyScript.mCorrectAnswer.ToString();
        mResult.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>().text = "+" + GetPointsAnswer() + "p";
        Debug.Log("POINTS: " + GetPointsAnswer());
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(GetCurrentQuestion() + 1);
        GlobalVariables.mCurrentQuizs++;
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(GetPoints() + GetPointsAnswer());
        

        //reference.Child("games").Child(PlayerListScript.mPinGame).Child("currentgame").SetValueAsync(PlayerListScript.mCurrentQuestion + 1);
        mExit = true;
    }

    public int GetPointsAnswer()
    {
        double ret =(mAnswerUser - LobbyScript.mCorrectAnswer) / LobbyScript.mVariant;
        switch (ret){
            case 0:
                return 20;
            case 1:
            case -1:
                return 15;
            case 2:
            case -2:
                return 13;
            case 3:
            case -3:
                return 12;
            case 4:
            case -4:
                return 11;
            case 5:
            case -5:
                return 10;
            case 6:
            case -6:
                return 9;
            case 7:
            case -7:
                return 8;
            case 8:
            case -8:
                return 7;
            case 9:
            case -9:
                return 6;
            case 10:
            case -10:
                return 5;
            case 11:
            case -11:
                return 4;
            case 12:
            case -12:
                return 3;
            case 13:
            case -13:
                return 2;
            case 14:
            case -14:
                return 1;
            default:
                return 0;

        }
    }

    public int GetPoints()
    {
        foreach (PlayerInfo item in PlayerListScript.mPlayerList)
        {
            if (item.mUid.Equals(auth.CurrentUser.UserId))
                return item.mPoints;
        }
        return 0;
    }

    public int GetCurrentQuestion()
    {
        foreach (PlayerInfo item in PlayerListScript.mPlayerList)
        {
            if (item.mUid.Equals(auth.CurrentUser.UserId))
                return item.mCurrentQuestion;
        }
        return 0;
    }

    public int GetCurrentRacha()
    {
        foreach (PlayerInfo item in PlayerListScript.mPlayerList)
        {
            if (item.mUid.Equals(auth.CurrentUser.UserId))
                return item.mRacha;
        }
        return 0;
    }

    public void BonusRndLaunch()
    {
        int cont = 0;
        List<int> rndList = new List<int>();
        while (cont < 10)
        {
            int rnd = new System.Random().Next(1, 20);
            if (!rndList.Contains(rnd))
            {
                rndList.Add(rnd);
                if (!mListAnswersButtons[rnd].transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text.Equals(LobbyScript.mCorrectAnswer.ToString()))
                {
                    mListAnswersButtons[rnd].SetActive(false);
                    cont++;
                }
            }
        }
    }
}
