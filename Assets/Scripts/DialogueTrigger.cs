using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject triggerBox2D;
    public GameObject textBox;
    public GameObject VFX;

    public int dialogueIndex;
    public bool playerInTrigger = false;
    public bool dialogueStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger && GameManager.instance.inDialogue)
        {
            textBox.SetActive(false);
            dialogueStarted = true;
        }
        else if (playerInTrigger && !GameManager.instance.inDialogue && dialogueStarted)
        {
            Instantiate(VFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            textBox.SetActive(true);
            GameManager.instance.OpenStory(dialogueIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            textBox.SetActive(false);
            GameManager.instance.CloseStory();
        }
    }

}
