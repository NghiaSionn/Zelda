using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu]
public class VectorValue : ScriptableObject, ISerializationCallbackReceiver
{
    public Vector2 initialValue;
    public Vector2 defaultvalue;


    public void OnAfterDeserialize()
    {
        initialValue = defaultvalue;
    }


    public void OnBeforeSerialize()
    {

    }

}
