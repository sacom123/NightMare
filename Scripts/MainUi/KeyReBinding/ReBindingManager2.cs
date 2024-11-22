using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using System.Linq;
public class ReBindingManager2 : MonoBehaviour
{
    [SerializeField] InputActionReference Pick;

    [SerializeField] public InputActionAsset PlayerInputAsset;

    [SerializeField] public InputActionAsset defaultInput;

    private List<InputBinding> inputbindingList;


    private InputActionMap IAM;
    private int count = 0;

    PlayerInput PI;

    private List<GameObject> BindingObject;

    private bool isAssigend = false;
    public GameObject textTemplate;
    public Text CodeText;
    public Transform textContainer;
    public GameObject inputImage;
    public string _curKeyCode;
    public GameObject _curnewText;
    public char slash = '/';


    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Init()
    {
        inputImage.transform.Find("InputWatingText").GetComponent<Text>().text = "Waiting For Input....";

        PI = GetComponent<PlayerInput>();

        var defaultInputaction = defaultInput.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("Reset", defaultInputaction);
        PlayerPrefs.Save();
        if (!PlayerPrefs.HasKey("ReBinds"))
        {
            var ResetRebinds = PlayerPrefs.GetString("ReSet");
            PlayerInputAsset.LoadBindingOverridesFromJson(ResetRebinds);
            PI.actions = defaultInput;
            IAM = PI.actions.FindActionMap("Player");
        }
        else
        {
            var rebins = PlayerPrefs.GetString("ReBinds");
            if (!string.IsNullOrEmpty(rebins))
            {
                PlayerInputAsset.LoadBindingOverridesFromJson(rebins);
                Debug.Log(PlayerInputAsset);
                PI.actions = PlayerInputAsset;
                IAM = PI.actions.FindActionMap("Player");
            }
            else
            {
                var playerInputAction = PlayerPrefs.GetString("Default");
                PlayerInputAsset.LoadBindingOverridesFromJson(playerInputAction);
                PI.actions = PlayerInputAsset;
                IAM = PI.actions.FindActionMap("Player");
            }
        }
        BindingObject = new List<GameObject>();
        inputbindingList = new List<InputBinding>();
    }


    public void Start()
    {
        Init();
        MakeRebindList();
    }

    public void MakeRebindList()
    {
        int listcount = 0;
        if (BindingObject != null)
        {
            foreach (var go in BindingObject)
            {
                Destroy(go);
            }
            BindingObject.Clear();
        }
        foreach(InputAction item in IAM)
        {
            if (item.name == "Move")
            {
                for (int i = 0; i < item.bindings.Count-1; i++)
                {
                    var newText = Instantiate(textTemplate, textContainer);
                    BindingObject.Add(newText);
                    var pairUI = newText.GetComponent<BindingPairUI>();
                    InputActionReference curReference = InputActionReference.Create(item);

                    inputbindingList.Add(curReference.action.bindings[i + 1]);

                    int bindingIndex = (curReference.action.GetBindingIndex(inputbindingList[i]));
                    string codeText = InputControlPath.ToHumanReadableString(inputbindingList[i].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    string labelText = MakeLabelString(inputbindingList[i].name);
                    pairUI.InitLabled(labelText, 
                        InputControlPath.ToHumanReadableString(inputbindingList[i].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice),bindingIndex);
                    pairUI.AddButtonListner(() =>
                    {
                        pairUI.Select();
                        inputImage.SetActive(true);
                        PI.SwitchCurrentActionMap("Menu");
                        RebindAction(curReference, pairUI,pairUI.index);
                    });
                    listcount++;
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
                InputActionReference curReference = InputActionReference.Create(item);
                inputbindingList.Add(curReference.action.bindings[0]);
                int bindingIndex = curReference.action.GetBindingIndex(inputbindingList[listcount]);
                string codeText = InputControlPath.ToHumanReadableString(inputbindingList[listcount].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                string labelText = MakeLabelString(curReference.name);
                pairUI.InitLabled(labelText, codeText);

                pairUI.AddButtonListner(() =>
                {
                    pairUI.Select();
                    inputImage.SetActive(true);
                    PI.SwitchCurrentActionMap("Menu");
                    RebindAction(curReference, pairUI);
                });
                listcount++;
            }

        }
        Debug.Log("Binding 생성 완료");
        textContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(950f, 1260f);
    }

    public void RebindAction(InputActionReference curReference, BindingPairUI pairUI,int index = 0)
    {
        if(rebindingOperation != null)
        {
            rebindingOperation.Dispose();
        }
        rebindingOperation = curReference.action.PerformInteractiveRebinding(index)
                        .OnMatchWaitForAnother(0.1f)
                        .OnApplyBinding((operation, path) =>
                        {
                            Debug.Log("OnApplyBinding 실행");

                            string firstpath = MakeLabelString(path);
                            string secondpath = null;
                            InputBinding newBinding = new InputBinding { overridePath = path };
                            if (firstpath == "leftButton" || firstpath == "rightButton" || firstpath == "middleButton")
                            {
                                Debug.Log("마우스 왼버튼 실행 in onapplybindings");
                                secondpath = "/Mouse/" + firstpath;
                                Debug.Log(secondpath);
                            }
                            else
                            {
                                Debug.Log("키보드버튼 실행 in onapplybindings");
                                secondpath = "/Keyboard/" + firstpath;
                                Debug.Log(secondpath);
                            }
                            if (IsBindingConflict(newBinding))
                            {
                                Debug.Log("이미 바인딩 된 키 입니다");
                            }
                            else
                            {
                                Debug.Log(curReference.action.bindings[index].id);
                                foreach (InputBinding inputBinding in inputbindingList)
                                {
                                    Debug.Log(curReference.action.bindings[index].id);
                                    if (inputBinding.id == curReference.action.bindings[index].id)
                                    {
                                        Debug.Log(newBinding);
                                        Debug.Log(curReference.action.name);
                                        curReference.action.ApplyBindingOverride(index,newBinding);
                                        inputbindingList[count] = curReference.action.bindings[index];
                                        break;
                                    }
                                    count++;
                                }
                                Debug.Log(curReference.action.bindings[index]);
                                count = 0;
                                Debug.Log("바인딩이 되었습니다");
                                _curKeyCode = firstpath;
                            }
                        })
                        .OnComplete(operation => RebindComplete(pairUI, operation, _curKeyCode,curReference,index))
                    .Start();
    }
    private void RebindComplete(BindingPairUI pairUI, InputActionRebindingExtensions.RebindingOperation operation,string keyCode,InputActionReference actionReference,int index)
    {
        if (!isAssigend && operation.completed)
        {
            RebindingDisplay(pairUI, keyCode);
            rebindingOperation.Dispose();

            //foreach (InputBinding existingBinding in inputbindingList)
            //{
            //    if (existingBinding == actionReference.action.bindings[index])
            //    {
            //        inputbindingList[inputbindingListIndex] = actionReference.action.bindings[0];
                   
            //    }
            //}

            pairUI.Deselect();
            
            inputImage.SetActive(false);
            
            Debug.Log(PI);
            
            PI.SwitchCurrentActionMap("Player");
            
            inputImage.transform.Find("InputWatingText").GetComponent<Text>().text = "Waiting For Input....";

        }
        else if(isAssigend || operation.canceled)
        {
            Debug.Log("재바인딩을 위한 설정");
            inputImage.transform.Find("InputWatingText").GetComponent<Text>().text = "Please enter a different key.....";
            rebindingOperation.Dispose();
            RebindAction(actionReference, pairUI,pairUI.index);
        }
    }

    private bool IsBindingConflict(InputBinding newBInding)
    {

        foreach (InputBinding existingBinding in inputbindingList)
        {
            if (existingBinding.overridePath != null)
            {
                if (newBInding.overridePath == existingBinding.overridePath)
                {
                    isAssigend = true;
                    return true;
                }
            }
            else
            {
                if (newBInding.overridePath == existingBinding.path)
                {
                    isAssigend = true;
                    return true;
                }
            }
        }
        isAssigend = false;
        return false;
    }
    public void RebindingDisplay(BindingPairUI pairUI, string keycode)
    {
        Debug.Log(keycode);
        pairUI.codeLabel.text = keycode;
    }

    public string MakeLabelString(string labelcode)
    {
        int delimiterIndex = labelcode.IndexOf(slash);
        if (delimiterIndex >= 0)
        {
            string result = labelcode.Substring(delimiterIndex + 1);
            return result;
        }
        else
        {
            return labelcode;
        }
    }

    public void SaveButton()
    {
        Debug.Log("Is Save");
        var rebins = PI.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("ReBinds", rebins);
        PlayerPrefs.Save();
    }

    public void ResetButton()
    {
        var Reset = PlayerPrefs.GetString("ReSet");
        PlayerInputAsset.LoadBindingOverridesFromJson(Reset);
        PI.actions = PlayerInputAsset;
        IAM = PI.actions.FindActionMap("Player");
        MakeRebindList();
    }

}
