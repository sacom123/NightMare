using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Menu������ ui�� ���뿡 ���缭 �������� Ŀ���� ȿ���� �ֱ����� ��ũ��Ʈ
/// UiAnimationManager���� �޾ƿ´�
/// </summary>
public class MainMeneTimingManager : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d; //��Ʈ ������ ���� �ð� üũ���� ����
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
            currentTime += Time.deltaTime; //1�ʿ� 1�� ����
            if (currentTime >= 60d / bpm) //60s / BPM = 1��Ʈ�� �ð�, 60s / 120BPM = 1��Ʈ�� 0.5�� //0.5�ʵ��� �ϳ��� ��Ʈ�� ����
            {
                UIAniManager.HeartBeat(MainUI.GetComponent<ButtonAction>().PlayButton, MainUI.GetComponent<ButtonAction>().PlayButton.GetComponent<TextAni>().scale, 1f);
                UIAniManager.HeartBeat(MainUI.GetComponent<ButtonAction>().OptionButton, MainUI.GetComponent<ButtonAction>().OptionButton.GetComponent<TextAni>().scale, 1f);
                UIAniManager.HeartBeat(MainUI.GetComponent<ButtonAction>().ExitButton, MainUI.GetComponent<ButtonAction>().ExitButton.GetComponent<TextAni>().scale, 1f);
                currentTime -= 60d / bpm; //currentTime�� �� �����߻� ����
            }
        }
        else
        {
            MainUI = null;
        }
    }
}
