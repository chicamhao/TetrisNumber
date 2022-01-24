using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameplayController : MonoBehaviour
{

    [SerializeField] private NumberSpawner numberSpawner;
    [SerializeField] private Playground playground;
    [SerializeField] private Merger merger;
    [SerializeField] private Button save;
    [SerializeField] private Button load;
    [SerializeField] private Hammer hammer;

    private static GameplayController instance;
    private Number[,] board;
    public bool isUsingHammer = false;
    public bool isDropping = false;
    public Number currentDroppingNumber; 
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

    private void Start()
    {
        instance = this;

        board = new Number[(int)Configurations.NORMAL_BOARD_SIZE.y, (int)Configurations.NORMAL_BOARD_SIZE.y];
        
        save.onClick.AddListener(Save);
        load.onClick.AddListener(Load);
        StartCoroutine(Spawn(0.5f));
    }

    public IEnumerator Spawn(float time)
    {
        yield return new WaitForSeconds(time);
        if (currentDroppingNumber == null && !isDropping)
            numberSpawner.Spawn();
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

        switch (currentDroppingNumber.specType)
        {
            case SpecialNumberType.None:
                board[(int)currentIdx.x, (int)currentIdx.y] = currentDroppingNumber;
                playground.UpdateColumnHeight(droppedColumn, 1);
                merger.MergeNumber(currentIdx);

                if (CheckLose(droppedColumn))
                {
                    Time.timeScale = 0f;
                    return;
                }
                break;
            case SpecialNumberType.BreakingAround:
                BreakAround(currentIdx);
                break;
            case SpecialNumberType.BreakingColumn:
                BreakColumn(currentIdx);
                break;
        }  
    }

    public void BreakAround(Vector2 index)
    {
        int x = (int)index.x;
        int y = (int)index.y;

        var destroyNumbers = new List<Number>();

        var leftVerticalBreakingNumber = 0;
        var rightVerticalBreakingNumber = 0;
        var bottomBreakingNumber = 0;
        for (int i = -1; i <= 1; ++i)
        {
            if (index.x - 1 >= 0 && index.y + i >= 0 && index.y + i < Configurations.NORMAL_BOARD_SIZE.y && board[(int)index.x - 1, (int)index.y + i] != null)
            {
                leftVerticalBreakingNumber++;
                destroyNumbers.Add(board[(int)index.x - 1, (int)index.y + i]);
            }

            if (index.x + 1 < Configurations.NORMAL_BOARD_SIZE.x && index.y + i >= 0 && index.y + i < Configurations.NORMAL_BOARD_SIZE.y && board[(int)index.x + 1, (int)index.y + i] != null)
            {
                rightVerticalBreakingNumber++;
                destroyNumbers.Add(board[(int)index.x + 1, (int)index.y + i]);
            }
        }

        if (index.y - 1 >= 0 && board[(int)index.x, (int)index.y - 1] != null)
        {
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
        Debug.Log(index);
        var x = (int)index.x;
        var y = (int)index.y;

        for (int i = 0; i < playground.droppedNumbersOnColumns[x]; ++i)
        {

            Destroy(board[x, i].gameObject, 1f);
        }

        Destroy(currentDroppingNumber.gameObject, 1f);
        playground.UpdateColumnHeight(x , -playground.droppedNumbersOnColumns[x]);
        currentDroppingNumber = null;
        isDropping = false;
        StartCoroutine(Spawn(1f));
    }

    private bool CheckLose(int column)
    {
        if (playground.droppedNumbersOnColumns[column] == Configurations.NORMAL_BOARD_SIZE.y)
        {
            Debug.Log("LOSE");
            return true;
        }
        return false;
    }

    public void DropColumnHeight(int column)
    {
        playground.UpdateColumnHeight(column, -1);
    }

    private void Save()
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
    }

    private void Load()
    {
        ClearBoard();

        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            playground.CurrentColumnHeights[i] = PlayerPrefs.GetInt(i.ToString());

        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            var key = "dropped" + i.ToString();
            playground.droppedNumbersOnColumns[i] = PlayerPrefs.GetInt(key);
        }

        StartCoroutine(WaitingLoad());
    }

    private IEnumerator WaitingLoad()
    {
        yield return new WaitForSeconds(1f);

        numberSpawner.Load(playground.Columns, board);

        if (currentDroppingNumber)
            StartCoroutine(Spawn(.5f));
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

        var index = (-1, -1);
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
            {
                if ((board[i, j] != null && board[i,j] == number))
                {
                    index = (i, j);
                    break;
                }
            }
            if (index != (-1, -1)) break;
        }
        Destroy(board[index.Item1, index.Item2].gameObject, .5f);
        DropColumnHeight(index.Item1);
        DropColumnAndMerge(new Vector2(index.Item1, index.Item2));
        hammer.CancelHammer();
        isUsingHammer = false;
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
}