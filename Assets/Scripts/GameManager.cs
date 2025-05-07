using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class GameManager : MonoBehaviour
{

    public static GameManager instance; // Singleton instance of GameManager
    public GameObject player;

    // Game state variables
    public bool playerAlive = true;
    public bool playerInput = true;
    public bool paused = false;

    // menus
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject exitMenu;
    public GameObject deathMenu;

    CanvasGroup deathCanvas; // Reference to the CanvasGroup component for fading effects
    public CanvasGroup pauseCanvas;
    public bool pauseActive = false;

    InputAction menuAction;

    void Awake()
    {
        // Check if an instance of GameManager already exists
        if (instance == null)
        {
            instance = this; // Assign this instance to the static instance
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
            player = GameObject.FindGameObjectWithTag("Player"); // Find the player object by its tag
        }
        else
        {
            Destroy(gameObject); // Destroy this object if another instance already exists
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathCanvas = deathMenu.GetComponent<CanvasGroup>(); // Get the CanvasGroup component from the death menu
        deathCanvas.alpha = 0; // Set the initial alpha value to 0 (invisible)
        pauseCanvas.alpha = 0; // Set the initial alpha value to 0 (invisible)
        menuAction = InputSystem.actions.FindAction("Menu"); // Find the menu action from the Input System
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput || !playerAlive)
        {
            if (menuAction.IsPressed() && !pauseActive)
            {
                if (!paused)
                {
                    pauseActive = true;
                    pauseMenu.SetActive(true); // Activate the pause menu
                    StartCoroutine(FadeMenuIn()); // Start fading in the pause menu
                }
                else
                {
                    pauseActive = true;
                    Time.timeScale = 1;
                    StartCoroutine(FadeMenuOut());
                }
            }
        }
    }

    IEnumerator FadeMenuIn()
    {
        SoundManager.PlaySound("menuHover");
        paused = true;

        if (deathMenu.activeSelf)
            deathMenu.SetActive(false);

        while (pauseCanvas.alpha < 1)
        {
            pauseCanvas.alpha += Time.deltaTime;
            yield return null;
        }
        pauseCanvas.interactable = true;
        yield return null;

        pauseActive = false;
        Time.timeScale = 0;
    }

    IEnumerator FadeMenuOut()
    {
        while (pauseCanvas.alpha > 0)
        {
            pauseCanvas.alpha -= Time.deltaTime;
            yield return null;
        }
        pauseCanvas.interactable = false;
        yield return null;

        //SoundManager.PlaySound("menuSelect");
        paused = false;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        exitMenu.SetActive(false);

        if (player != null && !playerAlive)
            deathMenu.SetActive(true);

        pauseActive = false;
    }

    public void LoadNextScene(int index)
    {
        SceneManager.UnloadSceneAsync(index);
        SceneManager.LoadSceneAsync(index + 1);
    }

    public void Fade()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        exitMenu.SetActive(false);
        deathMenu.SetActive(true);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        while (deathCanvas.alpha < 1)
        {
            deathCanvas.alpha += Time.deltaTime;
            yield return null;
        }
        deathCanvas.interactable = true;
        yield return null;
    }

}
