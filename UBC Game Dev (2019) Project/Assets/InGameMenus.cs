using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenus : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text deathText;
    [SerializeField] string[] deathMessages;
    private bool paused = false;
    private bool otherMenu = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }


    public void Resume()
    {
        pauseMenu.SetActive(false);
        paused = false;
        Time.timeScale = 1f;
    }

    public void Die()
    {
        if (deathMessages.Length > 0) deathText.text = deathMessages[Random.Range(0, deathMessages.Length)];
        otherMenu = true;
        deathMenu.SetActive(true);
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        paused = true;
        Time.timeScale = 0f;
    }

    public void Win(int score)
    {
        otherMenu = true;
        victoryMenu.SetActive(true);
        scoreText.text = "Score: " + score;
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused && !otherMenu)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}
