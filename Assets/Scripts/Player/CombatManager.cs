using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Quản lý kỹ năng")]
    public SkillManager skillManager;

    [Header("Quản lý Mana")]
    public StaminaWheel manaManager;

    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        for (int i = 0; i < skillManager.skills.Length; i++)
        {
            Skill skill = skillManager.skills[i];

            if (Input.GetKeyDown(skill.skillKey))
            {
                if (manaManager.UseMana(skill.manaCost))
                {
                    if (skill.useAnimatorWithWeapon)
                    {                      
                        GetComponent<Animator>().SetTrigger("attack2");
                        StartCoroutine(ActivateSkillWithDelay(skill));
                    }
                    else
                    {
                        StartCoroutine(StartSpellCast(skill));
                        //skill.ActivateSkill(gameObject);
                    }
                }
            }
        }
    }

    IEnumerator ActivateSkillWithDelay(Skill skill)
    {
        yield return new WaitForSeconds(0.2f);
        // Sau khi animation kết thúc, tung skill
        skill.ActivateSkill(gameObject);
        yield return null;
    }

    IEnumerator StartSpellCast(Skill skill)
    {
        GetComponent<Animator>().SetBool("startspell", true);
        yield return null;
    }

}

