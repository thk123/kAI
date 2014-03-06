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

    kAIDataPort<GameObject> lastAttackPort;

    // Use this for initialization
    void Start()
    {
        currentHealth = startingHealth;
        AIBehaviour behaviour = GetComponent<AIBehaviour>();
        healthPort = behaviour.GetPort("Health") as kAIDataPort<float>;
        lastAttackPort = behaviour.GetPort("LastAttacker") as kAIDataPort<GameObject>;
        if(healthPort == null)
        {
            Debug.LogWarning("No health port on AI behaviour");
        }

        if (lastAttackPort == null)
        {
            Debug.LogWarning("No on damage trigger");
        }
    }

    public void ApplyDamage(float damage, Vector2 originOfDamage, GameObject sourceOfPain)
    {
        currentHealth -= damage;
        SendMessage("OnDamage", originOfDamage, SendMessageOptions.DontRequireReceiver);

        if (lastAttackPort != null)
        {
            FactionBehaviour lFaction = sourceOfPain.GetComponent<FactionBehaviour>();
            if (lFaction == null || lFaction.IsEnemy(gameObject))
            {
                lastAttackPort.Data = sourceOfPain;
            }
            else
            {
                kAIObject.LogMessage(null, "OI, don't shoot me, I'm an ally");
            }
        }
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

    public float CurrentHealth {
        get
        {
            return currentHealth / startingHealth;
        }
    }
}