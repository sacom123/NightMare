using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionManager : MonoBehaviour
{
    public Sound Volume;
    public AudioMixer Amixer;

    private void Awake()
    {
        Volume.slider.value = PlayerPrefs.GetFloat("BGM");
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        Amixer.SetFloat("BGM", PlayerPrefs.GetFloat("BGM"));
    }

    public void SaveOption()
    {
        Debug.Log("렌더링 설정과 BGM 설정 변경");
        PlayerPrefs.SetFloat("BGM", Volume.sound);
        PlayerPrefs.Save();
    }

}
