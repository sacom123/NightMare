using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class VideoOption : MonoBehaviour
{
    FullScreenMode screenMode;
    public Dropdown VideoDrop;
    public Dropdown QualityDrop;

    public Toggle fullScreenBtn;
    List<Resolution> resolutions;

    int resolutionNum;
    int curQualityValue;
    int setResolution;
    private void Start()
    {
        var Qulity = PlayerPrefs.GetInt("Quality");

        if (!PlayerPrefs.HasKey("Quality"))
        {
            QualitySettings.SetQualityLevel(2);
        }
        else
        {
            QualitySettings.SetQualityLevel(Qulity);
        }

        Screen.SetResolution(1920, 1080, true,239);


        InitUI();
    }

     void InitUI()
    {
        resolutions = new List<Resolution>();

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            //||Screen.resolutions[i].refreshRate == 240
            if (Screen.resolutions[i].refreshRate == 60 || Screen.resolutions[i].refreshRate == 240)
            {
                resolutions.Add(Screen.resolutions[i]);
            }
        }

        if(VideoDrop.options != null)
        {
            VideoDrop.options.Clear();
        }
        int opnum = 0;

        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + " x " + item.height + " " + item.refreshRate + "hz";
            VideoDrop.options.Add(option);
            if (item.width == Screen.width && item.height == Screen.height)
                resolutionNum = opnum;
            opnum++;
        }

        if (!PlayerPrefs.HasKey("Resolution"))
        {
            int curopnum = 0;
            Resolution resolution = new Resolution();
            resolution.width = 1920;
            resolution.height = 1080;
            Screen.SetResolution(resolution.width, resolution.height, true);
            foreach (Resolution item in resolutions)
            {
                if (item.width == resolution.width && item.height == resolution.height)
                    setResolution = curopnum;
                curopnum++;
            }
        }
        else
        {
            setResolution = PlayerPrefs.GetInt("Resolution");
            Screen.SetResolution(resolutions[setResolution].width, resolutions[setResolution].height, true, resolutions[resolutionNum].refreshRate);
            VideoDrop.value = setResolution;
        }

        VideoDrop.RefreshShownValue();
        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow)?true:false;

        QualityDrop.options.Clear();
        QualityDrop.AddOptions(new List<string>(QualitySettings.names));
        QualityDrop.value = QualitySettings.GetQualityLevel();

     }


    public void ChangeQualityLevel(int newQualityLevel)
    {
        QualitySettings.SetQualityLevel(newQualityLevel);
        curQualityValue = QualitySettings.GetQualityLevel();
    }

    public void DropBoxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode, resolutions[resolutionNum].refreshRate);
        
        PlayerPrefs.SetInt("Resolution", resolutionNum);

        var Quality = curQualityValue;
        PlayerPrefs.SetInt("Quality", Quality);
        PlayerPrefs.Save();
    }
}
