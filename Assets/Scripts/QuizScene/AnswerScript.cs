using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    public QuizScene mQuizScene;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Text>().text = mQuizScene.GetQuizTxt();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
