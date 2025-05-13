using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{

    public GameObject menuPanel;
    public GameObject levelPanel;
    public GameObject optionsPanel;
    public GameObject exitPanel;
    public GameObject creditsPanel;

    public Button continueButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayGame()
    {
        SoundManager.PlaySound("menuSelect");
        menuPanel.gameObject.SetActive(false);
        levelPanel.gameObject.SetActive(true);
    }
    public void CancelLoad()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(true);
        levelPanel.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        //Delete the save file and set default values
        File.Delete(Application.persistentDataPath + "/playerInfo.dat");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadSceneAsync(1);
        Destroy(gameObject);
    }
    public void LoadGame()
    {
        SaveAndLoad.instance.Load();
    }
    public void OpenOptions()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(true);
    }
    public void OpenCredits()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(false);
        creditsPanel.gameObject.SetActive(true);
    }
    public void CloseOptions()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(false);
    }
    public void CloseCredits()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(true);
        creditsPanel.gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(false);
        exitPanel.gameObject.SetActive(true);
    }
    public void CancelExit()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(true);
        exitPanel.gameObject.SetActive(false);
    }
    public void ConfirmExit()
    {
        SoundManager.PlaySound("menuSelect");
        Application.Quit();
    }
}
