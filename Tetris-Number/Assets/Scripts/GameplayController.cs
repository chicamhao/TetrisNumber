using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        board[droppedColumn, playground.droppedNumbersOnColumns[droppedColumn]] = currentDroppingNumber;
        playground.UpdateColumnHeight(droppedColumn, 1);
        
        merger.MergeNumber(currentIdx);

        if (CheckLose(droppedColumn))
            return;
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

            merger.MergeNumber(new Vector2((int)index.x, (int)index.y + i - 1), .5f);

            ++i;

            //update topnumber for next loop
            topNumber = board[(int)index.x, (int)index.y + i];
        }
    }
}