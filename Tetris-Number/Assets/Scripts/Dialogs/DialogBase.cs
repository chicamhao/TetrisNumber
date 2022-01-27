using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public interface IDialogBase
{
    public void Show();
    public void Hide();
}

public abstract class DialogBase : MonoBehaviour, IDialogBase
{
    [SerializeField] Button close;
    [SerializeField] RectTransform rectTransform;

    private Vector2 initPosition;
    public void Awake()
    {
        initPosition = rectTransform.anchoredPosition;
        close.onClick.AddListener(() => Hide());
    }

    protected abstract void ShowBehaviour();
    protected abstract void HideBehaviour();

    public void Show()
    {
        ShowBehaviour();
        if (GameplayController.Instance != null)
        {
            GameplayController.Instance.PlayClickAudio();
            GameplayController.Instance.Pause();
        }
        rectTransform.DOAnchorPosY(0, 1f);
    }

    public void Hide()
    {
        if (GameplayController.Instance != null)
        {
            GameplayController.Instance.PlayClickAudio();
            GameplayController.Instance.Continue();
        }
        rectTransform.DOAnchorPosY(initPosition.y, 1f);
        HideBehaviour();
    }

}
