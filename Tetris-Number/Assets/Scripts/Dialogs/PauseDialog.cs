using UnityEngine;
using UnityEngine.UI;

public class PauseDialog : DialogBase
{
    [SerializeField] private Button load;
    [SerializeField] private Button save;
    [SerializeField] private Button restart;

    private void Start()
    {
        load.onClick.AddListener(() => 
        {
            GameplayController.Instance.Load();
             Hide();
        });

        save.onClick.AddListener(() =>
        {
            GameplayController.Instance.Save();
            Hide();
        });

        restart.onClick.AddListener(() => 
        {
            GameplayController.Instance.Restart();
            Hide();
        });
    }

    protected override void HideBehaviour()
    {
    }

    protected override void ShowBehaviour()
    {
    }
}