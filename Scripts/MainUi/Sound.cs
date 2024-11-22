using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
    public float sound;


    public void AudioControl()
    {
        sound = slider.value;
        if (sound == -40f) mixer.SetFloat("BGM", -80);
        else mixer.SetFloat("BGM", sound);
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}
