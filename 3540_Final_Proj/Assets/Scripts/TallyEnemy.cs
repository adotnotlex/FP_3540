using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TallyEnemy : MonoBehaviour
{
    public static int enemyCount = 0;
    public Text EnemyCounter;
    

    // Start is called before the first frame update
    void Start()
    {
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = allEnemies.Length;
        UpdateCount();
    }

    // Update is called once per frame
    void Update()
    {
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = allEnemies.Length;
        UpdateCount();
        if(!LevelManager.isGameOver)
        {
            // enemyCount--;
            // UpdateCount();
            if(enemyCount <= 0)
            {
                FindObjectOfType<LevelManager>().LevelBeat();
            }
        }
    }
    

    private void UpdateCount()
    {
        EnemyCounter.text = "Enemies Left: " + enemyCount.ToString();
    }


}
