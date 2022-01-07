using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;

public class Number : MonoBehaviour
{
    private RectTransform rectTranform;
    private Vector3 velocity = new Vector3(0f, -0.2f, 0f);
    public int currentDroppingColumn;
    public bool isDropped = false;
    public NumberType numType;

    public int CurrentDroppingColumn
    {
        get { return currentDroppingColumn; }
        set { currentDroppingColumn = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rectTranform = GetComponent<RectTransform>();
    }

    public void Setup(Transform parentTrans, Color color, int column, NumberType type)
    {
        GameplayController.Instance.CurrentDroppingNumber = this;
        GameplayController.Instance.isDropping = true;
        transform.name = type.ToString();
        numType = type;
        transform.SetParent(parentTrans);
        GetComponent<Image>().color = color;
        currentDroppingColumn = column;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDropped)
        if (rectTranform.anchoredPosition.y > GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn])
            transform.position = transform.position + velocity * Time.fixedDeltaTime;
        else
        {
            rectTranform.anchoredPosition = new Vector2(rectTranform.anchoredPosition.x, GameplayController.Instance.CurrentColumnHeights()[currentDroppingColumn]);
            isDropped = true;
             GameplayController.Instance.isDropping = false;
             GameplayController.Instance.currentDroppingNumber = null;
             GameplayController.Instance.SetupForNextNumber(currentDroppingColumn);
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

    private IEnumerator WaitForUpgrade(int n)
    {
        yield return new WaitForSeconds(1f);

        numType = (NumberType)(n + (int)numType);
        transform.name = numType.ToString();
        GetComponent<Image>().color = CreateColor(numType);
    }

    public NumberType NumberType { get { return numType; } set { numType = value; } }

    public Color CreateColor( NumberType type)
    {
        var color = new Color();
        switch ((int)type)
        {
            case 0:
                color = Color.red;
                break;
            case 1:
                color = Color.green;
                break;
            case 2:
                color = Color.blue;
                break;
            case 3:
                color = Color.yellow;
                break;
            case 4:
                color = Color.cyan;
                break;
            case 5:
                color = Color.grey;
                break;
            default:
                color = Color.black;
                break;
        }
        return color;
    }
}