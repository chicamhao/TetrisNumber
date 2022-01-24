using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;

public class Number : MonoBehaviour
{
    [SerializeField] private RectTransform rectTranform;
    [SerializeField] private Button button;

    public int currentDroppingColumn;
    public bool isDropped = false;
    public Vector2 index;
    public NumberType numType;

    private Sprite[] sprites;

    public int CurrentDroppingColumn
    {
        get { return currentDroppingColumn; }
        set { currentDroppingColumn = value; }
    }

    private void Start()
    {
        button.onClick.AddListener(() => GameplayController.Instance.OnClickNumber(this));
    }

    public void Setup(Transform parentTrans, Sprite[] sprites, int column, NumberType type)
    {
        this.sprites = sprites;

        GameplayController.Instance.isDropping = true;
        transform.name = type.ToString();
        numType = type;
        transform.SetParent(parentTrans);
        GetComponent<Image>().sprite = sprites[(int)type];
        currentDroppingColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDropped && !GameplayController.Instance.isUsingHammer)
        {
            if (rectTranform.anchoredPosition.y > GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn])
                transform.position = transform.position + Configurations.NumberDroppingVelocity * Time.fixedDeltaTime;
            else
            {
                rectTranform.anchoredPosition = new Vector2(rectTranform.anchoredPosition.x, GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn]);
                isDropped = true;
                // GameplayController.Instance.isDropping = false;
                // GameplayController.Instance.currentDroppingNumber = null;
                GameplayController.Instance.SetupForNextNumber(currentDroppingColumn);
            }
        }
    }

    public void UpdateNumberAfterSwitch(int column, Vector2 pos)
    {
        if (rectTranform.anchoredPosition.y < pos.y) return;

        if (column != currentDroppingColumn)
        {
            MoveHorizontal(pos.x, 0.2f);
            currentDroppingColumn = column;
        }

        StartCoroutine(Drop(pos.y, 0.2f));
        isDropped = true;
    }
    
    private void MoveHorizontal(float x, float time)
    {
        transform.DOMoveX(x, time);
    }

    private IEnumerator Drop(float y, float time)
    {
        yield return new WaitForSeconds(time);

        var target = this.rectTranform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(0, y), 0.5f);
        t.SetOptions(AxisConstraint.Y).SetTarget(target);
    }

    public void MoveRightAUnit()
    {
        StartCoroutine(WaitForMoveRight());
    }

    public void MoveLeftAUnit()
    {
        StartCoroutine(WaitForMoveLeft());
    }

    public void MoveTopAUnit()
    {
        StartCoroutine(WaitForMoveTop());
    }

    public IEnumerator WaitForMoveRight()
    {
        yield return new WaitForSeconds(0.7f);
        var target = this.rectTranform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(target.anchoredPosition.x + Configurations.NUMBER_SIZE, 0), 0.2f);
        t.SetOptions(AxisConstraint.X).SetTarget(target);
        yield return t.WaitForKill();
        Destroy(this.gameObject);
    }

    public IEnumerator WaitForMoveLeft()
    {
        yield return new WaitForSeconds(0.7f);
        var target = this.rectTranform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(target.anchoredPosition.x - Configurations.NUMBER_SIZE, 0), 0.2f);
        t.SetOptions(AxisConstraint.X).SetTarget(target);
        yield return t.WaitForKill();
        Destroy(this.gameObject);
    }

    public void DropAUnit()
    {

        StartCoroutine(WaitForDropAUnit());
    }

    private IEnumerator WaitForDropAUnit()
    {
        yield return new WaitForSeconds(1f);

        var target = this.rectTranform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(0, target.anchoredPosition.y - Configurations.NUMBER_SIZE), 0.2f);
        t.SetOptions(AxisConstraint.Y).SetTarget(target);
    }

    public IEnumerator WaitForMoveTop()
    {
        yield return new WaitForSeconds(0.7f);
        var target = this.rectTranform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(0, target.anchoredPosition.y + Configurations.NUMBER_SIZE), 0.2f);
        t.SetOptions(AxisConstraint.Y).SetTarget(target);
        yield return t.WaitForKill();
        Destroy(this.gameObject);
    }

    public void Upgrade(int n)
    {
        StartCoroutine(WaitForUpgrade(n));
    }

    public void SetRectPosition(float x, float y)
    {
        rectTranform.anchoredPosition = new Vector2(x, y);
    }

    private IEnumerator WaitForUpgrade(int n)
    {
        yield return new WaitForSeconds(1f);

        numType = (NumberType)(n + (int)numType);
        transform.name = numType.ToString();
        GetComponent<Image>().sprite = sprites[(int)numType];
    }

    public NumberType NumberType { get { return numType; } set { numType = value; } }
}