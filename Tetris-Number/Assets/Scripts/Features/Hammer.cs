using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum HammerType
{
    Hammer,
    ColourHammer
}

public class Hammer : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] RectTransform panel;
    public HammerType type = HammerType.Hammer;

    private bool isUsed = false;
    // Start is called before the first frame update
    protected void Start()
    {
        button.onClick.AddListener(HammerHandle);
    }

    protected void HammerHandle()
    {
        if (GameplayController.Instance.nHammer == 0)
        {
            ButtonController.Instance.ShowDialog(DialogType.Shop);
        }

        if (!isUsed)
        {
            panel.DOAnchorPosY(panel.anchoredPosition.y - 200, 1f);
            GameplayController.Instance.IsUsingHammer = true;
            GameplayController.Instance.currentHammerType = type;
        }
        else
        {
            CancelHammer();
            GameplayController.Instance.IsUsingHammer = false;
        }

        isUsed = !isUsed;
    }

    public void CancelHammer()
    {
        panel.DOAnchorPosY(panel.anchoredPosition.y + 200, 1f);
        isUsed = false;
    }
}