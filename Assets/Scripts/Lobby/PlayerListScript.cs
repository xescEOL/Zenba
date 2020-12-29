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
    public GameObject mExitButton;
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
    public Text mCommentary;
    public Text mCommentary2;
    public static List<PlayerInfo> mPlayerList = new List<PlayerInfo>();
    public bool mCurrentUserAdmin = false;

    public bool mStart = false;
    public static int mCurrentQuestionNoRefresh = -1;
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
        mExitButton.SetActive(false);
        //RetrieveGameValue();
        RetrievePlayersList();
        StartCoroutine(CourtineCheckDataSave());
        StartCoroutine(PlayBootCourtine());
        Time.timeScale = 1;
        int rnd = UnityEngine.Random.Range(1, 6);
        if (rnd == 1)
            mCommentary.text = "Parece que hay jugadores que tardan mucho en contestar...";
        else if (rnd == 2)
            mCommentary.text = "¡Mejor esperamos a que contesten todos los jugadores antes de comentar!";
        else if (rnd == 3)
            mCommentary.text = "Recuerda que dispones de más tiempo para pensar las respuestas, tus rivales todavía están contestando";
        else if (rnd == 4)
            mCommentary.text = "No te pongas nervioso, los otros concursantes tardan más que tú en contestar...";
        else if (rnd == 5)
            mCommentary.text = "Esperando a los demás jugadores, espera un poquito";
        else if (rnd == 6)
            mCommentary.text = "No te hagas ilusiones, cuando contesten todos los concursantes, puede que bajes alguna posición...";
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
                if(mPlayerList.Count<12)
                    mCommentary.text = GetCommentary();
                else
                    mCommentary2.text = GetCommentary();
                if (!Winner().Equals(""))
                {
                    //mInfoTextGame.GetComponent<UnityEngine.UI.Text>().text = Winner() + " ES EL GANADOR!";
                    mExitButton.SetActive(true);
                    reference.Child("users").Child(auth.CurrentUser.UserId).Child("currentgames").Child(GlobalVariables.mPinGame).Child("finish").SetValueAsync(true);
                }
                else
                {
                    if (mTimeLeft < 1)
                    {
                        //mCurrentQuestion++;
                        reference.Child("games").Child(GlobalVariables.mPinGame).Child("currentgame").SetValueAsync(mCurrentQuestionNoRefresh + 1);
                        //StopAllCoroutines();
                        SaveLastPositions();
                        mLobby.StartQuiz();
                    }
                    mSecondsTextGame.SetActive(true);
                    mSecondsTextGame.GetComponent<UnityEngine.UI.Text>().text = mTimeLeft.ToString("0");
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
            if (item.mCurrentQuestion == mCurrentQuestionNoRefresh && item.mCorrectAnswer)
                transform.GetChild(cont).transform.GetChild(4).GetComponent<UnityEngine.UI.Image>().enabled = true;
            else
                transform.GetChild(cont).transform.GetChild(4).GetComponent<UnityEngine.UI.Image>().enabled = false;
            transform.GetChild(cont).transform.GetChild(5).GetComponent<UnityEngine.UI.Text>().text = item.mCurrentQuestion.ToString();
            if (item.mEmoji != 0)
            {
                transform.GetChild(cont).transform.GetChild(6).gameObject.SetActive(true);
                transform.GetChild(cont).transform.GetChild(6).GetComponent<UnityEngine.UI.Image>().sprite = GetSprite(item.mEmoji);
            }
            else
            {
                transform.GetChild(cont).transform.GetChild(6).gameObject.SetActive(false);
            }
            if (item.mUid.Equals(auth.CurrentUser.UserId))
                transform.GetChild(cont).GetComponent<UnityEngine.UI.Image>().color = new Color(255, 213, 0, 1f);
            else
                transform.GetChild(cont).GetComponent<UnityEngine.UI.Image>().color = new Color(255, 213, 0, 0f);
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
        if (mTimeLeft >= 1)
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
                if (user.Child("correctanswer").Value != null)
                    player.mCorrectAnswer = user.Child("correctanswer").Value.ToString().Equals("True");
                if (user.Child("emoji").Value != null)
                    player.mEmoji = int.Parse(user.Child("emoji").Value.ToString());
                if (user.Child("boot").Value != null)
                    player.mBoot = user.Child("boot").Value.ToString().Equals("True");
                mPlayerList.Add(player);
                mChangeListPlayers = true;
            }
        }
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

    public void CheckDataSave()
    {
        if (mPlayerList.Count > 0 && !mStartTime)
        {
            int maxCurrent = 0;
            foreach (PlayerInfo item in mPlayerList)
            {
                if (item.mCurrentQuestion > maxCurrent)
                    maxCurrent = item.mCurrentQuestion;
            }
            foreach (PlayerInfo item in mPlayerList)
            {
                if (item.mCurrentQuestion != maxCurrent)
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(item.mUid).Child("currentquestion").SetValueAsync(maxCurrent);
            }    
        }
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

    public Sprite GetSprite(int pSprite)
    {
        switch (pSprite)
        {
            case 1:
                return Resources.Load<Sprite>("Emojis/Pack1/emocionado");
            case 2:
                return Resources.Load<Sprite>("Emojis/Pack1/partido");
            case 3:
                return Resources.Load<Sprite>("Emojis/Pack1/angel");
            case 4:
                return Resources.Load<Sprite>("Emojis/Pack1/diablo");
            case 5:
                return Resources.Load<Sprite>("Emojis/Pack1/aversion");
            case 6:
                return Resources.Load<Sprite>("Emojis/Pack1/riendo");
            case 7:
                return Resources.Load<Sprite>("Emojis/Pack1/empollon");
            case 8:
                return Resources.Load<Sprite>("Emojis/Pack1/enojado");
            case 9:
                return Resources.Load<Sprite>("Emojis/Pack1/rock");
            case 10:
                return Resources.Load<Sprite>("Emojis/Pack1/detective");
            case 11:
                return Resources.Load<Sprite>("Emojis/Pack1/fresco");
            case 12:
                return Resources.Load<Sprite>("Emojis/Pack1/espantado");
            case 13:
                return Resources.Load<Sprite>("Emojis/Pack1/decepcion");
            case 14:
                return Resources.Load<Sprite>("Emojis/Pack1/mudo");
            case 15:
                return Resources.Load<Sprite>("Emojis/Pack1/bostezando");
            case 16:
                return Resources.Load<Sprite>("Emojis/Pack1/estupido");
            case 17:
                return Resources.Load<Sprite>("Emojis/Pack1/llanto");
            case 18:
                return Resources.Load<Sprite>("Emojis/Pack1/gato");
            default:
                return Resources.Load<Sprite>("Emojis/Pack1/angel");
        }

    }

    public void SetEmoji(int pEmojiNum)
    {
        reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(auth.CurrentUser.UserId).Child("emoji").SetValueAsync(pEmojiNum);
    }

    public void BootPlay()
    {
            int rnd2 = UnityEngine.Random.Range(0, mPlayerList.Count);
            if (mPlayerList[rnd2].mBoot && mPlayerList[rnd2].mCurrentQuestion < mCurrentQuestionNoRefresh)
            {
                int rnd = UnityEngine.Random.Range(0, 23);
                if (rnd == 21 || rnd == 19)
                    rnd = 20;
                else if (rnd == 22)
                    rnd = 0;
                else if (rnd == 18 || rnd == 17 || rnd == 16)
                    rnd = 15;
                else if (rnd == 14)
                    rnd = 13;
                reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(mPlayerList[rnd2].mUid).Child("currentquestion").SetValueAsync(mPlayerList[rnd2].mCurrentQuestion + 1);
                reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(mPlayerList[rnd2].mUid).Child("points").SetValueAsync(mPlayerList[rnd2].mPoints + rnd);
                if(rnd == 20)
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(mPlayerList[rnd2].mUid).Child("correctanswer").SetValueAsync(true);
                else
                    reference.Child("games").Child(GlobalVariables.mPinGame).Child("players").Child(mPlayerList[rnd2].mUid).Child("correctanswer").SetValueAsync(false);
                return;
            }
    }

    public string GetCommentary()
    {
        List<int> positionsChange = new List<int>();
        List<string> playersCorrect = new List<string>();
        List<string> playersNoCorrect = new List<string>();
        string winPos = "";
        string lostPos = "";
        int posCurrent = 1;
        foreach (PlayerInfo item in mPlayerList)
        {
            if (item.mCorrectAnswer)
                playersCorrect.Add(item.mName);
            else
                playersNoCorrect.Add(item.mName);
            int poslast = 1;
            foreach (string item2 in GlobalVariables.mPositionsLastPlay)
            {
                if (item2.Equals(item.mName))
                    positionsChange.Add(poslast - posCurrent);
                poslast++;
            }
            posCurrent++;
        }
        int max = 0;
        int min = 0;
        int posList = 0;
        foreach (int num in positionsChange)
        {
            if (num > max)
            {
                max = num;
                winPos = mPlayerList[posList].mName;
            }
            if (num < min)
            {
                min = num;
                lostPos = mPlayerList[posList].mName;
            }
            posList++;
        }
        if (mCurrentQuestionNoRefresh == 0 && mPlayerList.Count > 1)
            return "Bienvenidos a una partida de Zenba! Hoy tenemos un interesantisimo duelo entre " + mPlayerList.Count + " concursantes. Mucha suerte a todos los participantes. ¡EMPEZAMOS!";
        if (mCurrentQuestionNoRefresh == 0 && mPlayerList.Count == 1)
            return "Bienvenidos a una partida de Zenba! Hoy tenemos una partida muy poco interesante con tan solo un concursante. Mi bola de cristal me dice que <b>" + mPlayerList[0].mName + "</b> ganará la partida. ¡EMPEZAMOS!";
        if (mCurrentQuestionNoRefresh == 0 && mPlayerList.Count == 2)
            return "Bienvenidos a una partida de Zenba! Hoy tenemos un duelo entre <b>" + mPlayerList[0].mName + "</b> y <b>" + mPlayerList[0].mName + "</b>. ¡EMPEZAMOS CON EL 1 CONTRA 1!";
        if (!Winner().Equals(""))
            return "¡FELICIDADES A <b>" + mPlayerList[0].mName + "</b>! Eres el ganador de la partida de hoy. Aquí termina esta disputado juego, les esperamos en las siguientes ediciones de Zenba!";
        if (playersCorrect.Count == mPlayerList.Count && mPlayerList.Count > 2)
            return "¡INCREIBLE! Todos los concursantes contestaron correctamente. La clasificación no se mueve.";
        if (playersNoCorrect.Count == 1 && mPlayerList.Count > 2)
            return "Parece que <b>" + playersNoCorrect[0] + "</b> a sido el único participante que no sabía de lo que le estaban hablando...";
        if (playersNoCorrect.Count == 2 && mPlayerList.Count > 2)
            return "Parece que <b>" + playersNoCorrect[0] + "</b> y <b>" + playersNoCorrect[1] + "</b> no estaban atentos a la pregunta y les han pillado hablando de sus cosas... que sea la última vez!";
        if (playersCorrect.Count == 2 && mPlayerList.Count > 2)
            return "<b>" + playersCorrect[0] + "</b> y <b>" + playersCorrect[1] + "</b> son los únicos que encertaron la pregunta. 20 puntos más que se suman a sus clasificaciones.";
        if (mCurrentQuestionNoRefresh == 1 && playersCorrect.Count == 1 && mPlayerList.Count > 2)
            return "¡Parece que <b>" + mPlayerList[0].mName + "</b> empieza muy fuerte! Su respuesta correcta le lleva a lo más alto de la clasificación. Por suerte para los demás participantes, esto acaba de empezar.";
        if (mCurrentQuestionNoRefresh == 1 && playersCorrect.Count == 2 && mPlayerList.Count > 2)
            return "¡Parece que <b>" + mPlayerList[0].mName + "</b> y <b>" + mPlayerList[1].mName + "</b> empiezan muy fuerte! Sus respuestas correctas los llevan a lo más alto de la clasificación. Por suerte para los demás participantes, esto acaba de empezar.";
        if (mCurrentQuestionNoRefresh == 1 && mPlayerList.Count > 2)
            return "<b>" + mPlayerList[0].mName + "</b> se pone en cabeza con la primera pregunta de la partida de hoy. ¿Podrá mantener esta poca ventaja que tiene con los demás?";
        if (playersCorrect.Count == 1 && mPlayerList.Count > 2)
            return "¡<b>" + playersCorrect[0] + "</b> a sido el único concursante en responder bien la pregunta! Coge ventaja con todos los demás.";
        if (max != 0 && (max + min) > 0 && mPlayerList.Count > 2)
            return "¡<b>" + winPos + "</b> escala " + max + " posiciones con una sola pregunta! ¿Empieza su remontada?";
        if (min != 0 && (max + min) < 0 && mPlayerList.Count > 2)
            return "¡<b>" + winPos + "</b> baja " + max + " posiciones con una sola pregunta! Las cosas no le están saliendo como quería...";
        if (mCurrentQuestionNoRefresh == 1 && mPlayerList.Count > 2 && mPlayerList[0].mPoints != mPlayerList[1].mPoints)
            return "<b>" + mPlayerList[0].mName + "</b> se pone en cabeza con la primera pregunta de la partida de hoy. ¿Podrá mantener esta poca ventaja que tiene con los demás?";
        if (mCurrentQuestionNoRefresh == 1 && mPlayerList.Count > 2 && mPlayerList[0].mPoints == mPlayerList[1].mPoints && mPlayerList[1].mPoints != mPlayerList[2].mPoints)
            return "<b>" + mPlayerList[0].mName + "</b> y <b>"  + mPlayerList[1].mName + "</b> se ponen en cabeza con la primera pregunta de la partida de hoy. ¿Quién ganará este duelo tan interesante?";
        if (mCurrentQuestionNoRefresh == 1 && mPlayerList.Count == 1 && playersCorrect.Count == 0)
            return "<b>" + mPlayerList[0].mName + "</b> esperaba que me dedicaras una respuesta correcta para la primera pregunta... espero que en las siguientes puedas conseguir los 20 puntos.";
        if (mCurrentQuestionNoRefresh == 1 && mPlayerList.Count == 1 && playersCorrect.Count == 1)
            return "¡Primera pregunta y consigues los 20 puntos... no esta mal para empezar <b>" + mPlayerList[0].mName + "</b>!";
        if (playersCorrect.Count == 1 && mPlayerList.Count == 1)
            return "<b>" + mPlayerList[0].mName + "</b>, jugar solo y sin presión te sienta mejor. ¡Muy buena respuesta!";
        if (playersCorrect.Count == 0 && mPlayerList.Count == 1)
            return "Aprovecha que juegas solo para intentar consiguir los 20 puntos, <b>" + mPlayerList[0].mName + "</b>. !Suerte para la siguiente pregunta!";
        if (playersCorrect.Count == 0 && mPlayerList.Count == 2)
            return "Se puede sentir el respeto entre <b>" + mPlayerList[0].mName + "</b> y <b>" + mPlayerList[1].mName + "</b> y nadie de los dos quiere contestar bien las preguntas!";
        if (playersCorrect.Count == 2 && mPlayerList.Count == 2)
            return "Nadie de los dos quiere dar ventaja al otro concursante. ¡40 puntos que se reparten entre los dos!";
        if (playersCorrect.Count == 1 && mPlayerList.Count == 2)
            return "¡Magnifica respuesta correcta de <b>" + playersCorrect[0] + "</b> que consigue sumar más puntos que <b>" + playersNoCorrect[0] + "</b>!";
        if (mPlayerList.Count > 1 && mPlayerList[mPlayerList.Count - 1].mPoints == 0)
            return "Parece que <b>" + mPlayerList[mPlayerList.Count - 1].mName + "</b> todavía no ha encendido el teléfono...";
        if (mPlayerList.Count > 2 && mPlayerList[0].mPoints == mPlayerList[1].mPoints && mPlayerList[1].mPoints != mPlayerList[2].mPoints)
            return "<b>" + mPlayerList[0].mName + "</b> y <b>" + mPlayerList[1].mName + "</b> se ponen en cabeza con un empate de puntos. ¿Quién ganará este duelo tan interesante?";
        if (mPlayerList.Count > 2 && mPlayerList[0].mPoints == mPlayerList[1].mPoints && mPlayerList[1].mPoints == mPlayerList[2].mPoints)
            return "¡Increible! Tenemos un triple empate entre <b>" + mPlayerList[0].mName + "</b>, <b>" + mPlayerList[1].mName + "</b> y <b>" + mPlayerList[2].mName + "</b> para conquistar la cima de la victoria. ¿Quén será el campeón de los tres?";
        if (mPlayerList.Count > 1 && mPlayerList[0].mPoints != mPlayerList[1].mPoints)
            return "Parece que a <b>" + mPlayerList[0].mName + "</b> le está gustando estar arriba de todo... ";
        if (mPlayerList.Count > 1 && mPlayerList[0].mPoints == mPlayerList[1].mPoints)
            return "<b>" + mPlayerList[0].mName + "</b> y <b>" + mPlayerList[1].mName + "</b> están empatados a puntos en lo más alto de la cima!";
        return "";
    }

    public void SaveLastPositions()
    {
        GlobalVariables.mPositionsLastPlay.Clear();
        foreach (PlayerInfo item in mPlayerList)
        {
            GlobalVariables.mPositionsLastPlay.Add(item.mName);
        }
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

    IEnumerator CourtineCheckDataSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(35);
            CheckDataSave();
            Debug.Log("Check");
        }
    }

    IEnumerator PlayBootCourtine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            BootPlay();
        }
    }

}