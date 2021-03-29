using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public Slider healthSlider;
    int currentHealth;

    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if(currentHealth > 0)
        {
            currentHealth -= damageAmount;
            healthSlider.value = currentHealth;
        }
        
        if(currentHealth <= 0)
        {
            PlayerDies();
        }
    }

    public void TakeHealing(int healingAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + healingAmount, 0, 100);
        healthSlider.value = currentHealth;
    }

    void PlayerDies()
    {
        LevelManager.isGameOver = true;
        FindObjectOfType<LevelManager>().LevelLost();
    }
}
