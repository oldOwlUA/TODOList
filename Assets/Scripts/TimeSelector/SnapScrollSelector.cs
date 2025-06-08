using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class SnapScrollSelector : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public event Action<int> OnNumberSelected;
    public int SelectedPanelIndex => _panelsList.IndexOf(_closestItem);

    [SerializeField] private ScrollRect _elementsScroll;
    [SerializeField] private RectTransform _elementsScrollContent;
    [SerializeField] private TextMeshProUGUI _elementsPrefab;
    [Range(1, 60)]
    [SerializeField] private int _panelCount = 1;

    [SerializeField] private float _verticalOffset = 0.0025f;
    [SerializeField] private float _animationDuration = 0.1f;
    [SerializeField] private float _minVelocity = 500;

    private RectTransform _closestItem;
    private List<RectTransform> _panelsList = new List<RectTransform>();
    private bool _isInited = false;

    private void Awake()
    {
        InitPanels();
    }

    [Button]
    public void InitPanels()
    {
        for (int i = 0; i < _panelCount; i++)
        {
            var text = Instantiate(_elementsPrefab);
            text.text = i.ToString("00");
            text.gameObject.name = i.ToString() + "_h";
            text.transform.SetParent(_elementsScrollContent, false);
            _panelsList.Add(text.rectTransform);
        }
    }

    private void Update()
    {
        if (_isInited)
        {
            return;
        }

        _closestItem = _panelsList
           .OrderBy(item => Mathf.Abs(item.position.y - _elementsScroll.viewport.position.y))
           .First();

        //if (Input.GetMouseButtonUp(0))
        //{
        //    SnapToClosestItem();
        //}
    }

    private void SnapToClosestItem()
    {
        StartCoroutine(SmoothScrollTo());
    }

    private float GetNormalizedPosition(RectTransform target)
    {
        float contentHeight = _elementsScrollContent.rect.height;
        float viewportHeight = _elementsScroll.viewport.rect.height;

        float itemCenterY = -target.localPosition.y;
        float contentBottom = -_elementsScrollContent.rect.yMax;


        float normalizedPos = Mathf.Clamp01((itemCenterY - contentBottom - (viewportHeight / 2)) / (contentHeight - viewportHeight));
        return 1 - normalizedPos;
    }

    private IEnumerator SmoothScrollTo()
    {
        while (Mathf.Abs(_elementsScroll.velocity.y) > _minVelocity)
        {
            yield return null;
        }

        float targetNormalizedPos = GetNormalizedPosition(_closestItem) + _verticalOffset;
        float elapsedTime = 0;
        float startValue = _elementsScroll.verticalNormalizedPosition;

        while (elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;
            _elementsScroll.verticalNormalizedPosition = Mathf.Lerp(startValue, targetNormalizedPos, elapsedTime / _animationDuration);
            yield return null;
        }
        _elementsScroll.verticalNormalizedPosition = targetNormalizedPos;

        OnNumberSelected?.Invoke(_panelsList.IndexOf(_closestItem));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isInited)
        {
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            SnapToClosestItem();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       // throw new NotImplementedException();
    }
}
