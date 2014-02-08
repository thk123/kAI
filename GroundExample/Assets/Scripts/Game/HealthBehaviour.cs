using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;
using kAI.Core;

[RequireComponent(typeof(AIBehaviour))]
class HealthBehaviour : MonoBehaviour
{
    public float startingHealth;

    float currentHealth;

    kAIDataPort<float> healthPort;

    // Use this for initialization
    void Start()
    {
        currentHealth = startingHealth;
        AIBehaviour behaviour = GetComponent<AIBehaviour>();
        healthPort = behaviour.GetPort("Health") as kAIDataPort<float>;
        if(healthPort == null)
        {
            Debug.LogWarning("No health port on AI behaviour");
        }
    }

    public void ApplyDamage(float damage, Vector2 originOfDamage)
    {
        currentHealth -= damage;
        SendMessage("OnDamage", originOfDamage, SendMessageOptions.DontRequireReceiver);
    }

    // Update is called once per frame
    void Update()
    {

        if(healthPort != null)
        {
            healthPort.Data = currentHealth / startingHealth;
        }

        if(currentHealth <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}