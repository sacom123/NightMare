using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using System.Linq;


public class ReBindingManager : MonoBehaviour    
{
        
    public InputActionAsset PlayerInputAsset;
    
    public InputActionAsset defaultInput;
    
    private InputActionMap IAM;
    
    PlayerInput PI;
    
    private List<GameObject> BindingObject;
    private List<InputActionReference> InputactionList;
    [SerializeField]private InputActionReference Move = null;
    [SerializeField] private InputActionReference Jump = null;
    [SerializeField] private InputActionReference dash = null;
    [SerializeField] private InputActionReference pick = null;
    [SerializeField] private InputActionReference attack = null;
    [SerializeField] private InputActionReference fire = null;
    [SerializeField] private InputActionReference reload = null;

    

    public GameObject textTemplate;    
    public Text CodeText;    
    public Transform textContainer;    
    public GameObject inputImage;
    bool _isRebinding = false;
    public string _curKeyName;
    public string _curKeyCode;
    public GameObject _curnewText;
    bool isWaitingForKeyPress;
    private InputBinding newinput;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    
    private void Init()
    {
        PI = GetComponent<PlayerInput>();
        IAM = PI.actions.FindActionMap("Player");
        BindingObject = new List<GameObject>();
        InputactionList = new List<InputActionReference>();
        InputactionList.Add(Move);
        InputactionList.Add(Jump);
        InputactionList.Add(dash);
        InputactionList.Add(pick);
        InputactionList.Add(attack);
        InputactionList.Add(fire);
        InputactionList.Add(reload);
    }

        public void Start()
        {
            Init();
            MakeRebindList();
        }

      
    public void Update()    
    {
        if (_isRebinding)
        {
            inputImage.SetActive(true);
            isWaitingForKeyPress = true;
            if (ListenInput(out string keyCode))
            {
                Bind(_curKeyName, keyCode);
                RefrashRebindingUI(_curnewText, keyCode);
                _isRebinding = false;
                inputImage.SetActive(false);
            }
        }
    }

    public void MakeRebindList()
    {
        if (BindingObject != null)
        {
            foreach (var go in BindingObject)
            {
                Destroy(go);
            }
            BindingObject.Clear();
        }

        foreach (InputAction item in IAM)
        {
            if (item.name == "Move")
            {
                for (int i = 1; i < item.bindings.Count; i++)
                {
                    var newText = Instantiate(textTemplate, textContainer);
                    BindingObject.Add(newText);
                    var pairUI = newText.GetComponent<BindingPairUI>();
                    pairUI.InitLabled(item.bindings[i].name, item.bindings[i].path, i);

                    pairUI.AddButtonListner(() =>
                    {
                        pairUI.Select();
                        _isRebinding = true;
                        _curKeyName = pairUI.bindingPairname;
                        CodeText = pairUI.codeLabel;
                        _curnewText = newText;

                    });
                }
            }
            else if (item.name == "Look")
            {
                break;
            }
            else
            {
                var newText = Instantiate(textTemplate, textContainer);
                BindingObject.Add(newText);
                var pairUI = newText.GetComponent<BindingPairUI>();
                pairUI.InitLabled(item.name, item.bindings[0].path);
                pairUI.AddButtonListner(() =>
                {
                    pairUI.Select();
                    _isRebinding = true;
                    _curKeyName = item.name;
                    CodeText = pairUI.codeLabel;
                    _curnewText = newText;
                });
            }
        }
        textContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(950f, 930f);
    }
    private bool ListenInput(out string code)
        {
            if (isWaitingForKeyPress)
            {
                // 아무 키나 눌렀을 때 검사
                Keyboard keyboard = Keyboard.current;
                Mouse mouse = Mouse.current;

                if (keyboard.anyKey.isPressed)
                {
                    string keyName = keyboard.allKeys.FirstOrDefault(k => k.isPressed).name;
                    code = "<Keyboard>/" + keyName;
                _curKeyCode = code;
                newinput = new InputBinding(_curKeyCode);
                    isWaitingForKeyPress = false;
                    return true;
                }
                else if (mouse.leftButton.isPressed || mouse.rightButton.isPressed || mouse.middleButton.isPressed)
                {
                    if (mouse.leftButton.isPressed)
                    {
                        code = "<Mouse>/" + "leftButton";
                    _curKeyCode = code;
                    newinput = new InputBinding(_curKeyCode);
                    isWaitingForKeyPress = false;
                        return true;
                    }
                    else if (mouse.rightButton.isPressed)
                    {
                        code = "<Mouse>/" + "rightButton";
                    _curKeyCode = code;
                    newinput = new InputBinding(_curKeyCode);
                    isWaitingForKeyPress = false;
                        return true;
                    }
                    
                else if (mouse.middleButton.isPressed)    
                {
                    code = "<Mouse>/" + "middleButton";
                    _curKeyCode = code;
                    newinput = new InputBinding(_curKeyCode);
                    isWaitingForKeyPress = false;
                 return true; 
                }   
            }    
        }
        code = null;
        return false;
    }

      
    public void Bind(string name, string keyCode)
    {
        foreach (InputAction item in IAM)
        {
            Debug.Log(item.name);
            if (item.name == "Move")
            {
                for (int i = 0; i < item.bindings.Count; i++)
                {
                    if (item.bindings[i].name == name)
                    {
                            
                    }
                }
            }
            else if (item.name == name)
            {
                PI.SwitchCurrentActionMap("Menu");
                foreach (var list in InputactionList)
                {
                    string listname = "Player/" + name;
                    if (list.name == listname)
                    {
                        Debug.Log(_curKeyCode);
                        list.action.ApplyBindingOverride(newinput);
                        Debug.Log(list.action.bindings);
                        Debug.Log(list.action.bindings[0].path);
                        
                    }
                }
            }
        }
    }

    private void RebindComplete()
    {
        Debug.Log("RebindComplete 불러와짐 ");
        Debug.Log(PI.actions["pick"].bindings[0].path);
        inputImage.SetActive(false);
        RefrashRebindingUI(_curnewText, _curKeyCode);
        rebindingOperation.Dispose();
        PI.SwitchCurrentActionMap("Player");

    }

   
    private void RefrashRebindingUI(GameObject text,string Keycode)
    {
        
        text.GetComponent<BindingPairUI>().SetCodeLadel(Keycode);   
        text.GetComponent<BindingPairUI>().Deselect();   
    }
   
    public void AllReSetButton()   
    {   
        IAM = defaultInput.FindActionMap("Player");
        MakeRebindList();
    }

    
    public void BindingSaveButton()   
    {
        Debug.Log("Is Save");
        var rebins = PI.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("ReBinds",rebins);
        PlayerPrefs.Save();
    }
    public void OnEnable()
    {
        
        Debug.Log("OnEnable 활성화");
        var rebins = PlayerPrefs.GetString("ReBinds");
        Debug.Log(!string.IsNullOrEmpty(rebins));
        if (string.IsNullOrEmpty(rebins)==false) 
        {
            Debug.Log("Input에 ReBinds 넣는중");
            PlayerInputAsset.LoadBindingOverridesFromJson(rebins);
        }
    }
}

