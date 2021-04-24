using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerHealth : MonoBehaviour
{
    int startingHealth = 500;
    public int currentHealth;
    public Slider healthSlider;
    public GameObject enemySpawner;

    void Awake() 
    {
        healthSlider = GetComponentInChildren<Slider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = startingHealth;
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
            Destroy(enemySpawner);
        }

    }

    private void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(25);
        }
    }
}
