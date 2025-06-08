using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pills : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fullList;
    [SerializeField] private Image _targetImage;
    [SerializeField] private Color _undone = Color.red;
    [SerializeField] private Color _done = Color.green;
    [SerializeField] private Button _doneButton;

    [SerializeField] private List<DayRecord> _records = new List<DayRecord>();

    private string ListKey = "PillsList";

    private void OnEnable()
    {
        _doneButton.onClick.AddListener(AddNewRecord);
        UpdateList();
    }

    private void OnDisable()
    {
        _doneButton.onClick.RemoveAllListeners();
    }

    private void AddNewRecord()
    {
        if (_records == null)
            _records = new List<DayRecord>();

        _records.Add(new DayRecord()
        {
            _day = DateTime.Now,
            _done = true
        });

        Debug.Log("--");
        Save();
        UpdateList();
    }

    private void UpdateList()
    {
        var records = PlayerPrefs.GetString(ListKey);
        _records = JsonConvert.DeserializeObject<List<DayRecord>>(records);
        StringBuilder sb = new StringBuilder();
        if (_records == null)
        {
            _targetImage.color = _undone;
            _fullList.text = "Empty";
            return;
        }

        foreach (DayRecord record in _records)
        {
            sb.AppendLine(record.ToString());
        }

        if (_records != null && _records.Count >= 1)
        {
            var last = _records[_records.Count - 1];
            _targetImage.color = last._day < DateTime.Now.Date ? _undone : _done;
        }

        sb.Clear();
        foreach (DayRecord record in _records)
        {
            sb.AppendLine(record.ToString());
        }
        _fullList.text = sb.ToString();
    }

    private void Save()
    {
        var s = JsonConvert.SerializeObject(_records);
        PlayerPrefs.SetString(ListKey, s);
        PlayerPrefs.Save();
    }
}

[Serializable]
public class DayRecord
{
    public DateTime _day;
    public bool _done;

    public override string ToString()
    {
        return $"{_day.Date} - {_done}";
    }
}
