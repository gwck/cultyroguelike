using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip theme;

    private void Start()
    {
        SoundManager.Instance.PlayLoop(theme, transform.parent);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ButtonClick()
    {
        SoundManager.Instance.Play(click, transform.parent);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value / 100;
    }
}
