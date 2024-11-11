using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class WorldTimeDisplay : MonoBehaviour
{
    [SerializeField] 
    private WorldTime _worldTime;
    private TMP_Text _text;

    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _worldTime.WorldTimeChange += OnWorldTimeChange;
    }

    private void OnDestroy()
    {
        _worldTime.WorldTimeChange -= OnWorldTimeChange;
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        _text.SetText(newTime.ToString(@"hh\:mm"));
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
