using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Quản lý kỹ năng")]
    public SkillManager skillManager;


    void Update()
    {
        if (Input.GetKeyDown(skillManager.skills[0].skillKey))
        {
            skillManager.skills[0].ActivateSkill(gameObject);
        }
    }



}
