using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hazards : MonoBehaviour
{
    public float damage;
    public float knockbackForce;
    public float cooldownTime;
    private TilemapCollider2D _collider;


    private void Start()
    {
        _collider = GetComponent<TilemapCollider2D>(); 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Player hit a hazard");
            Player_Controller.instance.Knock(knockbackForce);
            StartCoroutine(Cooldown(cooldownTime));
            // Damage?
        }
    }
    private IEnumerator Cooldown(float time)
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(time);
        _collider.enabled = true;
    }
}
