using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class DialogBase : MonoBehaviour
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
        rectTransform.DOAnchorPosY(0, 1f);
        GameplayController.Instance.Pause();
    }

    public void Hide()
    {
        rectTransform.DOAnchorPosY(initPosition.y, 1f);
        GameplayController.Instance.Continue();
        HideBehaviour();
    }
}
