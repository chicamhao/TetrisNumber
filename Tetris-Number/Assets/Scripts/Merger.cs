using UnityEngine;
using System.Collections;
using System;

public class Merger : MonoBehaviour
{
    Number[,] board;

    public AudioSource mergeAudio;
    public void MergeNumber(Vector2 index)
    {
        this.board = GameplayController.Instance.Board;

        if (board[(int)index.x, (int)index.y] == null)
        {
            throw new ArgumentException("invalid arg: " + index);
        }

        if (!CheckMergeableLeft(index) && !CheckMergeableRight(index) && !CheckMergeableBottom(index))
        {
            GameplayController.Instance.isDropping = false;
            GameplayController.Instance.CurrentDroppingNumber = null;
            StartCoroutine(GameplayController.Instance.Spawn(0.5f));
        }
        else
        {
            StartCoroutine(MergeNumber(index, 0));
        }
    }

    public IEnumerator MergeNumber(Vector2 index, float time, bool isUseHammer = false, bool isBreakingAround = false)
    { 
        if (isBreakingAround || isUseHammer)
            this.board = GameplayController.Instance.Board;

        yield return new WaitForSeconds(time);

        var nMergeCases = 0;
        var isBottomCase = false;

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

                StartCoroutine(MergeNumber(new Vector2((int)index.x - 1, (int)index.y + i - 1), .5f));

                //top is null
                board[(int)index.x - 1, (int)index.y + i] = null;

                ++i;
                topNumber = board[(int)index.x - 1, (int)index.y + i];
            }
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

                //current is top 
                board[(int)index.x + 1, (int)index.y + i - 1] = board[(int)index.x + 1, (int)index.y + i];
                //top is null
                board[(int)index.x + 1, (int)index.y + i] = null;

                StartCoroutine(MergeNumber(new Vector2((int)index.x + 1, (int)index.y + i - 1), .5f, isUseHammer));

                ++i;
                topNumber = board[(int)index.x + 1, (int)index.y + i];
            }
        }

        if (CheckMergeableBottom(index))
        {
            nMergeCases++;

            isBottomCase = true;

            board[(int)index.x, (int)index.y - 1].MoveTopAUnit();
            board[(int)index.x, (int)index.y].DropAUnit();
            board[(int)index.x, (int)index.y - 1] = board[(int)index.x, (int)index.y];
            GameplayController.Instance.CurrentDroppingNumber = board[(int)index.x, (int)index.y - 1];
            board[(int)index.x, (int)index.y] = null;

            GameplayController.Instance.DropColumnHeight((int)index.x);
        }

        if (nMergeCases > 0)
        {
            if (!isBottomCase)
            {
                StartCoroutine(PlayMerge());
                board[(int)index.x, (int)index.y].Upgrade(nMergeCases);
                StartCoroutine(MergeNumber(new Vector2((int)index.x, (int)index.y), 1.1f, isUseHammer));
            }
            else
            {
                StartCoroutine(PlayMerge());
                board[(int)index.x, (int)index.y - 1].Upgrade(nMergeCases);
                StartCoroutine(MergeNumber(new Vector2((int)index.x, (int)index.y - 1), 1.1f, isUseHammer));
            }
        }
        else
        {
            //base case
            if ((board[(int)index.x, (int)index.y] == GameplayController.Instance.CurrentDroppingNumber && !isUseHammer) || isBreakingAround)
            {
                GameplayController.Instance.isDropping = false;
                GameplayController.Instance.CurrentDroppingNumber = null;
                StartCoroutine(GameplayController.Instance.Spawn(0f));
            }
        }
    }

    IEnumerator PlayMerge()
    {
        yield return new WaitForSeconds(.5f);
        mergeAudio.Play();
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
        if (index.x + 1 > Configurations.NORMAL_BOARD_SIZE.x - 1) return false;
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
