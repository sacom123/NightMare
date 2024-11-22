using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Menu씬에서 ui에 리듬에 맞춰서 스케일이 커지는 효과를 주기위한 스크립트
/// UiAnimationManager에서 받아온다
/// </summary>
public class MainMeneTimingManager : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d; //노트 생성을 위한 시간 체크해줄 변수
    public float scale;

    public GameObject MainUI;

    public bool isRun = true;

    UIAnimation UIAniManager;


    void Start()
    {

        UIAniManager = GameObject.Find("UIAnimationManager").GetComponent<UIAnimation>();

        MainUI = GameObject.Find("MainUi");

    }

    void Update()
    {
        if(isRun)
        {
            currentTime += Time.deltaTime; //1초에 1씩 증가
            if (currentTime >= 60d / bpm) //60s / BPM = 1비트당 시간, 60s / 120BPM = 1비트당 0.5초 //0.5초동안 하나의 비트를 출현
            {
                UIAniManager.HeartBeat(MainUI.GetComponent<ButtonAction>().PlayButton, MainUI.GetComponent<ButtonAction>().PlayButton.GetComponent<TextAni>().scale, 1f);
                UIAniManager.HeartBeat(MainUI.GetComponent<ButtonAction>().OptionButton, MainUI.GetComponent<ButtonAction>().OptionButton.GetComponent<TextAni>().scale, 1f);
                UIAniManager.HeartBeat(MainUI.GetComponent<ButtonAction>().ExitButton, MainUI.GetComponent<ButtonAction>().ExitButton.GetComponent<TextAni>().scale, 1f);
                currentTime -= 60d / bpm; //currentTime의 값 오차발생 방지
            }
        }
        else
        {
            MainUI = null;
        }
    }
}
