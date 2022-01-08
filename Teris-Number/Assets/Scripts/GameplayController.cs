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
        int y = playground.droppedNumbersOnColumns[droppedColumn];
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            {
                for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
                {
                    if ((board[i, j] != null))
                    {
                        Debug.Log("index i = " + i + ", j = " + j + ", numbertype = " + (int)board[i, j].NumberType);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            {
                Debug.Log(playground.CurrentColumnHeights[i]);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
            {
                Debug.Log(playground.droppedNumbersOnColumns[i]);
            }
        }
    }
}