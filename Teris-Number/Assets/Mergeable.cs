using UnityEngine;
using System;

enum MergeCase
{
    None,
    Left,
    Right,
    Bottom,
    LeftRight,
    LeftBottom,
    RightBottom,
    LeftRightBottom
}

public class Mergeable : MonoBehaviour
{
    Number[,] board;
    MergeCase mergeCase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MergeNumber()
    {

    }

    public void CheckMergeable(Number target, Vector2 index)
    {
        board = GameplayController.Instance.Board;
        if (board[(int)index.x, (int)index.y] == null)
        {
            throw new ArgumentException("invalid arg");
        }

        var left = CheckMergeableLeft(index);
        var right = CheckMergeableRight(index);
        var bottom = CheckMergeableBottom(index);

        var mergeTuple = (left, right, bottom);
        switch (mergeTuple)
        {
            case (true, true, true):
                mergeCase = MergeCase.LeftRightBottom;
                break;
            case (true, true, false):
                mergeCase = MergeCase.LeftRight;
                break;
            case (true, false, true):
                mergeCase = MergeCase.LeftBottom;
                break;
            case (true, false, false):
                mergeCase = MergeCase.Left;
                break;
            case (false, true, true):
                mergeCase = MergeCase.RightBottom;
                break;
            case (false, true, false):
                mergeCase = MergeCase.Right;
                break;
            case (false, false, true):
                mergeCase = MergeCase.Bottom;
                break;
            default:
                mergeCase = MergeCase.None;
                break;
        }

        Debug.Log(mergeCase);
    }

    private bool CheckMergeableLeft(Vector2 index)
    {
        if (index.x - 1 < 0) return false;
        if (board[(int)index.x - 1, (int)index.y] != null && board[(int)index.x, (int)index.y].name[0] == board[(int)index.x - 1, (int)index.y].name[0])
        {
            return true;
        }

        return false;
    }

    private bool CheckMergeableRight(Vector2 index)
    {
        if (index.x + 1 > Configurations.NORMAL_BOARD_SIZE.X - 1) return false;
        if (board[(int)index.x + 1, (int)index.y] != null && board[(int)index.x, (int)index.y].name[0] == board[(int)index.x + 1, (int)index.y].name[0])
        {
            return true;
        }

        return false;
    }

    private bool CheckMergeableBottom(Vector2 index)
    {
        if (index.y - 1 < 0) return false;
        if (board[(int)index.x, (int)index.y - 1] != null && board[(int)index.x, (int)index.y].name[0] == board[(int)index.x, (int)index.y - 1].name[0])
        {
            return true;
        }
        return false;
    }
}
