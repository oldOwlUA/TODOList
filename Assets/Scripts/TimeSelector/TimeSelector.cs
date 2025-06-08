using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSelector : MonoBehaviour
{
    public event Action OnTimeUpdated;

    [SerializeField] private SnapScrollSelector _hoursSelector;
    [SerializeField] private SnapScrollSelector _minutesSelector;


}
