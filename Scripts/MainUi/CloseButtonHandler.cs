using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloseButtonHandler : MonoBehaviour
{
    public PanleHandler popupWindow;
    public ButtonAction MainWindow;
    public void CloseButtonClick()
    {
        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(0.95f, 0.1f));
        seq.Append(transform.DOScale(1f, 0.1f));
        seq.Append(transform.DOScale(0.6f, 0.1f));

        seq.Play().OnComplete(() => {
            popupWindow.Hide();
            MainWindow.gameObject.SetActive(true);
        });
    }
}
