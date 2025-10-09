using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCardHandler : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
{
    public Canvas canvas;
    public Transform cardContainer;

    private RectTransform rectTrans;
    private CanvasGroup cg;
    private WallpaperCardProperties cardProperties;

    private int oriCardIdx;

    private void Awake() {
        rectTrans = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        cardProperties = GetComponent<WallpaperCardProperties>();

        // Failsafe
        if(canvas == null) canvas = FindObjectOfType<Canvas>();
        if(cardContainer == null) cardContainer = transform.parent;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        oriCardIdx = transform.GetSiblingIndex();
        transform.SetParent(canvas.transform, true); // Temp place the card as canvas's child
        cg.blocksRaycasts = false;
        cg.alpha = 0.8f;

        // throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.pointerId >= 0){
            Vector2 delta = eventData.delta / canvas.scaleFactor;
            rectTrans.anchoredPosition += delta;
        }
        // rectTrans.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(cardContainer, true); // Return the dragged card as the Card Container's child
        cg.blocksRaycasts = true;
        cg.alpha = 1f;
        
        for(int i = 0; i < cardContainer.childCount - 1; i++){ // -1 to exclude the Add Card Button (cause its set to always be the last index/child of Card Container)
            Transform child = cardContainer.GetChild(i);

            if(child == transform) continue; // skip self check

            RectTransform childRect = child.GetComponent<RectTransform>();

            if(RectTransformUtility.RectangleContainsScreenPoint(childRect, Input.mousePosition, canvas.worldCamera)){
                // Swap the index
                int targetIdx = child.GetSiblingIndex();
                transform.SetSiblingIndex(targetIdx);
                AdjustAddCardBtn();

                // Save the new Sorted Cards Position
                cardProperties.manager.SaveSortedCard();
                // Debug.Log("EndDrag, Changed");
                return;
            }
        }

        // Return to original pos if invalid
        transform.SetSiblingIndex(oriCardIdx);
        AdjustAddCardBtn();

        // throw new System.NotImplementedException();
    }

    private void AdjustAddCardBtn(){
        Transform addCardBtn = cardContainer.GetChild(cardContainer.childCount - 1);
        addCardBtn.SetSiblingIndex(cardContainer.childCount - 1);
    }

}
