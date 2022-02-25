using UnityEngine;
using UnityEngine.UI;

public enum DialogType
{
    Pause,
    Result,
    Shop,
};

public class ButtonController : MonoBehaviour
{
    [SerializeField] Button pause;
    [SerializeField] Button shop;

    [SerializeField] PauseDialog pauseDialog;
    [SerializeField] ResultDialog resultDialog;
    [SerializeField] ShopDialog shopDialog;

    private static ButtonController instance;

    public static ButtonController Instance
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
                instance = new ButtonController();
                return instance;
            }
        }
    }

    void Start()
    {
        instance = this;

        pause.onClick.AddListener(() => pauseDialog.Show());
        shop.onClick.AddListener(() => shopDialog.Show());
    }

    public void ShowDialog(DialogType type)
    {
        switch (type)
        {
            case DialogType.Result:
                resultDialog.Show();
                break;
            case DialogType.Shop:
                shopDialog.Show();
                break;
            default:
                break;
        }
    }
}
