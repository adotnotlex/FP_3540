using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    
    int startingHealth = 100;
    public int currentHealth;
    public AudioClip playerDeathSFX;
    public Slider healthSlider;

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
            
        }

    }

    private void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(50);
        }
    }

}
