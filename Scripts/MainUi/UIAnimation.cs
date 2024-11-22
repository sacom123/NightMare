using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// DoTween 모음집
/// 게임 제작하면서 자주 쓰는 DoTween 들 모아서 수치만 입력하면 작동하게 모아두는 스크립트
/// DontDestroyOnLoad에 올려서 씬이 바뀌어도 삭제되지 않도록 관리
/// 게임 메뉴 뿐 아니라 게임 내에서도 사용하기 때문에 DontDestroy에 올려둔다
/// 
/// </summary>

public class UIAnimation : MonoBehaviour
{

    private void Start()
    {
        DOTween.Init();
    }
    public void Moving1(GameObject UI)
    {
        DOTween.Sequence().Append(Move1(UI)).SetLoops(-1,LoopType.Yoyo);
    }

    public void HeartBeat(GameObject UI,float scale,float origin)
    {
        DOTween.Sequence().Append(HeartBeatSequnce(UI, scale, origin));
    }

    /// <summary>
    /// FadeIn에 사용 Image ver 
    /// </summary>
    /// <param name="UI"></param>
    /// <param name="scale"></param>
    public void FadeIn(Image UI,float scale)
    {
        DOTween.Sequence().Append(Fade(UI, scale));
    }

    

    public void PTakeDamage(RawImage raw,float scale)
    {
        DOTween.Sequence().Append(TakeDamage(raw,scale));
    }

    //마우스가 UI 위에 올라 갔을때 크기 키움
    public void OnPointerAnimation(GameObject UI)
    {
        DOTween.Sequence().Append(Bigger(UI));
    }
    public void ExitPointerAnimation(GameObject UI)
    {
        DOTween.Sequence().Append(Small(UI));
    }

    public void StopDoTween()
    {
        DOTween.Sequence().Pause();
    }

    public void StartDoTween()
    {
        DOTween.Sequence().Restart();
    }
    public void SGSeq(Image Ui)
    {
        DOTween.Sequence().Append(FailedRythm(Ui));
    }
    //Rotation값을 움직임
    public Sequence Move1(GameObject UI)
    {
        var seq = DOTween.Sequence();
        seq.Append(UI.transform.DORotate(new Vector3(30, 50, 20), 2f));
        seq.Append(UI.transform.DORotate(new Vector3(-30, -50, -20), 2f));
        return seq;
    }

    //노래 비트에 맞춰서 둥둥 거림 추가
    public Sequence HeartBeatSequnce(GameObject UI,float scale,float origin)
    {
        var seq = DOTween.Sequence();
        seq.Append(UI.transform.DOScale(scale, 0.1f));
        seq.Append(UI.transform.DOScale(origin, 0.1f));
        return seq;
    }
    
    public Sequence Fade(Image fadein,float scale)
    {
        var seq = DOTween.Sequence();
        seq.Append(fadein.DOFade(scale, 1f));
        return seq;
    }

    Sequence Bigger(GameObject ui)
    {
        var seq = DOTween.Sequence();
        seq.Append(ui.transform.DOScale(2f, 01f));
        return seq;
    }
    Sequence Small(GameObject UI)
    {
        var seq = DOTween.Sequence();
        seq.Append(UI.transform.DOScale(1f, 01f));
        return seq;
    }

    Sequence TakeDamage(RawImage UI,float scale)
    {
        var seq = DOTween.Sequence();
        seq.Append(UI.DOFade(scale, 0.3f));
        return seq;
    }

    public Sequence FailedRythm(Image UI)
    {
        var seq = DOTween.Sequence();
        seq.Append(UI.DOColor(new Color(1f, 1f, 1f, 0.1f), 0.15f));
        //seq.Append(UI.DO)
        seq.Append(UI.DOColor(new Color(1f, 0f, 0f, 0.5f), 0.15f));
        seq.Append(UI.DOColor(new Color(1f, 1f, 1f, 0.1f), 0.15f));
        //seq.Append(UI.DOColor(new Color(1f, 0f, 0f, 0.5f), 0.15f).From(true));//.OnComplete(() =>
        //{
        //    seq.Append(UI.DOColor(new Color(1f, 1f, 1f, 0.01f), 0.15f).From(true));
        //});
        seq.Append(UI.transform.DORotate(new Vector3(0, 0, -4), 0.1f, default)).OnComplete(() => 
            seq.Append(UI.transform.DORotate(new Vector3(0, 0, 0), 0.1f, default)).OnComplete(() => 
                seq.Append(UI.transform.DORotate(new Vector3(0, 0, 0), 0.1f, default))));

        return seq;
    }

}
