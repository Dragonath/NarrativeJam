using UnityEngine;
using UnityEngine.SceneManagement;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput || !playerAlive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!paused)
                {
                    //SoundManager.PlaySound("MENUHOVER");
                    paused = true;

                    if (deathMenu.activeSelf)
                        deathMenu.SetActive(false);

                    pauseMenu.SetActive(true);
                    Time.timeScale = 0;
                }
                else
                {
                    //SoundManager.PlaySound("MENUSELECT");
                    paused = false;
                    pauseMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    exitMenu.SetActive(false);

                    if (player != null && !playerAlive)
                        deathMenu.SetActive(true);

                    Time.timeScale = 1;
                }
            }
        }
    }

    public void LoadNextScene(int index)
    {
        SceneManager.UnloadSceneAsync(index);
        SceneManager.LoadSceneAsync(index + 1);
    }

}
