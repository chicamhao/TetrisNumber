using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameplayController : MonoBehaviour
{

    [SerializeField] private NumberSpawner numberSpawner;
    [SerializeField] private Playground playground;

    private static GameplayController instance;
    private int currentClickedColumnIndex;
    public bool isDropping = false;
    public Number currentDroppingNumber; 

   
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

    public int CurrentClickedColumnIndex { get; set; }

    public bool IsDropping { get; set; }

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
        if (currentDroppingNumber.transform.position.y < CurrentColumnHeights()[column]) return;

        currentDroppingNumber.UpdateNumberAfterSwitch(column, CurrentColumnHeights()[column]);
        playground.UpdateColumnHeight(column);
        Spawn();
        UnityEngine.Debug.Log("col " + column + " height " + CurrentColumnHeights()[column]);
    }

    public void SetupForNextNumber(int droppedColumn)
    {
        playground.UpdateColumnHeight(droppedColumn);
        currentDroppingNumber = null;
        isDropping = false;
        Spawn();
    }
}