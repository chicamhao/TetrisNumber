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
    N2048
};

public class NumberSpawner : MonoBehaviour
{
    [SerializeField] Number numberPrefab;

    private Button[] columns;

    public void Spawn()
    {
        columns = GameplayController.Instance.Columns();
        var rand = Random.Range(0, columns.Length - 1);
        Number number = Instantiate(numberPrefab, columns[rand].transform);
        var type = new NumberType();
        number.Setup(this.transform, CreateColor(out type), rand, type);
        GameplayController.Instance.CurrentDroppingNumber = number;
    }

    public Color CreateColor(out NumberType type)
    {
        var rand = Random.Range((int)NumberType.N2, (int)NumberType.N64);
        type = (NumberType)rand;
        return GetColor(type);
    }

    public Color GetColor(NumberType type)
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

    public void Load(Button[] columns, Number[,] board)
    {
        for (int i = 0; i < Configurations.NORMAL_BOARD_SIZE.x; i++)
        {
            for (int j = 0; j < Configurations.NORMAL_BOARD_SIZE.y; j++)
            {
                if (PlayerPrefs.GetInt((i, j).ToString()) == -1) break;

                var number = Instantiate(numberPrefab, columns[i].transform);
                var type = (NumberType)PlayerPrefs.GetInt((i, j).ToString());
                number.Setup(this.transform, GetColor(type), j, type);
                var verPos = -((int)Configurations.NORMAL_BOARD_SIZE.x / 2 * Configurations.NUMBER_SIZE) - 10 + (i * (Configurations.NUMBER_SIZE + 5));
                var horPos = -((int)Configurations.NORMAL_BOARD_SIZE.y / 2 * Configurations.NUMBER_SIZE - Configurations.NUMBER_SIZE/2)  + (j * (Configurations.NUMBER_SIZE));
                number.SetRectPosition(verPos, horPos);
                number.isDropped = true; 
                board[i, j] = number;
            }
        }
    }
}
