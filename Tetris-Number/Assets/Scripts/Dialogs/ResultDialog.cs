using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultDialog : DialogBase
{
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private Button restart;
    [SerializeField] private Button menu;

    private void Start()
    {
        restart.onClick.AddListener(() => GameplayController.Instance.Restart());
        menu.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
    }
    protected override void HideBehaviour()
    {
        scoreText.text =  GameplayController.Instance.Score.ToString();
    }   

    protected override void ShowBehaviour()
    {
        
    }
}
