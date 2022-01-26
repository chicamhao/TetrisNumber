using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Playground : MonoBehaviour
{
    private Button[] columns;
    public int[] droppedNumbersOnColumns; 

    public Button[] Columns { get { return columns; } }

    public int[] CurrentColumnHeights { get; } = new int[5];

    private void Awake()
    {
        columns = gameObject.GetComponentsInChildren<Button>();

        //initilize columns 
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < CurrentColumnHeights.Length; ++i)
        {
            CurrentColumnHeights[i] = (((int)-Configurations.NORMAL_BOARD_SIZE.y * Configurations.NUMBER_SIZE) + Configurations.NUMBER_SIZE) / 2;
        }

        droppedNumbersOnColumns = new int[(int)Configurations.NORMAL_BOARD_SIZE.y];
    }
    void Start()
    {
        //add button listener streams, subcribes to switch current number to another branch
        columns[0].onClick.AddListener(() => GameplayController.Instance.SwitchColumn(0));
        columns[1].onClick.AddListener(() => GameplayController.Instance.SwitchColumn(1));
        columns[2].onClick.AddListener(() => GameplayController.Instance.SwitchColumn(2));
        columns[3].onClick.AddListener(() => GameplayController.Instance.SwitchColumn(3));
        columns[4].onClick.AddListener(() => GameplayController.Instance.SwitchColumn(4));
    }

    public void UpdateColumnHeight(int idx, int isDropped)
    {
        if (idx >= 0 && idx < Configurations.NORMAL_BOARD_SIZE.x)
        {      
            CurrentColumnHeights[idx] += (isDropped) * Configurations.NUMBER_SIZE;
            droppedNumbersOnColumns[idx] += isDropped;

            if (droppedNumbersOnColumns[idx] < 0)
                droppedNumbersOnColumns[idx] = 0;
        }
    }
}