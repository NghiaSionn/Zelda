using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Quản lý kỹ năng")]
    public SkillManager skillManager;

    [Header("Quản lý Mana")]
    public StaminaWheel manaManager;

    void Update()
    {
        if (Input.GetKeyDown(skillManager.skills[0].skillKey))
        {
            Skill skill = skillManager.skills[0];

            // Kiểm tra đủ mana trước khi sử dụng kỹ năng
            if (manaManager.UseMana(skill.manaCost))
            {
                skill.ActivateSkill(gameObject);
            }
        }
    }
}

