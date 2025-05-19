using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameObject player;
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject exitPanel;
    public GameObject deathPanel;
    private GameManager gameManager;

    public Button loadButton;

    private void Start()
    {
        player = Player_Controller.instance.gameObject;
        gameManager = GameManager.instance;

    }

    private void FixedUpdate()
    {

    }

    public void Continue()
    {
        SoundManager.PlaySound("menuSelect");
        gameManager.paused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        Player_Controller.instance.playerHasControl = true;
    }

    public void OpenOptions()
    {
        SoundManager.PlaySound("menuHover");
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        SoundManager.PlaySound("menuHover");
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ExitGame()
    {
        SoundManager.PlaySound("menuHover");
        pausePanel.SetActive(false);
        exitPanel.SetActive(true);
    }

    public void CancelExit()
    {
        SoundManager.PlaySound("menuHover");
        exitPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ConfirmExit()
    {
        SoundManager.PlaySound("menuSelect");
        gameManager.paused = false;
        exitPanel.SetActive(false);
        deathPanel.SetActive(false);
        Destroy(Player_Controller.instance.gameObject);
        Destroy(GameManager.instance.gameObject);
        Destroy(SaveAndLoad.instance.gameObject);
        Destroy(SoundManager.instance.gameObject);
        SceneManager.LoadScene(0);
    }

    public void ToggleDeathPanel(bool state)
    {
        deathPanel.SetActive(state);
    }

    public void ContinueDeath()
    {
        SoundManager.PlaySound("menuSelect");
        gameManager.paused = false;
        deathPanel.SetActive(false);
        SaveAndLoad.instance.RespawnPlayer();
        Time.timeScale = 1;
        Player_Controller.instance.playerHasControl = true;
        GameManager.instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TestDeath()
    {
        SoundManager.PlaySound("menuSelect");
        Time.timeScale = 1;
        GameManager.instance.Fade();
        Player_Controller.instance.currentHealth = 0;
    }
}
