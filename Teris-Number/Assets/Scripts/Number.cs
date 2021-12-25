using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Number : MonoBehaviour
{
    private RectTransform rectTranform;
    private Vector3 velocity = new Vector3(0f, -0.2f, 0f);
    public int currentDroppingColumn;
    public bool isDropped = false;

    public int CurrentDroppingColumn
    {
        get { return currentDroppingColumn; }
        set { currentDroppingColumn = value; }
    }

    public void Drop(float y)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        rectTranform = GetComponent<RectTransform>();
    }

    public void Setup(Transform parentTrans, Color color, int column)
    {
        GameplayController.Instance.CurrentDroppingNumber = this;

        transform.SetParent(parentTrans);
        GetComponent<Image>().color = color;
        currentDroppingColumn = column;
        Debug.Log("current column = " + column);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDropped)
        if (rectTranform.anchoredPosition.y > GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn])
            transform.position = transform.position + velocity * Time.fixedDeltaTime;

        else
        {
            UnityEngine.Debug.Log("dropped");
            rectTranform.anchoredPosition = new Vector2(rectTranform.anchoredPosition.x, GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn]);
            isDropped = true;
            GameplayController.Instance.SetupForNextNumber(currentDroppingColumn);
        }
    }

    public void UpdateNumberAfterSwitch(int column, int height)
    {
        Drop(height);
        currentDroppingColumn = column;
        isDropped = true;
    }
}