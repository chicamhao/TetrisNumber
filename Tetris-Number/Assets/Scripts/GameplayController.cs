using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private NumberSpawner numberSpawner;
    [SerializeField] private Playground playground;
    [SerializeField] private Merger merger;
    [SerializeField] private Hammer hammer;
    [SerializeField] private ColourHammer colourHammer;
    [SerializeField] private TextUpdater texter;

    private static GameplayController instance;

    public int nHammer;
    public int nColourHammer;
    public HammerType currentHammerType;

    private bool isPause;
    private bool isUsingHammer = false;
    public bool isDropping = false;

    private Number[,] board;
    public Number currentDroppingNumber;

    private int highScore;

    [SerializeField] private Text hammerText;
    [SerializeField] private Text colourHammerText;

    [SerializeField] private AudioSource boomAudio;
    [SerializeField] private AudioSource hammerAudio;
    [SerializeField] private AudioSource clickAudio;

    private void Start()
    {
        instance = this;
        board = new Number[(int)Configurations.NORMAL_BOARD_SIZE.y, (int)Configurations.NORMAL_BOARD_SIZE.y];        
        StartCoroutine(Spawn(0.5f));

        coin = PlayerPrefs.GetInt("coin");
        highScore = PlayerPrefs.GetInt("high_score");
        nHammer = PlayerPrefs.GetInt("hammer");
        nColourHammer = PlayerPrefs.GetInt("colour_hammer");

        //for testing
        coin = 1000;
        nColourHammer = 3;
        nHammer = 3;

        hammerText.text = nHammer.ToString();
        colourHammerText.text = nColourHammer.ToString();
        
        texter.UpdateCoin(coin);
        texter.UpdateHighScore(highScore);

        if (Configurations.IS_LOAD)
        {
            Load();
            Configurations.IS_LOAD = false;
        }
    } 

    public IEnumerator Spawn(float time)
    {
        yield return new WaitForSeconds(time);
        if (currentDroppingNumber == null && !isDropping)
        {
            texter.UpdateScore(score);
            numberSpawner.Spawn();
        }

    }

    public void SwitchColumn(int column)
    {
        if (currentDroppingNumber == null || !isDropping) return;

        isDropping = false;
        currentDroppingNumber.UpdateNumberAfterSwitch(column, new Vector2(playground.Columns[column].transform.position.x, CurrentColumnHeights()[column]));
        SetupForNextNumber(column);

    }

    public void SetupForNextNumber(int droppedColumn)
    {
        var currentIdx = new Vector2(droppedColumn, playground.droppedNumbersOnColumns[droppedColumn]);

        switch (currentDroppingNumber.specialType)
        {
            case SpecialNumberType.None:
                board[(int)currentIdx.x, (int)currentIdx.y] = currentDroppingNumber;
                playground.UpdateColumnHeight(droppedColumn, 1);
                merger.MergeNumber(currentIdx);

                if (CheckLose(droppedColumn))
                {

                    coin += (int)(score * Configurations.COIN_FER_SCORE);
                    texter.UpdateCoin(coin);

                    if (score > highScore)
                    {
                        highScore = score;
                        texter.UpdateHighScore(highScore);
                        PlayerPrefs.SetInt("high_score", highScore);
                    }

                    ButtonController.Instance.ShowDialog(DialogType.Result);
                    return;
                }
                break;
            case SpecialNumberType.BreakingAround:
                BreakAround(currentIdx);
                break;
            case SpecialNumberType.BreakingColumn:
                BreakColumn(currentIdx);
                break;
            case SpecialNumberType.BreakingRow:
                BreakRow(currentIdx);
                break;
        }  
    }

    public void BreakAround(Vector2 index)
    {
        boomAudio.Play();

        int x = (int)index.x;
        int y = (int)index.y;

        var destroyNumbers = new List<Number>();

        var leftVerticalBreakingNumber = 0;
        var rightVerticalBreakingNumber = 0;
        var bottomBreakingNumber = 0;

        var score = 0;
        for (int i = -1; i <= 1; ++i)
        {
            if (index.x - 1 >= 0 && index.y + i >= 0 && index.y + i < Configurations.NORMAL_BOARD_SIZE.y && board[(int)index.x - 1, (int)index.y + i] != null)
            {
                leftVerticalBreakingNumber++;

                var type = board[(int)index.x - 1, (int)index.y + i].gameObject.name;
                type = type.Remove(0, 1);
                Debug.Log(type);
                score += Int32.Parse(type);

                destroyNumbers.Add(board[(int)index.x - 1, (int)index.y + i]);
            }

            if (index.x + 1 < Configurations.NORMAL_BOARD_SIZE.x && index.y + i >= 0 && index.y + i < Configurations.NORMAL_BOARD_SIZE.y && board[(int)index.x + 1, (int)index.y + i] != null)
            {
                var type = board[(int)index.x + 1, (int)index.y + i].gameObject.name;
                type = type.Remove(0, 1);
                score += Int32.Parse(type);

                rightVerticalBreakingNumber++;
                destroyNumbers.Add(board[(int)index.x + 1, (int)index.y + i]);
            }
        }

        if (index.y - 1 >= 0 && board[(int)index.x, (int)index.y - 1] != null)
        {
            var type = board[(int)index.x, (int)index.y - 1].gameObject.name;
            type = type.Remove(0, 1);
            score += Int32.Parse(type);

            bottomBreakingNumber++;
            destroyNumbers.Add(board[(int)index.x, (int)index.y - 1]);
        }

        destroyNumbers
            .ForEach(x => Destroy(x.gameObject, 1f));

        playground.UpdateColumnHeight(x - 1, -leftVerticalBreakingNumber);
        playground.UpdateColumnHeight(x + 1, -rightVerticalBreakingNumber);
        playground.UpdateColumnHeight(x, -bottomBreakingNumber);

        var isMergeable = false;
        if (leftVerticalBreakingNumber == 3)
        {
            int up = 2;
            while (up < Configurations.NORMAL_BOARD_SIZE.y && board[x - 1, y + up] != null)
            {
                isMergeable = true;
                board[x - 1, y + up].Drop3Units();
                board[x - 1, y + up - 3] = board[x - 1, y + up];
                board[x - 1, y + up] = null;
                StartCoroutine(merger.MergeNumber(new Vector2((int)index.x - 1, (int)index.y + up - 3), .5f, false, true));
                up++;
            }
        }

        if (rightVerticalBreakingNumber == 3)
        {
            int up = 2;
            while (up < Configurations.NORMAL_BOARD_SIZE.y && board[x + 1, y + up] != null)
            {
                isMergeable = true;
                board[x + 1, y + up].Drop3Units();
                board[x + 1, y + up - 3] = board[x + 1, y + up];
                board[x + 1, y + up] = null;
                StartCoroutine(merger.MergeNumber(new Vector2((int)index.x + 1, (int)index.y + up - 3), .5f, false, true));
                up++;
            }
        }

        AddScore(score);
        Destroy(currentDroppingNumber.gameObject, 1f);

        if (!isMergeable)
        {
            currentDroppingNumber = null;
            isDropping = false;
            StartCoroutine(Spawn(1f));
        }
    }

    public void BreakColumn(Vector2 index)
    {
        boomAudio.Play();

        var x = (int)index.x;
        var y = (int)index.y;
        var score = 0;

        for (int i = 0; i < playground.droppedNumbersOnColumns[x]; ++i)
        {
            var type = board[x, i].gameObject.name;
            type = type.Remove(0, 1);
            score += Int32.Parse(type);

            Destroy(board[x, i].gameObject, 1f);
        }

        AddScore(score);
        Destroy(currentDroppingNumber.gameObject, 1f);
        playground.UpdateColumnHeight(x , -playground.droppedNumbersOnColumns[x]);
        currentDroppingNumber = null;
        isDropping = false;
        StartCoroutine(Spawn(1f));
    }

    public void BreakRow(Vector2 index)
    {
        boomAudio.Play();

        var x = (int)index.x;
        var y = (int)index.y;

        var idxes = new List<int>();
        var score = 0;

        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; ++i)
        {
            if (board[i, y] != null)
            {
                var type = board[i, y].gameObject.name;
                type = type.Remove(0, 1);
                score += Int32.Parse(type);

                idxes.Add(i);
                Destroy(board[i, y].gameObject, 1f);
                playground.UpdateColumnHeight(i, -1);
            }
        }

        bool isMergeable = false;

        foreach(var i in idxes)
        {
            var upper = 1;
            while (upper < Configurations.NORMAL_BOARD_SIZE.y && board[i, y + upper] != null)
            {
                isMergeable = true;
                board[i, y + upper].DropAUnit();
                board[i, y + upper - 1] = board[i, y + upper];
                board[i, y + upper] = null;
                StartCoroutine(merger.MergeNumber(new Vector2(i, y + upper - 1), .5f, false, true));

                upper++;
            }
        }

        AddScore(score);
        Destroy(currentDroppingNumber.gameObject, 1f);

        if (!isMergeable)
        {
            currentDroppingNumber = null;
            isDropping = false;
            StartCoroutine(Spawn(1f));
        }
    }

    private bool CheckLose(int column)
    {
        if (playground.droppedNumbersOnColumns[column] == Configurations.NORMAL_BOARD_SIZE.y)
        {
            return true;
        }
        return false;
    }

    public void DropColumnHeight(int column)
    {
        playground.UpdateColumnHeight(column, -1);
    }

    public void Save()
    {
        //save column's height
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            PlayerPrefs.SetInt(i.ToString(), playground.CurrentColumnHeights[i]);
        }

        //save dropped numbers on columns
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            var key = "dropped" + i.ToString();
            PlayerPrefs.SetInt(key, playground.droppedNumbersOnColumns[i]);
        }

        //save board
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
            {
                var key = (i, j).ToString();
                if (board[i, j] != null)
                {
                    Debug.Log("index i = " + i + ", j = " + j + ", numbertype = " + (int)board[i, j].NumberType);
                    PlayerPrefs.SetInt(key, (int)board[i, j].NumberType);
                }
                else
                    PlayerPrefs.SetInt(key, -1);
            }
        }

        PlayerPrefs.SetInt("score", score);
    }

    public void Load()
    {
        ClearBoard();

        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            playground.CurrentColumnHeights[i] = PlayerPrefs.GetInt(i.ToString());

        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            var key = "dropped" + i.ToString();
            playground.droppedNumbersOnColumns[i] = PlayerPrefs.GetInt(key);
        }

        score = PlayerPrefs.GetInt("score");

        StartCoroutine(WaitingLoad());
    }

    private IEnumerator WaitingLoad()
    {
        yield return new WaitForSeconds(0.5f);

        numberSpawner.Load(playground.Columns, board);

        if (currentDroppingNumber)
            StartCoroutine(Spawn(.5f));

        texter.UpdateScore(score);
    }

    private void ClearBoard()
    {
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
            {
                if ((board[i, j] != null))
                {
                    Destroy(board[i, j].gameObject, .5f);
                    board[i, j] = null;
                }
            }
        }
    }

    public void OnClickNumber(Number number)
    {
        if (!isUsingHammer) return;
        if (currentHammerType == HammerType.Hammer)
        {
            nHammer--;
            hammerText.text = nHammer.ToString();

            var index = (-1, -1);
            for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            {
                for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
                {
                    if ((board[i, j] != null && board[i, j] == number))
                    {
                        index = (i, j);
                        break;
                    }
                }
                if (index != (-1, -1)) break;
            }
            Destroy(board[index.Item1, index.Item2].gameObject, .5f);
            StartCoroutine(PlayHammerAudio());
            DropColumnHeight(index.Item1);
            DropColumnAndMerge(new Vector2(index.Item1, index.Item2));
            hammer.CancelHammer();
            isUsingHammer = false;
            isPause = false;
        }
        else if (currentHammerType == HammerType.ColourHammer)
        {
            StartCoroutine(PlayHammerAudio());
            nColourHammer--;
            colourHammerText.text = nColourHammer.ToString();
            colourHammer.CancelHammer();
            isUsingHammer = false;
            isPause = false;

            var index = (-1, -1);
            for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            {
                for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
                {
                    if ((board[i, j] != null && board[i, j] == number))
                    {
                        index = (i, j);
                        break;
                    }
                }
                if (index != (-1, -1)) break;
            }

            var list = new List<(int, int)>();
            for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            {
                for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
                {
                    if ((board[i, j] != null && board[index.Item1, index.Item2].numType == board[i, j].numType))
                    {
                        list.Add((i, j));
                        Destroy(board[i, j].gameObject, .5f);
                    }
                }
            }

            Destroy(board[index.Item1, index.Item2].gameObject, .5f);

            foreach (var i in list)
            {
                DropColumnHeight(i.Item1);
                DropColumnAndMerge(new Vector2(i.Item1, i.Item2));
            }
        }
    }

    private void DropColumnAndMerge(Vector2 index)
    {
        //move all current column down
        var i = 1;
        var topNumber = board[(int)index.x, (int)index.y + i];

        while (topNumber != null)
        {
            topNumber.DropAUnit();

            //current is upper 
            board[(int)index.x, (int)index.y + i - 1] = topNumber;
            //upper is null
            board[(int)index.x, (int)index.y + i] = null;

            StartCoroutine(merger.MergeNumber(new Vector2((int)index.x, (int)index.y + i - 1), .5f, true));

            ++i;

            //update topnumber for next loop
            topNumber = board[(int)index.x, (int)index.y + i];
        }
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Continue()
    {
        isPause = false;
    }

    public void Restart()
    {
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
            {
                if ((board[i, j] != null))
                {
                    Destroy(board[i, j].gameObject, 0.5f);
                }
            }
        }

        playground.Init();
        Destroy(currentDroppingNumber.gameObject, .5f);

        board = new Number[(int)Configurations.NORMAL_BOARD_SIZE.y, (int)Configurations.NORMAL_BOARD_SIZE.y];
        isDropping = false;
        currentDroppingNumber = null;
        score = 0;

        StartCoroutine(Spawn(1f));
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void Buy(BuyType type)
    {      
        switch (type)
        {
            case BuyType.OneHammer:
                if (coin > Configurations.ONE_HAMMER_PRICE)
                {
                    coin -= Configurations.ONE_HAMMER_PRICE;
                    nHammer++;
                }
                break;
            case BuyType.ThreeHammer:
                if (coin > Configurations.THREE_HAMMER_PRICE)
                {
                    coin -= Configurations.THREE_HAMMER_PRICE;
                    nHammer += 3;
                }
                break;
            case BuyType.OneColourHammer:
                if (coin > Configurations.ONE_COLOURHAMMER_PRICE)
                {
                    coin -= Configurations.ONE_COLOURHAMMER_PRICE;
                    nColourHammer++;
                }
                break;
            case BuyType.ThreeColourHammer:
                if (coin > Configurations.THREE_COLOURHAMMER_PRICE)
                {
                    coin -= Configurations.THREE_COLOURHAMMER_PRICE;
                    nColourHammer += 3;
                }
                break;
        }

        hammerText.text = nHammer.ToString();
        colourHammerText.text = nColourHammer.ToString();
        texter.UpdateCoin(coin);
    }

    IEnumerator PlayHammerAudio()
     {
        yield return new WaitForSeconds(.01f);
        hammerAudio.Play();
     }
     public void PlayClickAudio()
    {
        clickAudio.Play();
    }
    /*
     * properties
     */
    public bool IsPause { get { return isPause; } set { isPause = value; } }
    public bool IsUsingHammer
    {
        get { return isUsingHammer; }
        set
        {
            if (value)
            {
                isPause = true;
            }

            isUsingHammer = value;
        }
    }

    private int score = 0;
    private int coin = 0;
    public int Score { get { return score; } }
    public int Coin { get { return coin; } }

    public Number[,] Board { get { return board; } }

    public static GameplayController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Attempt to intialize a second singleton instance!");
                instance = new GameplayController();
                return instance;
            }
        }
    }

    public Number CurrentDroppingNumber
    {
        get { return currentDroppingNumber; }
        set { currentDroppingNumber = value; }
    }

    public int[] CurrentColumnHeights()
    {
        return playground.CurrentColumnHeights;
    }

    public Button[] Columns()
    {
        return playground.Columns;
    }

    public void OnDestroy()
    {
        PlayerPrefs.SetInt("coin", coin);
        PlayerPrefs.GetInt("high_score", highScore);
        PlayerPrefs.GetInt("hammer", nHammer);
        PlayerPrefs.GetInt("colour_hammer", nColourHammer);
    }
}