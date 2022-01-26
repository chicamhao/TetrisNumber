using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System;

public class Number : MonoBehaviour
{
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected Button button;

    public int currentDroppingColumn;
    public bool isDropped = false;
    public Vector2 index;
    public NumberType numType;
    public SpecialNumberType specialType;

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
        GameplayController.Instance.isDropping = true;
        transform.SetParent(parentTrans);

        if (specialType == SpecialNumberType.None)
        {
            this.sprites = sprites;
            transform.name = type.ToString();
            numType = type;

            if ((int)type < sprites.Length)
                GetComponent<Image>().sprite = sprites[(int)type];

            currentDroppingColumn = column;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!isDropped && !GameplayController.Instance.IsPause)
        {
            if (rectTransform.anchoredPosition.y > GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn])
                transform.position = transform.position + Configurations.DROPPING_VELOCITY * Time.fixedDeltaTime;
            else
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn]);
                isDropped = true;
                GameplayController.Instance.SetupForNextNumber(currentDroppingColumn);
            }
        }
    }

    public void UpdateNumberAfterSwitch(int column, Vector2 pos)
    {
        if (rectTransform.anchoredPosition.y < pos.y) return;

        if (column != currentDroppingColumn)
        {
            MoveHorizontal(pos.x, 0.2f);
            currentDroppingColumn = column;
        }

        StartCoroutine(Drop(pos.y, 0.2f));
        isDropped = true;
    }
    
    protected void MoveHorizontal(float x, float time)
    {
        transform.DOMoveX(x, time);
    }

    protected IEnumerator Drop(float y, float time)
    {
        yield return new WaitForSeconds(time);

        var target = this.rectTransform;
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
        var target = this.rectTransform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(target.anchoredPosition.x + Configurations.NUMBER_SIZE, 0), 0.2f);
        t.SetOptions(AxisConstraint.X).SetTarget(target);
        yield return t.WaitForKill();
        Destroy(this.gameObject);
    }

    public IEnumerator WaitForMoveLeft()
    {
        yield return new WaitForSeconds(0.7f);
        var target = this.rectTransform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(target.anchoredPosition.x - Configurations.NUMBER_SIZE, 0), 0.2f);
        t.SetOptions(AxisConstraint.X).SetTarget(target);
        yield return t.WaitForKill();
        Destroy(this.gameObject);
    }

    public void DropAUnit()
    {
        StartCoroutine(WaitForDropAUnit());
    }
    public void Drop3Units()
    {
        StartCoroutine(WaitForDrop3Units());
    }


    private IEnumerator WaitForDropAUnit()
    {
        yield return new WaitForSeconds(1f);

        var target = this.rectTransform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(0, target.anchoredPosition.y - Configurations.NUMBER_SIZE), 0.2f);
        t.SetOptions(AxisConstraint.Y).SetTarget(target);
    }

    private IEnumerator WaitForDrop3Units()
    {
        yield return new WaitForSeconds(1f);

        var target = this.rectTransform;
        TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(0, target.anchoredPosition.y - Configurations.NUMBER_SIZE * 3), 0.6f);
        t.SetOptions(AxisConstraint.Y).SetTarget(target);
    }

    public IEnumerator WaitForMoveTop()
    {
        yield return new WaitForSeconds(0.7f);
        var target = this.rectTransform;
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
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    private IEnumerator WaitForUpgrade(int n)
    {
        yield return new WaitForSeconds(1f);

        numType = (NumberType)(n + (int)numType);

        transform.name = numType.ToString();

        var score = numType.ToString().Remove(0, 1);

        GameplayController.Instance.AddScore(Int32.Parse(score) + (int)Mathf.Pow(2, n - 1));

        GetComponent<Image>().sprite = sprites[(int)numType];
    }

    public NumberType NumberType { get { return numType; } set { numType = value; } }
}