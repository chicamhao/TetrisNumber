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

    private static GameplayController instance;
    private Number[,] board;

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

        board = new Number[(int)Configurations.NORMAL_BOARD_SIZE.X, (int)Configurations.NORMAL_BOARD_SIZE.Y];
        
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
        if (playground.droppedNumbersOnColumns[column] == Configurations.NORMAL_BOARD_SIZE.Y)
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
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.X; i++)
        {
            PlayerPrefs.SetInt(i.ToString(), playground.CurrentColumnHeights[i]);
        }

        //save board
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.X; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.Y; j++)
            {
                if (board[i, j] != null)
                {
                    Debug.Log("index i = " + i + ", j = " + j + ", numbertype = " + (int)board[i, j].NumberType);
                    PlayerPrefs.SetInt((i, j).ToString(), (int)board[i, j].NumberType);
                }
                else
                    PlayerPrefs.SetInt((i, j).ToString(), -1);
            }
        }
    }

    private void ClearBoard()
    {
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.X; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.Y; j++)
            {
                if ((board[i, j] != null))
                    Destroy(board[i, j].gameObject, .5f);
            }
        }
    }

    private void Load()
    {
        ClearBoard();
        Destroy(currentDroppingNumber.gameObject, .5f);
        isDropping = false;

        var columnsHeight = new int[(int)Configurations.NORMAL_BOARD_SIZE.X];

        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.X; i++)
        {
            columnsHeight[i] = PlayerPrefs.GetInt(i.ToString());
        }

        playground.SetColumnHeights(columnsHeight);

        StartCoroutine(WaitingLoad());
    }
    private IEnumerator WaitingLoad()
    {
        yield return new WaitForSeconds(1f);

        numberSpawner.Load(playground.Columns, board);

        Spawn(.5f);
    }
}