using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGame : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Button load;
    public Button settings;
    public SettingDialog dialog;

    public TMP_Text highScore;
    public TMP_Text coin;

    private void Start()
    {
        highScore.text = PlayerPrefs.GetInt("high_score").ToString();
        coin.text = PlayerPrefs.GetInt("coin").ToString();

        settings.onClick.AddListener(() => dialog.Show());
        load.onClick.AddListener(() =>
        {
            Configurations.IS_LOAD = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

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
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }
}
