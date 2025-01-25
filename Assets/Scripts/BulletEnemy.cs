using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    private void OnEnable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = -transform.right * speed;
        Destroy(gameObject, 3f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
        //if (other.gameObject.CompareTag("Bubble"))
        //{
        //    Destroy(other);
        //}
    }
}
