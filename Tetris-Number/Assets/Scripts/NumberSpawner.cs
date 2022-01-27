using UnityEngine;
using UnityEngine.UI;

public enum NumberType
{
    N2,
    N4,
    N8,
    N16,
    N32,
    N64,
    N128,
    N256,
    N512,
    N1024,
    N2048,
    N4096,
    N8192,
    N16384,
    N32768,
    N65536,
    N131072,
    SSS
};

public enum SpecialNumberType
{
    None,
    BreakingAround,
    BreakingRow,
    BreakingColumn
}

public class NumberSpawner : MonoBehaviour
{
    [SerializeField] Number[] numberPrefab;

    private Button[] columns;

    [SerializeField]
    private Sprite[] sprites;

    private int currentMaximumSpawningNumber = (int)NumberType.N64;

    public void Spawn()
    {
        columns = GameplayController.Instance.Columns();
        var rand = Random.Range(0, columns.Length - 1);
        var type = (NumberType)HighLevelRandomNumber();
        //var type = (NumberType)SpawningNumberTest();
        Number number;
        if ((int)type > currentMaximumSpawningNumber)
        {
            number = Instantiate(numberPrefab[(int)type - currentMaximumSpawningNumber], columns[rand].transform);
        }
        else
        {
            number = Instantiate(numberPrefab[0], columns[rand].transform);
        }
        
        number.Setup(this.transform, sprites, rand, type);
        GameplayController.Instance.CurrentDroppingNumber = number;
    }

    //demo random
    public int RandomNumber()
    {
        var rand = Random.Range((int)NumberType.N2, currentMaximumSpawningNumber + (int)SpecialNumberType.BreakingColumn + 1);
        return rand;
    }

    public int HighLevelRandomNumber()
    {
        float rand = Random.Range(0f, 1f);
        var result = 0;

        if (rand < Configurations.SPECIAL_NUMBER_RATIO)
            result = Random.Range(currentMaximumSpawningNumber, currentMaximumSpawningNumber + (int)SpecialNumberType.BreakingColumn);
        else
            result = Random.Range((int)NumberType.N2, currentMaximumSpawningNumber);

        return result;
    }

    //remove after test
    int s = 0;
    public int SpawningNumberTest()
    {
        if (s > currentMaximumSpawningNumber)
        {
            s = 0;
            return currentMaximumSpawningNumber + (int)SpecialNumberType.BreakingRow;
        }
        s++;

        return s;
    }

    public void UpdateCurrentMaximumNumber()
    {
        currentMaximumSpawningNumber++;
    }

    public void Load(Button[] columns, Number[,] board)
    {
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
            {
                if (PlayerPrefs.GetInt((i, j).ToString()) == -1) break;

                var number = Instantiate(numberPrefab[0], columns[i].transform);
                var type = (NumberType)PlayerPrefs.GetInt((i, j).ToString());
                number.Setup(this.transform, sprites, j, type);
                var verPos = -((int)Configurations.NORMAL_BOARD_SIZE.x / 2 * Configurations.NUMBER_SIZE) + (i * (Configurations.NUMBER_SIZE));
                var horPos = -((int)Configurations.NORMAL_BOARD_SIZE.y / 2 * Configurations.NUMBER_SIZE - Configurations.NUMBER_SIZE/2)  + (j * (Configurations.NUMBER_SIZE));
                number.SetRectPosition(verPos, horPos);
                number.isDropped = true; 
                board[i, j] = number;
            }
        }
    }
}
