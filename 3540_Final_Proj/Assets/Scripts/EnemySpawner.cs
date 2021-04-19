using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;

    public float xMin = -8;

    public float xMax = 8;

    public float yMin = 0;

    public float yMax = 3;

    public float zMin = -8;

    public float zMax = 8;

    public float spawnTime = 5;

    public float initialDelay = 8;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemies", initialDelay, spawnTime);
    }

    void SpawnEnemies()
    {
        Vector3 enemyPosition;
        enemyPosition.x = Random.Range(xMin, xMax);
        enemyPosition.y = Random.Range(yMin, yMax);
        enemyPosition.z = Random.Range(zMin, zMax);
        enemyPosition += transform.position;
        GameObject spawnedEnemy = Instantiate(enemyPrefab, enemyPosition, transform.rotation) as GameObject;
        spawnedEnemy.transform.parent = null;
    }
}
