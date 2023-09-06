using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : UIButton
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemQuantity;
}
