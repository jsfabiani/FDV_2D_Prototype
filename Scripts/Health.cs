using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        ResetHealth();
    }

    public void ModifyHealth(float amount)
    {
        currentHealth += amount;

        // Ensure that health is within bounds
        if (currentHealth > maxHealth)
            ResetHealth();
        if (currentHealth < 0)
            currentHealth = 0;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }


}
