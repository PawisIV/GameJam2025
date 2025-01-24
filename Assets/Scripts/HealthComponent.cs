using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float MaxHealth = 50f;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = MaxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}