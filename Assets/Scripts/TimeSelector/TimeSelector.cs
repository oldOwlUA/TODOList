using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections;

public class TimeSelector : MonoBehaviour
{
    [SerializeField] private ScrollRect _hoursScroll;
    [SerializeField] private RectTransform _hoursScrollContent;
    [SerializeField] private TextMeshProUGUI _hoursText;
    [SerializeField] private List<RectTransform> _hoursList;
    [SerializeField] private ScrollRect _minutesScroll;
    [SerializeField] private RectTransform _minutesScrollContent;
    [SerializeField] private TextMeshProUGUI _minutesText;
    [SerializeField] private List<RectTransform> _minutesList;
    [SerializeField] private float _norm = 0;
    [SerializeField] private float targetNormalizedPos = 0;
    [SerializeField] private RectTransform closestItem;
    [SerializeField] private float _verticalOffset = 0.0025f;
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private float minVelocity = 500;


    private void Awake()
    {
        InitHoursScroll();
        InitMinutesScroll();
    }

    [Button]
    public void InitHoursScroll()
    {
        for (int i = 0; i < 24; i++)
        {
            var text = Instantiate(_hoursText);
            text.text = i.ToString("00");
            text.gameObject.name = i.ToString() + "_h";
            text.transform.SetParent(_hoursScrollContent, false);
            _hoursList.Add(text.GetComponent<RectTransform>());
        }
    }


    [Button]
    public void InitMinutesScroll()
    {
        for (int i = 0; i < 60; i++)
        {
            var text = Instantiate(_minutesText);
            text.text = (i + 1).ToString("00");
            text.gameObject.name = i.ToString() + "_m";
            text.transform.SetParent(_minutesScrollContent, false);
            _minutesList.Add(text.GetComponent<RectTransform>());
        }
    }


    private void Update()
    {
        _norm = _hoursScroll.verticalNormalizedPosition;
        closestItem = _hoursList
           .OrderBy(item => Mathf.Abs(item.position.y - _hoursScroll.viewport.position.y))
           .First();

        if (Input.GetMouseButtonUp(0))
        {
            SnapToClosestItem();
        }
    }

    private void SnapToClosestItem()
    {
        StartCoroutine(SmoothScrollTo());
    }

    private float GetNormalizedPosition(RectTransform target)
    {
        float contentHeight = _hoursScrollContent.rect.height;
        float viewportHeight = _hoursScroll.viewport.rect.height;

        // Позиція елемента відносно батьківського контейнера
        float itemCenterY = -target.localPosition.y; // Локальна позиція у ScrollRect
        float contentBottom = -_hoursScrollContent.rect.yMax; // Нижня межа контенту

        // Обчислення нормалізованої позиції
        float normalizedPos = Mathf.Clamp01((itemCenterY - contentBottom - (viewportHeight / 2)) / (contentHeight - viewportHeight));
        return 1 - normalizedPos; // Інвертуємо, бо 1 = верх, 0 = низ
    }

    private IEnumerator SmoothScrollTo()
    {
        while (_hoursScroll.velocity.y > minVelocity)
        {
            yield return null;
        }

        targetNormalizedPos = GetNormalizedPosition(closestItem) + _verticalOffset;
        // Час анімації
        float elapsedTime = 0;
        float startValue = _hoursScroll.verticalNormalizedPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _hoursScroll.verticalNormalizedPosition = Mathf.Lerp(startValue, targetNormalizedPos, elapsedTime / duration);
            yield return null;
        }

        _hoursScroll.verticalNormalizedPosition = targetNormalizedPos;
    }
}
