using System.Collections;
using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{

    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text highScore;
    [SerializeField] private TMP_Text coin;
    
    // Start is called before the first frame update
    void Start()
    {
        highScore.text = PlayerPrefs.GetInt("high_score").ToString();
        coin.text = PlayerPrefs.GetInt("coint").ToString();
        score.text = 0.ToString();
    }

    public void UpdateScore(int score)
    {
        this.score.text = score.ToString();
    }

    public void UpdateCoin(int coin)
    {
        this.coin.text = coin.ToString();
    }

    public void UpdateHighScore(int highscore)
    {
        this.highScore.text = highScore.ToString();
    }
}
