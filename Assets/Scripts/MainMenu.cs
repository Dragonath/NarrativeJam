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

    public Button continueButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
        SceneManager.LoadSceneAsync(5);
    }
    public void CloseOptions()
    {
        SoundManager.PlaySound("menuHover");
        menuPanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(false);
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
