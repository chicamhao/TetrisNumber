using UnityEngine;
using UnityEngine.UI;

public enum NumberType
{
    N2,
    N4,
    N8,
    N16,
    N32,
    N64
};

public class NumberSpawner : MonoBehaviour
{
    [SerializeField] Number numberPrefab;

    private Button[] columns;

    public void Spawn()
    {
        if (GameplayController.Instance.CurrentDroppingNumber != null) return;
        columns = GameplayController.Instance.Columns();
        var rand = Random.Range(0, columns.Length - 1);
        Number number = Instantiate(numberPrefab, columns[rand].transform);
        var type = new NumberType();
        number.Setup(this.transform, CreateColor(out type), rand, type);
        
    }

    public Color CreateColor(out NumberType type)
    {
        var rand = Random.Range((int)NumberType.N2, (int)NumberType.N64 - 3);
        type = (NumberType)rand;
        var color = new Color();

        switch (rand)
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
