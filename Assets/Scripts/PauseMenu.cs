using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject player;
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject exitPanel;
    public GameObject deathPanel;
    public GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void Continue()
    {
        SoundManager.PlaySound("menuSelect");
        gameManager.paused = false;
        pausePanel.SetActive(false);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && player.GetComponent<PlayerControls>().getDead())
            deathPanel.SetActive(true);

        Time.timeScale = 1;

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
        Application.Quit();
    }

    public void ToggleDeathPanel(bool state)
    {
        deathPanel.SetActive(state);
    }

}
