using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

/// <summary>
/// Button관련 action들을 관리하는 스크립트
/// </summary>

public class ButtonAction : MonoBehaviour
{
    public PanleHandler popupWindow;
    public Image GameStartFade;
    public GameObject mainButton;
    public GameObject PlayButton;
    public GameObject OptionButton;
    public GameObject ExitButton;

    UIAnimation UIManager;
    MainMeneTimingManager MMTM;
    ManagerControl MC;
    AudioSource AS;

    private void Start()
    {
        UIManager = GameObject.Find("UIAnimationManager").GetComponent<UIAnimation>();
        MMTM = FindObjectOfType<MainMeneTimingManager>();
        MC = FindObjectOfType<ManagerControl>();
        AS = FindObjectOfType<AudioSource>();
    }

    public void GameStart()
    {
        MMTM.isRun = false;
        var seq = DOTween.Sequence();
        mainButton.transform.localScale = Vector3.one * 0.2f;
        seq.Append(mainButton.transform.DOScale(1.1f, 0.1f));
        seq.Append(mainButton.transform.DOScale(0.2f, 0.2f));
        seq.Play().OnComplete(() =>
        {
            MMTM.enabled = false;
            mainButton.SetActive(false);
        });

        var seq2 = DOTween.Sequence();
        seq2.Append(UIManager.Fade(GameStartFade,1f));
        seq2.Play().OnComplete(() =>
        {
            //GetComponent<MainMeneTimingManager>().enabled = false;
            MC.OnGameManager();
            AS.Pause();
            DOTween.KillAll();
            SceneManager.LoadScene(1);
        });
        
    }
    public void option()
    {
        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(0.95f, 0.1f));
        seq.Append(transform.DOScale(1.05f, 0.1f));
        seq.Append(transform.DOScale(-0.6f, 0.6f));

        seq.Play().OnComplete(() => {
            popupWindow.Show();
            gameObject.SetActive(false);
        });
    }

    public void EndGame()
    {
        var seq = DOTween.Sequence();
        mainButton.transform.localScale = Vector3.one * 0.2f;
        seq.Append(mainButton.transform.DOScale(1.1f, 0.1f));
        seq.Append(mainButton.transform.DOScale(0.2f, 0.2f));
        seq.Play().OnComplete(() =>
        {
            mainButton.SetActive(false);
        });
        var seq2 = DOTween.Sequence();
        seq2.Append(UIManager.Fade(GameStartFade,1f));
        seq2.Play().OnComplete(() =>
        {
            Application.Quit();
        });
        
    }
}
