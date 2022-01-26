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
        Debug.Log(GameplayController.Instance.nHammer);
        if ((GameplayController.Instance.nHammer == 0 && type == HammerType.Hammer) || (GameplayController.Instance.nColourHammer == 0 && type == HammerType.ColourHammer))
        {
            ButtonController.Instance.ShowDialog(DialogType.Shop);
        }
        else
        {
            if (!isUsed)
            {
                panel.DOAnchorPosY(panel.anchoredPosition.y - 200, 1f);
                GameplayController.Instance.IsUsingHammer = true;
                GameplayController.Instance.currentHammerType = type;
                isUsed = !isUsed;
            }
            else
            {
                CancelHammer();
                GameplayController.Instance.IsUsingHammer = false;
                GameplayController.Instance.IsPause = false;
            }
        }
    }

    public void CancelHammer()
    {
        panel.DOAnchorPosY(panel.anchoredPosition.y + 200, 1f);
        isUsed = false;
    }
}