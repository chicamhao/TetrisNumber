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
}