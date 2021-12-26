using UnityEngine;
using System.Collections;
using System;

public class Merger : MonoBehaviour
{
    Number[,] board;

    public IEnumerator MergeNumber(Number[,] board , Vector2 index, float time)
    {
        yield return new WaitForSeconds(time);

        this.board = board;

        if (board[(int)index.x, (int)index.y] == null)
        {
            throw new ArgumentException("invalid arg");
        }

        var nMergeCases = 0;

        if (CheckMergeableLeft(index)) 
        {
            nMergeCases++;

            //merge after drop
            Number left = board[(int)index.x - 1, (int)index.y];
            left.MoveRightAUnit();
            GameplayController.Instance.DropColumnHeight((int)index.x - 1);

            //move all left column down
            var i = 1;
            var topNumber = board[(int)index.x - 1, (int)index.y + i];
            while (topNumber != null)
            {
                //drop that top unit
                topNumber.DropAUnit();

                //current is top
                board[(int)index.x - 1, (int)index.y + i - 1] = board[(int)index.x - 1, (int)index.y + i];                
                //top is null
                board[(int)index.x - 1, (int)index.y + i] = null;

                ++i;
                topNumber = board[(int)index.x - 1, (int)index.y + i];
            }

            Debug.Log("dropped " + i + " units");
        }

        if (CheckMergeableRight(index))
        {
            nMergeCases++;

            Number right = board[(int)index.x + 1, (int)index.y];
            right.MoveLeftAUnit();
            GameplayController.Instance.DropColumnHeight((int)index.x + 1);

            //move all right column down
            var i = 1;
            var topNumber = board[(int)index.x + 1, (int)index.y + i];
            while (topNumber != null)
            {
                //drop that top unit
                topNumber.DropAUnit();

                //upper is top 
                board[(int)index.x + 1, (int)index.y + i - 1] = board[(int)index.x + 1, (int)index.y + i];
                //top is null
                board[(int)index.x + 1, (int)index.y + i] = null;

                ++i;
                topNumber = board[(int)index.x + 1, (int)index.y + i];
            }
        }

        if (CheckMergeableBottom(index))
        {
            nMergeCases++;

            board[(int)index.x, (int)index.y - 1].MoveTopAUnit();
            board[(int)index.x, (int)index.y].DropAUnit();
            board[(int)index.x, (int)index.y - 1] = board[(int)index.x, (int)index.y];
            board[(int)index.x, (int)index.y] = null;
            GameplayController.Instance.DropColumnHeight((int)index.x);
        }

        Debug.Log("upgrade to " + nMergeCases + "level");

        if (nMergeCases > 0)
            GameplayController.Instance.CurrentDroppingNumber.Upgrade(nMergeCases);
    }


    private bool CheckMergeableLeft(Vector2 index)
    {
        if (index.x - 1 < 0) return false;
        if (board[(int)index.x - 1, (int)index.y] != null && board[(int)index.x, (int)index.y].numType == board[(int)index.x - 1, (int)index.y].numType)
        {
            return true;
        }

        return false;
    }

    private bool CheckMergeableRight(Vector2 index)
    {
        if (index.x + 1 > Configurations.NORMAL_BOARD_SIZE.X - 1) return false;
        if (board[(int)index.x + 1, (int)index.y] != null && board[(int)index.x, (int)index.y].numType == board[(int)index.x + 1, (int)index.y].numType)
        {
            return true;
        }

        return false;
    }

    private bool CheckMergeableBottom(Vector2 index)
    {
        if (index.y - 1 < 0) return false;
        if (board[(int)index.x, (int)index.y - 1] != null && board[(int)index.x, (int)index.y].numType == board[(int)index.x, (int)index.y - 1].numType)
        {
            return true;
        }
        return false;
    }

}
