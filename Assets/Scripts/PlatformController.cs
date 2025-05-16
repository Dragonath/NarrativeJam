using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public int ground, ignore;

    private void Start()
    {
        ground = LayerMask.NameToLayer("Ground");
        ignore = LayerMask.NameToLayer("Default");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.contacts[0].normal);
        if (collision.contacts[0].normal == Vector2.up)
        {
            gameObject.layer = ignore;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        gameObject.layer = ground;
    }
}
