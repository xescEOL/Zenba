using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PatataScript : MonoBehaviour
{
    private static DatabaseReference reference;
    private static Firebase.Auth.FirebaseAuth auth;
    public Button mBotonConf;
    public Button mBotonReset;
    public GameObject mAnswerTxt;
    public GameObject mPistaTxt;
    public GameObject mQuizTxt;
    public GameObject mNumQuiz;
    public GameObject mTotalQuizs;
    public GameObject mPosition;
    public GameObject mTopicTxt;
    public GameObject mTopicImg;
    AudioSource audioData;
    public GameObject mTimeBar;

    public int mAnswerUser = -12345678;

    private bool mFinish = false;
    private bool mConfirm = false;

    private bool mCorrectAnswer = false;

    Animator mAnim;

    private void Awake()
    {
        Time.timeScale = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zenba-3a261.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        mQuizTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = LobbyScript.mQuizTxt;
        mAnim = mTimeBar.transform.GetChild(0).GetComponent<Animator>();
        mNumQuiz.GetComponent<UnityEngine.UI.Text>().text = (GlobalVariables.mCurrentQuizs + 1).ToString("0")+"/";
        mTotalQuizs.GetComponent<UnityEngine.UI.Text>().text = GlobalVariables.mGameWinValue.ToString("0");
        mPosition.GetComponent<UnityEngine.UI.Text>().text = GetPosition().ToString("0") + "º";
        mTopicTxt.GetComponent<UnityEngine.UI.Text>().text = GetTopicTxt();
        mTopicImg.GetComponent<UnityEngine.UI.Image>().sprite = GetTopicImg();
        audioData = GetComponent<AudioSource>();
        mAnim.SetBool("Start", true);
        StartCoroutine(LoseTime());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InsertNumber(int pNum)
    {
        if(mAnswerTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text.Length < 4 && !mFinish && !mConfirm)
            mAnswerTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = mAnswerTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text + pNum.ToString();
        else if(mConfirm)
            mAnswerTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = pNum.ToString();
        mConfirm = false;

    }
    public void CleanAnswer()
    {
        if(!mFinish)
            mAnswerTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = "";
    }
    public void ConfirmAnswer()
    {
        if (!mFinish)
        {
            audioData.Play(0);
            mAnswerUser = int.Parse(mAnswerTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text);
            mConfirm = true;
            if (mAnswerUser == LobbyScript.mCorrectAnswer)
            {
                mBotonConf.interactable = false;
                mBotonReset.interactable = false;
                mPistaTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = "CORRECTO";
                mCorrectAnswer = true;
                SaveQuizResult();
            }
            else if (mAnswerUser < LobbyScript.mCorrectAnswer)
            {
                mPistaTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = "MÁS";
            }
            else if (mAnswerUser > LobbyScript.mCorrectAnswer)
            {
                mPistaTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = "MENOS";
            }
        }
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

    public void SaveQuizResult()
    {
        mAnim.SetBool("Start", false);
        StopAllCoroutines();
        GlobalVariables.mCurrentQuizs++;
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("currentquestion").SetValueAsync(GlobalVariables.mCurrentQuizs);
        GlobalVariables.mCurrentPoints = GlobalVariables.mCurrentPoints + GetPoints(mAnswerUser);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("points").SetValueAsync(GlobalVariables.mCurrentPoints);
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("correctanswer").SetValueAsync(mCorrectAnswer);
        StartCoroutine(Finish());
        //reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("racha").SetValueAsync(mCurrentRacha);
    }
    public int GetPoints(int pAnswer)
    {
        double ret = (pAnswer - LobbyScript.mCorrectAnswer);
        switch (ret)
        {
            case 0:
                return 20;
            case 1:
            case -1:
                return 15;
            case 2:
            case -2:
            case 3:
            case -3:
            case 4:
            case -4:
                return 13;
            case 5:
            case -5:
            case 6:
            case -6:
            case 7:
            case -7:
                return 10;
            case 8:
            case -8:
            case 9:
            case -9:
            case 10:
            case -10:
            case 11:
            case -11:
                return 8;
            case 12:
            case -12:
            case 13:
            case -13:
            case 14:
            case -14:
            case 15:
            case -15:
                return 6;
            case 16:
            case -16:
            case 17:
            case -17:
            case 18:
            case -18:
            case 19:
            case -19:
                return 4;
            case 20:
            case -20:
            case 21:
            case -21:
            case 22:
            case -22:
            case 23:
            case -23:
                return 2;
            case 24:
            case -24:
            case 25:
            case -25:
            case 26:
            case -26:
            case 27:
            case -27:
                return 1;
            default:
                return 0;
        }
    }
    IEnumerator LoseTime()
    {
        yield return new WaitForSeconds(20);
        mFinish = true;
        mBotonConf.interactable = false;
        mBotonReset.interactable = false;
        if (mAnswerUser != -12345678)
            mAnswerTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = mAnswerUser.ToString();
        mPistaTxt.gameObject.GetComponent<UnityEngine.UI.Text>().text = "+" + GetPoints(mAnswerUser) + "p. Respuesta: " + LobbyScript.mCorrectAnswer;
        SaveQuizResult();
        Debug.Log("finish");
    }

    IEnumerator Finish()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("LobbyGame");
    }

}
