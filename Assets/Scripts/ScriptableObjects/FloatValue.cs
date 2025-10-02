using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class FloatValue : ScriptableObject, ISerializationCallbackReceiver
{
    public float initiaValue;

    [HideInInspector] 
    public float RuntimeValue;


    public void OnAfterDeserialize()
    {
        RuntimeValue = initiaValue;
    }


    public void OnBeforeSerialize()
    {

    }
    public void SetValue(float value)
    {
        RuntimeValue = value;
    }
}
