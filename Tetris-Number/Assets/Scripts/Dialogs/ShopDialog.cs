using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuyType
{
    OneHammer,
    ThreeHammer,
    OneColourHammer,
    ThreeColourHammer
}

public class ShopDialog : DialogBase
{
    [SerializeField] Button[] buttons;

    private void Start()
    {
        buttons[0].onClick.AddListener(() => GameplayController.Instance.Buy(BuyType.OneHammer));
        buttons[1].onClick.AddListener(() => GameplayController.Instance.Buy(BuyType.ThreeHammer));
        buttons[2].onClick.AddListener(() => GameplayController.Instance.Buy(BuyType.OneColourHammer));
        buttons[3].onClick.AddListener(() => GameplayController.Instance.Buy(BuyType.ThreeColourHammer));
    }

    protected override void HideBehaviour()
    {
    }

    protected override void ShowBehaviour()
    {
    }
}
