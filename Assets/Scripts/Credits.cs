using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public GameObject creditsPanel1;
    public GameObject creditsPanel2;

    public void Continue()
    {
        creditsPanel1.SetActive(false);
        creditsPanel2.SetActive(true);
    }

    public void ConfirmExit()
    {
        if(Player_Controller.instance != null)
        {
            Destroy(Player_Controller.instance.gameObject);
        }
        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
        }
        if (SaveAndLoad.instance != null)
        {
            Destroy(SaveAndLoad.instance.gameObject);
        }
        if (SoundManager.instance != null)
        {
            Destroy(SoundManager.instance.gameObject);
        }
        SceneManager.LoadScene(0);
    }
}
