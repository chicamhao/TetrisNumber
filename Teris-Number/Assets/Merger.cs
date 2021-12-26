using UnityEngine;
using System;

public class Merger : MonoBehaviour
{
    Number[,] board;

    public void MergeNumber(Number number, Vector2 index)
    {
        board = GameplayController.Instance.Board;
        if (board[(int)index.x, (int)index.y] == null)
        {
            throw new ArgumentException("invalid arg");
        }

        if (CheckMergeableLeft(index)) 
        {
            Number left = board[(int)index.x - 1, (int)index.y];
            left.MoveRightAUnit();
            GameplayController.Instance.DropColumnHeight((int)index.x - 1);


            /*            var i = 1;
                        var topNumber = board[(int)index.x, (int)index.y + i];
                        while (topNumber != null)
                        {
                            GameplayController.Instance.DropColumnHeight((int)index.x - 1);
                            board[(int)index.x, (int)index.y + i - 1] = board[(int)index.x, (int)index.y + i];
                            topNumber.DropAUnit();
                            board[(int)index.x, (int)index.y + i] = null;
                            ++i;
                            topNumber = board[(int)index.x, (int)index.y + i];
                        }*/
        }
        if (CheckMergeableRight(index))
        {
            Number right = board[(int)index.x + 1, (int)index.y];
            right.MoveLeftAUnit();
            GameplayController.Instance.DropColumnHeight((int)index.x + 1);
        }

        if (CheckMergeableBottom(index))
        {
            board[(int)index.x, (int)index.y - 1].MoveTopAUnit();
            board[(int)index.x, (int)index.y].DropAUnit();
            board[(int)index.x, (int)index.y - 1] = board[(int)index.x, (int)index.y];
            board[(int)index.x, (int)index.y] = null;
            GameplayController.Instance.DropColumnHeight((int)index.x);
        }
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
