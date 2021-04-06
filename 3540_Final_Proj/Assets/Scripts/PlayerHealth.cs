using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public Slider healthSlider;
    public int currentHealth;
    public GameObject enemy;
    private int enemyDamageAmount;

    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;
        enemyDamageAmount = enemy.GetComponent<EnemyAI>().enemyMeleeDamage;
    }

    private void Update()
    {
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
    
    private void OnCollisionEnter(Collision other)
    {
        print("Collision: " + other.gameObject.tag);
        
        if (other.gameObject.CompareTag("Weapon"))
        {
            AudioSource.PlayClipAtPoint(enemy.GetComponent<EnemyAI>().hitFX, enemy.transform.position);
            TakeDamage(enemyDamageAmount);
        }
    }



    void PlayerDies()
    {
        LevelManager.isGameOver = true;
        FindObjectOfType<LevelManager>().LevelLost();
    }
}
