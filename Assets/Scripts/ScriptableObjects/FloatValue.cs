using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class FloatValue : ScriptableObject, ISerializationCallbackReceiver
{
    public float initiaValue;
    public float RuntimeValue;


    public void OnAfterDeserialize()
    {
        RuntimeValue = initiaValue;
    }


    public void OnBeforeSerialize()
    {

    }   

     // Trả về giá trị hiện tại
    public float GetValue()
    {
        return RuntimeValue;
    }

    // Đặt giá trị từ dữ liệu đã lưu
    public void SetValue(float value)
    {
        RuntimeValue = value;
    }
}
