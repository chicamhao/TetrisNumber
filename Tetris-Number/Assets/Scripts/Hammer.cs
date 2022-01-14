using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Hammer : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] RectTransform panel;
    private bool isUsed = false;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(HammerHandle);
    }

    private void HammerHandle()
    {
        if (!isUsed)
        {
            panel.DOAnchorPosY(panel.anchoredPosition.y - 200, 1f);
            GameplayController.Instance.isUsingHammer = true;
        }
        else
        {
            CancelHammer();
            GameplayController.Instance.isUsingHammer = false;
        }

        isUsed = !isUsed;
    }


    public void CancelHammer()
    {
        panel.DOAnchorPosY(panel.anchoredPosition.y + 200, 1f);
        isUsed = false;
    }
}