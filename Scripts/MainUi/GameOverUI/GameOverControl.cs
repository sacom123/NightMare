using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameOverControl : MonoBehaviour
{
    public Image Fade;
    float RSscale;
    float EXscale;

    //버튼들을 키기 위함
    public GameObject buttons;

    public Button ReStartButton;
    public Button ExitButton;

    //리듬에 맞춰서 둥둥거림 추가
    public Text ReStart;
    public Text Exit;

    //UI에 Animation을 넣기 위함
    UIAnimation UIA;

    public int bpm = 0;
    double currentTime = 0;

    void Start()
    {
        UIA = GameObject.Find("UIAnimationManager").GetComponent<UIAnimation>();
        UIA.FadeIn(Fade, 0.8f);
    }

    void Update()
    {
        currentTime += Time.deltaTime; //1초에 1씩 증가
        if (currentTime >= 60d / bpm) //60s / BPM = 1비트당 시간, 60s / 120BPM = 1비트당 0.5초 //0.5초동안 하나의 비트를 출현
        {
            RSscale = ReStart.GetComponent<TextAni>().scale;
            EXscale = Exit.GetComponent<TextAni>().scale;

            UIA.HeartBeat(ReStartButton.gameObject, RSscale, 1f);
            UIA.HeartBeat(ExitButton.gameObject, EXscale, 1f);
            currentTime -= 60d / bpm; //currentTime의 값 오차발생 방지
        }
        OnButton();
    }

    void OnButton()
    {
        if(Fade.color.a <= 0.81)
        {
            buttons.SetActive(true);
        }
    }
}
