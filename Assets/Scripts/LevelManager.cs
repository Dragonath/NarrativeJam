using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public Camera mainCamera; 
    public List<GameObject> spawnPoints;
    public int dialogueIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TriggerLevel();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Player_Controller.instance.rb.position = spawnPoints[SaveAndLoad.instance._lvl1Flag].transform.position;
            mainCamera.transform.position = new Vector3(Player_Controller.instance.rb.position.x, Player_Controller.instance.rb.position.y, -2);
            if (SaveAndLoad.instance._lvl1Flag == 0)
            {
                GameManager.instance.BeginStory(dialogueIndex);
            }
            return;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            Player_Controller.instance.rb.position = spawnPoints[SaveAndLoad.instance._lvl2Flag].transform.position;
            mainCamera.transform.position = new Vector3(Player_Controller.instance.rb.position.x, Player_Controller.instance.rb.position.y, -2);
            if (SaveAndLoad.instance._lvl2Flag == 0)
            {
                GameManager.instance.BeginStory(dialogueIndex);
            }
            return;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            Player_Controller.instance.rb.position = spawnPoints[SaveAndLoad.instance._lvl3Flag].transform.position;
            mainCamera.transform.position = new Vector3(Player_Controller.instance.rb.position.x, Player_Controller.instance.rb.position.y, -2);

            if (SaveAndLoad.instance._lvl3Flag == 0)
            {
                GameManager.instance.BeginStory(dialogueIndex);
            }
            return;
        }
    }
}
