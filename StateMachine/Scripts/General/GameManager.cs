using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IObserver
{
    public static GameManager Instance;
    [SerializeField]
    GameObject panelControl;
    [SerializeField]
    GameObject panelGameOver;
    [SerializeField]
    Button btnPlayAgain;
    private void Awake()
    {
        
        Instance = this;
        panelGameOver.SetActive(false);
        panelControl.SetActive(true);
    }
    void Start()
    {
        ObserverManager.Instance.AddObserver(this);
        if (btnPlayAgain != null)
        {
            btnPlayAgain.onClick.AddListener(delegate
            {
                LoadLevelAgain();
            });
        }
       
    }

    private void ActiveGameOverPanel()
    {
        panelGameOver.SetActive(true);
    }
    private void LoadLevelAgain()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public void OnPlayerDeath() 
    {
        ActiveGameOverPanel();
        panelControl.SetActive(false);
    }

}
