using UnityEngine;

public class Bubble : MonoBehaviour
{
    private float damage;
    private float speed;
    private float size;

    public void Initialize(float damage, float speed, float size)
    {
        this.damage = damage;
        this.speed = speed;

        // Scale bubble size
        transform.localScale = Vector3.one * size;

        // Set velocity (assuming 2D physics)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }
    private void OnEnable()
    {
        Destroy(gameObject, 3f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision logic
        Debug.Log($"Bubble hit {collision.gameObject.name} with {damage} damage.");
        Destroy(gameObject);
    }
}
