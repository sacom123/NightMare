using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraShakingTest : MonoBehaviour
{
    public float duration;
    public float strngth;
    public int vibrato;

    void Start()
    {
        Camera.main.DOShakePosition(duration,strngth,vibrato);
    }
}
