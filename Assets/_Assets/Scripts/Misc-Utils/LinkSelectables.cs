using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkSelectables : MonoBehaviour
{
    [Header("This assumes children start top left and go down (and are completely filled, for now)")]
    [SerializeField] private bool startVertical;
    [Space(10)]
    [SerializeField] private int numRows;
    [SerializeField] private int numCols;
    [Space(10)]
    [SerializeField] private bool LinkOnValidate;
    [SerializeField] private List<Selectable> selectables;


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (LinkOnValidate)
        {
            Link();
        }
    }
#endif

    public void Link()
    {
        selectables = new List<Selectable>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childObj = transform.GetChild(i);
            if (childObj.gameObject.activeSelf)
                selectables.Add(childObj.GetComponent<Selectable>());
        }

        if (startVertical)
            numCols = Mathf.CeilToInt((float)selectables.Count / numRows);
        else
            numRows = Mathf.CeilToInt((float)selectables.Count / numCols);

        for (int i = 0; i < selectables.Count; i++)
        {
            Selectable currSelectable = selectables[i];
            int x;
            int y;
            if (startVertical)
            {
                x = i / numRows;
                y = i % numRows;
            }
            else
            {
                x = i % numCols;
                y = i / numCols;
            }

            Selectable up = GetSelectableAtPosition(x, y - 1, false, currSelectable);
            Selectable down = GetSelectableAtPosition(x, y + 1, false, currSelectable);
            Selectable left = GetSelectableAtPosition(x - 1, y, true, currSelectable);
            Selectable right = GetSelectableAtPosition(x + 1, y, true, currSelectable);

            Navigation customNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = up,
                selectOnDown = down,
                selectOnLeft = left,
                selectOnRight = right
            };

            currSelectable.navigation = customNav;
        }

        Selectable GetSelectableAtPosition(int _x, int _y, bool _isHorizontal, Selectable _currSelectable)
        {
            _x = (_x + numCols) % numCols;
            _y = (_y + numRows) % numRows;
            int newIndex;
            if (startVertical)
                newIndex = _x * numRows + _y;
            else
                newIndex = _x + _y * numCols;

            while (newIndex >= selectables.Count)
            {
                //Invalid
                if (startVertical)
                {
                    if (_isHorizontal)
                        newIndex -= numRows;
                    else
                        newIndex -= 1;
                }
                else
                {
                    if (_isHorizontal)
                        newIndex -= 1;
                    else
                        newIndex -= numCols;
                }
            }

            Selectable toReturn = selectables[newIndex];
            if (toReturn != _currSelectable)
                return toReturn;

            return null;
        }
    }
}
