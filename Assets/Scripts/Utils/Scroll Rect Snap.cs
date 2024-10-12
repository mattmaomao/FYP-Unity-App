using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectSnap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // scroll inputs
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform itemContainer;
    [SerializeField] RectTransform scrollItem_Temp;
    [SerializeField] VerticalLayoutGroup VLG;
    [SerializeField] RectTransform normalViewPort;
    [SerializeField] RectTransform scrollingViewPort;

    [SerializeField] int selectedItemIdx;
    int maxItem;

    [SerializeField] float snapForce = 100;
    float slotHeight;
    float snapSpeed;

    bool overrideSnapping;
    bool isDragging;

    [SerializeField] List<string> contents;

    void Start()
    {
        slotHeight = scrollItem_Temp.rect.height + VLG.spacing;
        maxItem = contents.Count;
    }

    void Update()
    {
        // scroll selection
        if (!overrideSnapping)
        {
            selectedItemIdx = Mathf.RoundToInt(itemContainer.localPosition.y / slotHeight - 0.5f);
            selectedItemIdx = Mathf.Clamp(selectedItemIdx, 0, maxItem-1);
        }

        if (!isDragging || overrideSnapping)
        {
            // show selected item
            scrollRect.viewport = normalViewPort;
            itemContainer.transform.SetParent(normalViewPort);

            // snap to selection
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
                {
                    snapSpeed = 0;
                    overrideSnapping = false;
                }
            }
        }
        // show more item when scrolling
        else
        {
            scrollRect.viewport = scrollingViewPort;
            itemContainer.transform.SetParent(scrollingViewPort);
        }
    }

    // force snap to index
    public void initSnap(int i)
    {
        selectedItemIdx = i;
        overrideSnapping = true;
    }
    public void initSnap(string str)
    {
        selectedItemIdx = contents.IndexOf(str);
        overrideSnapping = true;
    }

    // return selected item
    public string getItem()
    {
        return contents[selectedItemIdx];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
