using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Record : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Toggle _stateToggle;
    [SerializeField] private Button _deleteButton;

    private DayRecord _data;

    public Guid Id => _data != null ? _data.Id : Guid.Empty;

    public event Action<Guid> OnDelete;

    private void Awake()
    {
        _stateToggle.onValueChanged.AddListener(SetToggleState);
        _inputField.onValueChanged.AddListener(SetNewDate);
        _inputField.onEndEdit.AddListener(SetNewDate);
        _deleteButton.onClick.AddListener(OnDeleteClick);
    }

    private void OnDeleteClick()
    {
        OnDelete?.Invoke(_data.Id);
    }

    private void OnDestroy()
    {
        _stateToggle.onValueChanged.RemoveAllListeners();
        _inputField.onValueChanged.RemoveAllListeners();
        _inputField.onEndEdit.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();
    }

    private void SetNewDate(string dateString)
    {
        if (DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            _data.Day = date;
        }
        else
        {
            Debug.LogWarning("Wrong date format");
        }
    }

    private void SetToggleState(bool state)
    {
        _data.Done = state;
    }

    public void Init(DayRecord record)
    {
        if (record == null)
        {
            Debug.LogWarning("record is null");
        }
        _inputField.SetTextWithoutNotify(record.ToString());
        _stateToggle.SetIsOnWithoutNotify(record.Done);
        _data = record;
    }

    public DayRecord GetData()
    {
        return _data;
    }

    internal void Reset()
    {
        throw new NotImplementedException();
    }
}
