using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

    public class BindingPairUI : MonoBehaviour
    {
        public bool selected = false;
        public Text actionLabel;
        public Text codeLabel;
        public Button InputButton;
        public Image inputfieldImage;
        public int index=0;
        public string bindingPairname;
        char slash = '/';

        public void Select()
        {
            selected = true;
            inputfieldImage.color = Color.green;
        }
        public void Deselect()
        {
            selected = false;
            inputfieldImage.color = Color.white;
        }

        public void InitLabled(string actionText, string CodeText, int moveindex = 0)
        {
            int delimiterIndex = CodeText.IndexOf(slash);
            index = moveindex;
            if (delimiterIndex >= 0)
            {
                string result = CodeText.Substring(delimiterIndex + 1);
            //Debug.Log(result);
                codeLabel.text = result;
            }
        else
        {
            codeLabel.text = CodeText;
        }
            actionLabel.text = actionText;
            bindingPairname = actionText;
        }

        
    public void SetCodeLadel(string text)
    {    
        int delimiterIndex = text.IndexOf(slash);    
        if (delimiterIndex >= 0)
        {
            string result = text.Substring(delimiterIndex + 1);
            codeLabel.text = result;
        }
    }

        public void AddButtonListner(UnityAction method)
        {
            InputButton.onClick.AddListener(method);
        }

    }
