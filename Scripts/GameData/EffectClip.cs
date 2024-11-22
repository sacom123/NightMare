using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 이펙트 프리펩과 경로, 타입등의 속성 데이터를 가지고 있음
/// 프리팹 사전 로딩 기능 - 풀링을 위한 기능
/// 이펙트 인스턴스 기능 - 풀링과 연계하여 사용
/// </summary>

public class EffectClip
{   //추후 속성은 같지만 다른 이펙트 클립이 있을 수 있어서 분별용
    public int realID = 0;

    public EffectType effectType = EffectType.NORMAL;
    public GameObject effectPrefab = null;
    public string effectName = string.Empty;
    public string effectPath = string.Empty;
    public string effectFullPath = string.Empty;
    
    public EffectClip() { }

    public void PreLoad()
    {
        this.effectFullPath = effectPath + effectName;
        if(this.effectFullPath != string.Empty && this.effectPrefab ==null)
        {
            this.effectPrefab = ResourceManager.Load(effectFullPath) as GameObject;
        }
    }

    public void ReleaseEffect()
    {
        if(this.effectPrefab != null)
        {
            this.effectPrefab = null;
        }
    }

    //원하는 위치에 생성
    public GameObject Instantiate(Vector3 Pos)
    {
        if (this.effectPrefab == null)
        {
            this.PreLoad();
        }
        if (this.effectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(effectPrefab, Pos, Quaternion.identity);
            return effect;
        }
        return null;
    }

}
