using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;

class HealthBehaviour : MonoBehaviour
{
    public float startingHealth;

    float currentHealth;

    // Use this for initialization
    void Start()
    {
        currentHealth = startingHealth;
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}