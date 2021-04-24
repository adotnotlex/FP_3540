using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class GoldTracker : MonoBehaviour
{
    public int gold = 0;
    public Slider goldSlider;
    public AudioClip lootFX;
    
    // Start is called before the first frame update
    void Start()
    {
        gold = 100;
        goldSlider.value = gold;
    }

    // Update is called once per frame
    void Update()
    {
        goldSlider.value = gold;
    }

    void Looted()
    {
        if (gold > 0)
        {
            print("Looted: " + gold);
            gold -= 10;
            goldSlider.value = gold;
            AudioSource.PlayClipAtPoint(lootFX, transform.position);
        }
        else
        {
            // var allPots = GameObject.FindGameObjectsWithTag("Pot");
            // print("Before destroy: " + allPots.Length);
            Destroy(gameObject);
            // print("After destroy: " + allPots.Length);
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        print("Looted by: " + other.gameObject.tag);
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            Looted();
        }
    }
}
