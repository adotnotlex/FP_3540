using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldCount : MonoBehaviour
{
    public Text goldCountText;
    public static int RemainingGold = 0;
    private GameObject[] allPots;
    
    // Start is called before the first frame update
    void Start()
    {
        allPots = GameObject.FindGameObjectsWithTag("Pot");
        foreach (var pot in allPots)
        {
            RemainingGold += pot.GetComponent<GoldTracker>().gold;
        }
        UpdateCount();
    }

    // Update is called once per frame
    void Update()
    {
        var leftOvers = 0;
        allPots = GameObject.FindGameObjectsWithTag("Pot");
        foreach (var pot in allPots)
        {
            leftOvers += pot.GetComponent<GoldTracker>().gold;
            RemainingGold = leftOvers;
        }
        
        print(RemainingGold);
        if(!LevelManager.isGameOver)
        {
            if(RemainingGold <= 0)
            {
                FindObjectOfType<LevelManager>().LevelLost();
            }
        }
        
        UpdateCount();
    }
    
    private void UpdateCount()
    {
        goldCountText.text = "Gold Remaining: " + RemainingGold;
    }
}
