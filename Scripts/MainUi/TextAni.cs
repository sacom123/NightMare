using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TextAni : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private Text textComponent;
    private Color originalColor;
    private Color hoverColor = Color.white;
    public float scale = 1.5f;
    public bool OnPointer = false;


    UIAnimation UI;
    public void Start()
    {
        UI = GameObject.Find("UIAnimationManager").GetComponent<UIAnimation>();
        UI.Moving1(this.gameObject);
        textComponent = GetComponent<Text>();
        originalColor = textComponent.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        scale = 2.5f;
        textComponent.color = hoverColor;

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        scale = 1.5f;
        textComponent.color = originalColor;

    }
}
