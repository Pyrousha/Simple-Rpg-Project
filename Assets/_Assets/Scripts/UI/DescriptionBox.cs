using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DescriptionBox : Singleton<DescriptionBox>
{
    [SerializeField] private Animator anim;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Space(10)]
    [SerializeField] private Sprite transparentSprite;

    [System.Serializable]
    public class DescriptionInfo
    {
        public DescriptionInfo(Sprite icon)
        {
            Icon = icon;
            Name = "name";
            CostText = "mPCostText";
            DescriptionText = "descriptionText";
        }

        public DescriptionInfo(Sprite icon, string name, string mPCostText, string descriptionText)
        {
            Icon = icon;
            Name = name;
            CostText = mPCostText;
            DescriptionText = descriptionText;
        }

        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string CostText { get; private set; }
        [field: SerializeField] public string DescriptionText { get; private set; }
    }

    private void Awake()
    {
        SetUI(new DescriptionInfo(transparentSprite));
    }

    public void SetStatus(bool _enabled)
    {
        anim.SetBool("Status", _enabled);
    }

    public void SetUI(DescriptionInfo _info)
    {
        icon.sprite = _info.Icon;
        nameText.text = _info.Name;
        costText.text = _info.CostText;
        descriptionText.text = _info.DescriptionText;

        SetStatus(true);
    }
}
