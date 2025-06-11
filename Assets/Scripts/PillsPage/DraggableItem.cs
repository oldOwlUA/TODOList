using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static event Action<DraggableItem, int> PositionChanged;

    private Transform _originalParent;
    private RectTransform _rectTransform;
    private GameObject _placeholder;
    private Vector2 _dragOffset;

    private int _newIndex;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _dragOffset
        );

        if (_placeholder == null)
        {
            _placeholder = new GameObject("Placeholder");
            RectTransform placeholderRect = _placeholder.AddComponent<RectTransform>();
            placeholderRect.sizeDelta = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
        }

        _placeholder.gameObject.SetActive(true);
        _placeholder.transform.SetParent(_originalParent);
        _placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
        transform.SetParent(_originalParent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _originalParent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);


        _rectTransform.anchoredPosition = localPoint - _dragOffset;


        _newIndex = _originalParent.childCount-1;

        for (int i = 0; i < _originalParent.childCount; i++)
        {
            Transform child = _originalParent.GetChild(i);
            if (child == _placeholder) continue;

            if (_rectTransform.position.y > child.position.y)
            {
                _newIndex = i;

                if (_placeholder.transform.GetSiblingIndex() < _newIndex)
                    _newIndex--;

                break;
            }
        }

        _placeholder.transform.SetSiblingIndex(_newIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_originalParent);
        transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());        
        _placeholder.SetActive(false);
        _placeholder.transform.SetParent(transform);
        PositionChanged?.Invoke(this, _newIndex);

    }
}
