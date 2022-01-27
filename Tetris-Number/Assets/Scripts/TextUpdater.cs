using System.Collections;
using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{

    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text highScore;
    [SerializeField] private TMP_Text coin;
    


    public void UpdateScore(int score)
    {
        this.score.text = score.ToString();
    }

    public void UpdateCoin(int coin)
    {
        Debug.Log(coin);
        this.coin.text = coin.ToString();
    }

    public void UpdateHighScore(int highscore)
    {
        this.highScore.text = highScore.ToString();
    }
}
