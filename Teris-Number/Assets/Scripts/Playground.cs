using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Playground : MonoBehaviour
{
    private Button[] columns;

    public Button[] Columns
    {
        get { return columns; }
    }

    public int[] CurrentColumnHeights { get; } = new int[5];

    private void Awake()
    {
        columns = gameObject.GetComponentsInChildren<Button>();

        //initilize columns height
        for (int i = 0; i < CurrentColumnHeights.Length; ++i)
        {
            CurrentColumnHeights[i] = (((int)-Configurations.NORMAL_BOARD_SIZE.Y * Configurations.NUMBER_SIZE) + Configurations.NUMBER_SIZE) / 2;
        }
    }
    void Start()
    {
        //add button listener streams, subcribes to switch current number to another branch
        columns[0].OnClickAsObservable()
        .Subscribe(_ =>
        {
            GameplayController.Instance.SwitchColumn(0);
        })
        .AddTo(this);

        columns[1].OnClickAsObservable()
            .Subscribe(_ =>
            {
                GameplayController.Instance.SwitchColumn(1);
            })
            .AddTo(this);

        columns[2].OnClickAsObservable()
            .Subscribe(_ =>
            {
                GameplayController.Instance.SwitchColumn(2);
            })
            .AddTo(this);

        columns[3].OnClickAsObservable()
            .Subscribe(_ =>
            {
                GameplayController.Instance.SwitchColumn(3);
            })
            .AddTo(this);

        columns[4].OnClickAsObservable()
            .Subscribe(_ =>
            {
                GameplayController.Instance.SwitchColumn(4);
            })
            .AddTo(this);
    }

    public void UpdateColumnHeight(int idx)
    {
        CurrentColumnHeights[idx] += Configurations.NUMBER_SIZE;
    }
}
