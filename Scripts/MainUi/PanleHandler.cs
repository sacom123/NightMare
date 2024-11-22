using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PanleHandler : MonoBehaviour
{
    public Button closeButton;

    public ReBindingManager2 reBinding;
    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
        transform.localScale = Vector3.one * 0.1f;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        reBinding.gameObject.SetActive(true);
        closeButton.transform.localScale = new Vector3(2f, 2f, 2f);
        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1.1f, 0.2f));
        seq.Append(transform.DOScale(0.6f, 0.1f));

        seq.Play().OnComplete(()=>
        {
            //reBinding.gameObject.SetActive(true);
        });
    }

    public void Hide()
    {
        var seq = DOTween.Sequence();

        transform.localScale = Vector3.one * 0.2f;

        seq.Append(transform.DOScale(1.1f, 0.1f));
        seq.Append(transform.DOScale(0.2f, 0.2f));

        seq.Play().OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }


}
