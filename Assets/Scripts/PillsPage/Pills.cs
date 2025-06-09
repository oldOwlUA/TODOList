using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class Pills : MonoBehaviour
{
    [SerializeField] private Record _recordObject;
    [SerializeField] private Transform _instantiateTarget;

    [SerializeField] private TextMeshProUGUI _emptyLabel;
    [SerializeField] private Image _targetImage;
    [SerializeField] private Color _undone = Color.red;
    [SerializeField] private Color _done = Color.green;
    [SerializeField] private Button _newButton;
    [SerializeField] private Button _saveButton;

    [SerializeField] private List<DayRecord> _records = new List<DayRecord>();

    [SerializeField] private List<Record> _instantiatedRecords = new List<Record>();

    private string ListKey = "PillsList";

    private void OnEnable()
    {
        _newButton.onClick.AddListener(OnAddClick);
        _saveButton.onClick.AddListener(OnSaveClick);
        UpdateList();
    }

    private void OnDisable()
    {
        _newButton.onClick.RemoveAllListeners();
        _saveButton.onClick.RemoveAllListeners();
    }

    private void OnSaveClick()
    {
        Save();
        UpdateList();
    }

    private void OnAddClick()
    {
        DayRecord data = GetNewRecord();

        Record newRecord = Instantiate(_recordObject, _instantiateTarget);
        newRecord.Init(data);
        _instantiatedRecords.Add(newRecord);

        Save();
        UpdateList();
    }

    private DayRecord GetNewRecord()
    {
      return  new DayRecord()
        {
            Id = new Guid(),
            Day = DateTime.Now.Date,
            Done = true
        };
    }

    private void UpdateList()
    {
        var records = PlayerPrefs.GetString(ListKey);
        Debug.Log(records);

        string output = Regex.Replace(records, @"_(\p{L})", m => m.Groups[1].Value.ToUpper());

        _records = JsonConvert.DeserializeObject<List<DayRecord>>(output);

        if (_records == null)
        {
            _targetImage.color = _undone;
            _emptyLabel.text = "Empty";
            return;
        }

        _emptyLabel.text = "";

        ClearScroll();
        UpdateScroll();
        UpdateCurrentState();
    }

    private void UpdateCurrentState()
    {
        if (_records != null && _records.Count >= 1)
        {
            var last = _records[_records.Count - 1];
            _targetImage.color = last.Day < DateTime.Now.Date ? _undone : _done;
        }
    }

    private void UpdateScroll()
    {
        for (int i = 0; i < _records.Count; i++)
        {
            Record newRecord = Instantiate(_recordObject, _instantiateTarget);
            newRecord.Init(_records[i]);
            _instantiatedRecords.Add(newRecord);
        }
    }

    private void ClearScroll()
    {
        for (int i = 0; i < _instantiatedRecords.Count; i++)
        {
            Destroy(_instantiatedRecords[i].gameObject);
        }
        _instantiatedRecords.Clear();
    }

    private void Save()
    {
        _records.Clear();

        if (_records == null) _records = new List<DayRecord>();

        for (int i = 0; i < _instantiatedRecords.Count; i++)
        {
            _records.Add(_instantiatedRecords[i].GetData());
        }

        var s = JsonConvert.SerializeObject(_records);
        Debug.Log(s);
        PlayerPrefs.SetString(ListKey, s);
        PlayerPrefs.Save();
    }
}

[Serializable]
public class DayRecord
{
    public Guid Id;
    public DateTime Day;
    public bool Done;

    public override string ToString()
    {
        return Day.Date.ToString("dd-MM-yyyy");
    }
}
