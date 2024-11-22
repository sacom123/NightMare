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

    //��ư���� Ű�� ����
    public GameObject buttons;

    public Button ReStartButton;
    public Button ExitButton;

    //���뿡 ���缭 �յհŸ� �߰�
    public Text ReStart;
    public Text Exit;

    //UI�� Animation�� �ֱ� ����
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
        currentTime += Time.deltaTime; //1�ʿ� 1�� ����
        if (currentTime >= 60d / bpm) //60s / BPM = 1��Ʈ�� �ð�, 60s / 120BPM = 1��Ʈ�� 0.5�� //0.5�ʵ��� �ϳ��� ��Ʈ�� ����
        {
            RSscale = ReStart.GetComponent<TextAni>().scale;
            EXscale = Exit.GetComponent<TextAni>().scale;

            UIA.HeartBeat(ReStartButton.gameObject, RSscale, 1f);
            UIA.HeartBeat(ExitButton.gameObject, EXscale, 1f);
            currentTime -= 60d / bpm; //currentTime�� �� �����߻� ����
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
