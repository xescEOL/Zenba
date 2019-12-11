using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class TouchBar : MonoBehaviour
{
    [SerializeField] int mBarNum;
    AudioSource audioData;
    Image myImageComponent;
    public Sprite selectedImg;
    public Sprite semiselectedImg;
    public Sprite noSelectedImg;

    public QuizScene mQuizScene;

    Animator mAnim;

    void Start()
    {
        audioData = GetComponent<AudioSource>();
        myImageComponent = GetComponent<Image>();
        Time.timeScale = 0;
        transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = (mQuizScene.mFirstElement + (LobbyScript.mVariant * mBarNum)).ToString();
        mAnim = GetComponent<Animator>();
        mAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
    public void pushFinger()
    {
        Debug.Log("Push bar: " + mBarNum.ToString());
        //mCursorTxt.GetComponent<UnityEngine.UI.Text>().text = (mQuizScene.mFirstElement + (LobbyScript.mVariant * mBarNum)).ToString();
        //mCursor.SetActive(true);
        //mCursor.transform.position = transform.position + new Vector3(-200, 0, 0);
        audioData.Play(0);
        myImageComponent.sprite = selectedImg;
        mQuizScene.SetAnswerUser(mQuizScene.mFirstElement + (LobbyScript.mVariant * mBarNum));
        mAnim.SetInteger("Anim", mBarNum);
    }

    private void Update()
    {
        if (mQuizScene.mAnswerUser != mQuizScene.mFirstElement + (LobbyScript.mVariant * mBarNum))
        {
            myImageComponent.sprite = noSelectedImg;
            mAnim.SetInteger("Anim", 0);
        }
        //transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = (mQuizScene.mFirstElementDouble + (mQuizScene.mVariantDouble * mBarNum)).ToString();
    }


}