using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    public GameObject player;

    public AudioClip hitFX;

    public GameObject enemy;

    private int enemyMeleeDamage;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyMeleeDamage = enemy.GetComponent<EnemyAI>().enemyMeleeDamage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamagePlayer()
    {
        OnTriggerEnter(player.GetComponent<Collider>());
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = player.GetComponent<PlayerHealth>();
    
        if (other.CompareTag("Player"))
        {
            playerHealth.TakeDamage(enemyMeleeDamage);
            AudioSource.PlayClipAtPoint(hitFX, transform.position);
        }
    }
    
    // private void OnCollisionEnter(Collision collision) 
    // {
    //     var playerHealth = player.GetComponent<PlayerHealth>();
    //     
    //     if(collision.gameObject.CompareTag("Player"))
    //     {
    //         playerHealth.TakeDamage(enemyMeleeDamage);
    //         AudioSource.PlayClipAtPoint(hitFX, transform.position);
    //         // TakeDamage(50);
    //     }
    // }

}
