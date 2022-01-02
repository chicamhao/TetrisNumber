using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuGameScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume",volume);
    }
}
