using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class ManagerControl : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject EffectCanvas;
    public GameObject EffectManager;
    public GameObject DashEffect;

    Scene scene;
    
    void Awake()
    {
        scene = SceneManager.GetActiveScene();
        if(scene.buildIndex == 0 )
        {
            DontDestroyOnLoad(this);
            GameManager.SetActive(false);
            EffectCanvas.SetActive(false);
            EffectManager.SetActive(false);
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(2);
        }
    }
    public void OnGameManager()
    {
        //GameManager.SetActive(true);
        EffectCanvas.SetActive(true);
        EffectManager.SetActive(true);
    }
    public void OnChain()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DashEffect.SetActive(true);
        GameManager.GetComponent<GameManager>().IsReady = false;
        GameManager.GetComponent<GameManager>().SetUiObject();
        Invoke("SetUi", 1f); 
    }
    public void SetUi()
    {
        GameManager.GetComponent<GameManager>().SetUI();
    }
    void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
