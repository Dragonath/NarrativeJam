using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public int dialogueIndex;
    public bool playerInTrigger = false;
    public bool dialogueStarted = false;

    public int nextSceneIndex;

    void Update()
    {
        if (playerInTrigger && GameManager.instance.inDialogue)
        {
            dialogueStarted = true;
        }
        else if (playerInTrigger && !GameManager.instance.inDialogue && dialogueStarted)
        {
            GameManager.instance.LoadScene(nextSceneIndex);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            GameManager.instance.BeginStory(dialogueIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
