using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public static GameManager instance; // Singleton instance of GameManager
    public GameObject player;
    public int playerDefaultHp = 10; // Default health of the player

    // Game state variables
    public bool playerAlive = true;
    public bool playerInput = true;
    public bool paused = false;

    // menus
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject exitMenu;
    public GameObject deathMenu;
    public GameObject dialogueMenu;

    CanvasGroup deathCanvas; // Reference to the CanvasGroup component for fading effects
    public CanvasGroup pauseCanvas;
    public bool pauseActive = false;
    public CanvasGroup sceneTransition;
    public bool inDialogue = false;
    public bool inStory = false; // Flag to check if the player is in a story dialogue

    InputAction menuAction;
    InputAction interactAction;

    private Dialogue dialogue; // Reference to the Dialogue component for managing story dialogues
    public StoryDialogue storyDialogue; // Reference to the StoryDialogue component for managing story dialogues
    private int currentStoryIndex = 0; // Index of the current story being played
    private bool inTrigger = false; // Flag to check if the player is in a trigger area

    readonly float inputCD = 0.5f;
    bool inputReady = true;

    public bool noEarlyUnlocks = false;
    public bool loadStarted = false;

    public bool continueGame = false; // Flag to check if the player wants to continue the game

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
        sceneTransition.alpha = 1; // Set the initial alpha value to 1 (invisible)

        // ACTIONS
        interactAction = InputSystem.actions.FindAction("Interact"); 
        menuAction = InputSystem.actions.FindAction("Menu");

        dialogue = dialogueMenu.GetComponent<Dialogue>(); // Get the Dialogue component from this GameObject
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(SceneFadeOut());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(GameManager.instance != null)
        {
            StartCoroutine(WaitForFade());
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event to avoid memory leaks
        }
    }

    // Update is called once per frame
    void Update()
    {
        //  Check if the player is alive and if the game is paused
        if (playerInput || !playerAlive)
        {
            // MENU
            if (menuAction.IsPressed() && !pauseActive && inputReady)
            {
                inputReady = false; // Disable input until cooldown is over
                StartCoroutine(InputCooldown()); // Start the input cooldown coroutine
                if (!paused)
                {
                    Player_Controller.instance.playerHasControl = false; // Disable player control
                    pauseActive = true;
                    pauseMenu.SetActive(true); // Activate the pause menu
                    StartCoroutine(FadeMenuIn()); // Start fading in the pause menu
                }
                else
                {
                    pauseActive = true;
                    Time.timeScale = 1;
                    StartCoroutine(FadeMenuOut());
                    Player_Controller.instance.playerHasControl = true; // Enable player control
                }
            }

            // INTERACTIONS
            if (interactAction.IsPressed() && inputReady)
            {
                inputReady = false;
                StartCoroutine(InputCooldown());
                // Dialogue
                if (inTrigger && !inDialogue)
                {
                    Player_Controller.instance.playerHasControl = false;
                    inTrigger = false; // Reset the trigger flag
                    BeginStory(currentStoryIndex); // Start the story with the current index
                }
                else if (inDialogue && !dialogue.choiceGiven && !inTrigger)
                {
                    if (!dialogue.isTyping)
                    {
                        dialogue.PlayStory();
                    }
                    else if (dialogue.isTyping)
                    {
                        dialogue.skip = true;
                    }
                }
                else if (storyDialogue != null)
                {
                    if (inStory && !storyDialogue.choiceGiven)
                    {
                        if (!storyDialogue.isTyping)
                        {
                            storyDialogue.PlayStory();
                        }
                        else if (storyDialogue.isTyping)
                        {
                            storyDialogue.skip = true;
                        }
                    }
                }
                else
                {
                    // INTERACT
                }
            }
        }
    }

    IEnumerator InputCooldown()
    {
        yield return new WaitForSeconds(inputCD);
        inputReady = true;
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
        StartCoroutine(SceneFadeIn());
        SceneManager.UnloadSceneAsync(index);
        SceneManager.LoadSceneAsync(index + 1);
    }
    public void LoadScene(int index)
    {
        while (sceneTransition.alpha < 1)
        {
            sceneTransition.alpha += Time.deltaTime * 2;
        }
        SceneManager.LoadSceneAsync(index);
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
        Debug.Log("death Fading in");
        while (deathCanvas.alpha < 1)
        {
            deathCanvas.alpha += Time.deltaTime;
            yield return null;
        }
        deathCanvas.interactable = true;
        yield return null;
    }

    IEnumerator SceneFadeIn()
    {
        while (sceneTransition.alpha < 1)
        {
            sceneTransition.alpha += Time.deltaTime * 2;
            yield return null;
        }
    }

    IEnumerator SceneFadeOut()
    {
        while(sceneTransition.alpha > 0)
        {
            sceneTransition.alpha -= Time.deltaTime * 2;
            yield return null;
        }
    }

    public void BeginStory(int dialogueIndex)
    {
        Player_Controller.instance.playerHasControl = false; // Disable player input
        inDialogue = true;
        if (!loadStarted) 
        {
            loadStarted = true; // Prevent multiple calls to BeginStory
            StartCoroutine(WaitForLoad(dialogueIndex));
        }
    }

    public void EndStory()
    {
        Debug.Log("Story ended");
        Player_Controller.instance.playerHasControl = true; // Enable player input
        dialogueMenu.SetActive(false);
        inDialogue = false;
    }

    public void EndDiagStory()
    {
        Player_Controller.instance.playerHasControl = false;
        storyDialogue.gameObject.SetActive(false);
        inDialogue = false;
        inStory = false; // Reset the inStory flag
        IntroLogic.Instance.StartIntroAnimation();
    }

    public void OpenStory(int index)
    {
        inTrigger = true;
        currentStoryIndex = index;
    }

    public void CloseStory()
    {
        inTrigger = false;
        currentStoryIndex = 0;
    }

    public void DoDamage(int damage)
    {
        Debug.Log(damage + " Damage taken");
        Player_Controller.instance.currentHealth -= damage;
    }

    IEnumerator WaitForLoad(int index)
    {
        yield return new WaitForSeconds(0.5f);
        loadStarted = false; // Reset the loadStarted flag
        dialogueMenu.SetActive(true);
        dialogue.StartStory(index);
    }
    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(SceneFadeOut());
    }

    public void PlayerDeath()
    {
        Time.timeScale = 1;
        Fade();
        pauseCanvas.alpha = 1;
    }

}
