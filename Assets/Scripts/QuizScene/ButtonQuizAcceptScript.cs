using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonQuizAcceptScript : MonoBehaviour
{
    public QuizScene mQuizScene;
    [SerializeField] GameObject mButtonFather;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
            GetComponent<UnityEngine.UI.Text>().text = "Confirmar " + mQuizScene.mAnswerUser;
    }
}
