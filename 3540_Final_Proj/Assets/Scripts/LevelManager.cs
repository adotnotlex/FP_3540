using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static bool isGameOver;
    public string nextLevel;
    public Text gameText;
    public static GameObject Player;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        isGameOver = false;
    }

    public void LevelLost()
    {
        isGameOver = true;
        gameText.text = "YOU FAILED!";
        gameText.color = Color.red;
        gameText.gameObject.SetActive(true);
        Invoke("LoadCurrentLevel", 2);
    }

    public void LevelBeat()
    {
        isGameOver = true;
        gameText.text = "YOU CRUSHED THEM!";
        gameText.color = Color.green;
        gameText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        if(!string.IsNullOrEmpty(nextLevel))
        {
            Invoke("LoadNextLevel", 0);
            Time.timeScale = 1f;
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

    void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
