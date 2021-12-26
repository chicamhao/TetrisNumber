using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class GameplayController : MonoBehaviour
{

    [SerializeField] private NumberSpawner numberSpawner;
    [SerializeField] private Playground playground;
    [SerializeField] private Merger merger;

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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        board = new Number[(int)Configurations.NORMAL_BOARD_SIZE.X, (int)Configurations.NORMAL_BOARD_SIZE.Y];
        Invoke("Spawn", 1f);        
    }

    public void Spawn()
    {
        if (isDropping) return;

        numberSpawner.Spawn();
        isDropping = true;
    }

    public void SwitchColumn(int column)
    {
        if (currentDroppingNumber == null) return;

        currentDroppingNumber.UpdateNumberAfterSwitch(column, new Vector2(playground.Columns[column].transform.position.x, CurrentColumnHeights()[column]));
        SetupForNextNumber(column);

    }

    public void SetupForNextNumber(int droppedColumn)
    {
        var currentIdx = new Vector2(droppedColumn, playground.droppedNumbersOnColumns[droppedColumn]);
        board[droppedColumn, playground.droppedNumbersOnColumns[droppedColumn]] = currentDroppingNumber;
        playground.UpdateColumnHeight(droppedColumn, 1);
        merger.MergeNumber(CurrentDroppingNumber, currentIdx);

        currentDroppingNumber = null;
        isDropping = false;

        if (CheckLose(droppedColumn))
            return;

        Invoke("Spawn", 0.5f);
    }

    private bool CheckLose(int column)
    {
        if (playground.droppedNumbersOnColumns[column] == Configurations.NORMAL_BOARD_SIZE.Y - 1)
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

    void LogForDebug()
    {
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 6; ++j)
            {
                if (board[i, j] != null)
                    Debug.Log("i = " + i + ", j = " + j);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogForDebug();
        }
    }
}