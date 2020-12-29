using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class QuizScene : MonoBehaviour
{
    public List<GameObject> mListAnswersButtons = new List<GameObject>();
    public GameObject mButtonAcceptAnswer;
    public GameObject mButtonBonus50;
    public GameObject mButtonBonus15;
    public GameObject mButtonBonusRnd;
    public GameObject mResult;
    public GameObject mNumQuiz;
    public GameObject mTotalQuizs;
    public GameObject mPosition;
    public GameObject mTopicTxt;
    public GameObject mTopicImg;
    public Text mAnserTxt;
    public GameObject mAnswerBG;
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    public double mFirstElement;
    public double mAnswerUser;
    public bool mExit = false;
    public float mTimeLeft = 5;
    public float mTimeQuizLeft = 20;
    public bool mStartTime = false;
    private int mCurrentRacha = 0;
    public GameObject mSecondsTextGame;

    private void Awake()
    {
        mFirstElement = GenerateOptions(LobbyScript.mCorrectAnswer);
        Time.timeScale = 1;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        mResult.SetActive(false);
        mSecondsTextGame.GetComponent<UnityEngine.UI.Text>().text = mTimeQuizLeft.ToString("0");
        mNumQuiz.GetComponent<UnityEngine.UI.Text>().text = (GlobalVariables.mCurrentQuizs + 1).ToString("0")+"/";
        mTotalQuizs.GetComponent<UnityEngine.UI.Text>().text = GlobalVariables.mGameWinValue.ToString("0");
        mPosition.GetComponent<UnityEngine.UI.Text>().text = GetPosition().ToString("0") + "º";
        mTopicTxt.GetComponent<UnityEngine.UI.Text>().text = GetTopicTxt();
        mTopicImg.GetComponent<UnityEngine.UI.Image>().sprite = GetTopicImg();
        Time.timeScale = 1;
        StartCoroutine(QuizTimeDown());
        mAnswerUser = -12345678;
        mButtonAcceptAnswer.SetActive(false);
        mCurrentRacha = GetCurrentRacha();
        if (mCurrentRacha > 1){ //2
            mButtonBonus50.GetComponent<UnityEngine.UI.Button>().interactable = true;
            //mButtonBonus15.GetComponent<UnityEngine.UI.Button>().interactable = true;
            //mButtonBonusRnd.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnswerUser != -12345678 && !mExit)
        {
            mButtonAcceptAnswer.SetActive(true);
        }
        if (mExit)
        {
            Debug.Log("EXIT " + mTimeLeft);
            if (!mStartTime)
            {
                StartCoroutine(LoseTime());
                mStartTime = true;
            }
        }
        if (mTimeQuizLeft < 0 && !mExit)
        {
            mExit = true;
            ConfirmAnswer();
        }

    }
    public double GenerateOptions(double correctAnswer)
    {
        double ret = -12345678;
        while (ret == -12345678 | LobbyScript.mMaxOption < (ret + (LobbyScript.mVariant * 20)) | LobbyScript.mMinOption > ret)
        {
            int randomPosition = Random.Range(1, 21);
            if(Random.Range(0, 3) == 1 && randomPosition < 10)
            {
                randomPosition = 20 - randomPosition;
            }
            ret = correctAnswer - (LobbyScript.mVariant * randomPosition);
        }
        return ret;
    }

    public string GetQuizTxt()
    {
        return LobbyScript.mQuizTxt;
    }

        public string GetTopicTxt()
    {

        if (LobbyScript.mTopicQuiz.Equals("1"))
                return "Ciencia y Naturaleza";
        if (LobbyScript.mTopicQuiz.Equals("2"))
                return "Historia";
        if (LobbyScript.mTopicQuiz.Equals("3"))
                return "Deportes";
        if (LobbyScript.mTopicQuiz.Equals("4"))
                return "Actualidad";
        if (LobbyScript.mTopicQuiz.Equals("5"))
                return "Arte y Literatura";

        return "Actualidad";
    }

            public Sprite GetTopicImg()
    {

        if (LobbyScript.mTopicQuiz.Equals("1"))
                return Resources.Load<Sprite>("NaturaTopic");
        if (LobbyScript.mTopicQuiz.Equals("2"))
                return Resources.Load<Sprite>("HistoriaTopic");
        if (LobbyScript.mTopicQuiz.Equals("3"))
                return Resources.Load<Sprite>("DeportesTopic");
        if (LobbyScript.mTopicQuiz.Equals("4"))
                return Resources.Load<Sprite>("ActualidadTopic");
        if (LobbyScript.mTopicQuiz.Equals("5"))
                return Resources.Load<Sprite>("ArteTopic");

        return Resources.Load<Sprite>("ActualidadTopic");
    }
   

    public void SetAnswerUser(double pAnswerUser)
    {
        mAnswerUser = pAnswerUser;
        mAnserTxt.text = pAnswerUser.ToString();
    }

    public void ConfirmAnswer()
    {
        mButtonAcceptAnswer.SetActive(false);
        //mButtonAcceptAnswer.GetComponent<UnityEngine.UI.Button>().interactable = true;
        mResult.SetActive(true);
        if (mAnswerUser == LobbyScript.mCorrectAnswer)
        {
            Debug.Log("CORRECT!!!!!");
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("correctanswer").SetValueAsync(true);
            mResult.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = "HAS ACERTADO!";
            if (mCurrentRacha < 3)
                mCurrentRacha++;
            else
            {
                reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("bonus").SetValueAsync(true);
                mCurrentRacha = 0;
            }
            mAnswerBG.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("correctstar_background");
        }
        else
        {
            Debug.Log("WRONG ANSWER!!!!!");
            reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("correctanswer").SetValueAsync(false);
            mResult.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = "La respuesta correcta es:";
            
        }
        mResult.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = LobbyScript.mCorrectAnswer.ToString();
        mResult.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>().text = GetPointsAnswer() + "p";
        Debug.Log("POINTS: " + GetPointsAnswer());
        GlobalVariables.mCurrentQuizs++;
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(GlobalVariables.mCurrentQuizs);
        GlobalVariables.mCurrentPoints = GetPointsAnswer() + GlobalVariables.mCurrentPoints;
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(GlobalVariables.mCurrentPoints);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(mCurrentRacha);


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

    public int GetPosition()
    {
        int pos = 0;
        foreach (PlayerInfo item in PlayerListScript.mPlayerList){
            pos++;
            if (item.mUid.Equals(auth.CurrentUser.UserId))
                return pos;
        }
        return 1;
    }

    public void BonusRndLaunch()
    {
        mButtonBonus50.GetComponent<UnityEngine.UI.Button>().interactable = false;
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
        mCurrentRacha = 0;
    }

    //Countdown Thread
    IEnumerator LoseTime()
    {
        yield return new WaitForSeconds(4);
        Debug.Log("lOBBY");
        SceneManager.LoadScene("LobbyGame");
    }

    IEnumerator QuizTimeDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!mExit)
            {
                mTimeQuizLeft--;
                if (mTimeQuizLeft >= 0)
                    mSecondsTextGame.GetComponent<UnityEngine.UI.Text>().text = mTimeQuizLeft.ToString("0");
            }
        }
    }

}
