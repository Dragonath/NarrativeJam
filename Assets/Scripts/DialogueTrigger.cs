using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject triggerBox2D;
    public GameObject textBox;

    public int dialogueIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerBox2D.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Player")) && GameManager.instance.inDialogue)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textBox.SetActive(true);
            GameManager.instance.OpenStory(dialogueIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textBox.SetActive(false);
            GameManager.instance.CloseStory();
        }
    }
}
