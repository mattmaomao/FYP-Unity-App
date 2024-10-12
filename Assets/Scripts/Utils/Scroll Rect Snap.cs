using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap : MonoBehaviour
{
    // scroll inputs
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform itemContainer;
    [SerializeField] RectTransform scrollItem_Temp;
    [SerializeField] VerticalLayoutGroup VLG;
    [SerializeField] int selectedItemIdx;
    [SerializeField] float snapForce = 100;
    float snapSpeed;

    [SerializeField] List<string> contents;

    // Update is called once per frame
    void Update()
    {
        // scroll selection
        float slotHeight = scrollItem_Temp.rect.height + VLG.spacing;
        selectedItemIdx = Mathf.RoundToInt(itemContainer.localPosition.y / slotHeight - 0.5f);
        selectedItemIdx = Mathf.Clamp(selectedItemIdx, 0, 5);

        if (scrollRect.velocity.magnitude < 150)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;
            itemContainer.localPosition = new Vector3(
                itemContainer.localPosition.x,
                Mathf.MoveTowards(itemContainer.localPosition.y, (selectedItemIdx + 0.5f) * slotHeight, snapSpeed),
                itemContainer.localPosition.z
            );
            if (itemContainer.localPosition.y == (selectedItemIdx + 0.5f) * slotHeight)
                snapSpeed = 0;
        }
    }

    public string getItem() {
        return contents[selectedItemIdx];
    }
}
