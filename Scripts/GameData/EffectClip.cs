using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����Ʈ ������� ���, Ÿ�Ե��� �Ӽ� �����͸� ������ ����
/// ������ ���� �ε� ��� - Ǯ���� ���� ���
/// ����Ʈ �ν��Ͻ� ��� - Ǯ���� �����Ͽ� ���
/// </summary>

public class EffectClip
{   //���� �Ӽ��� ������ �ٸ� ����Ʈ Ŭ���� ���� �� �־ �к���
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

    //���ϴ� ��ġ�� ����
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
