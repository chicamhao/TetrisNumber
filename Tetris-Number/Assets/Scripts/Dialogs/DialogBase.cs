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
        GameplayController.Instance.PlayClickAudio();
        rectTransform.DOAnchorPosY(0, 1f);
        GameplayController.Instance.Pause();
    }

    public void Hide()
    {
        GameplayController.Instance.PlayClickAudio();
        rectTransform.DOAnchorPosY(initPosition.y, 1f);
        GameplayController.Instance.Continue();
        HideBehaviour();
    }

}
