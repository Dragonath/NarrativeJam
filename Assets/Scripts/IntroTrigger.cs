using UnityEngine;

public class IntroTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.LoadScene(2);
    }
}
