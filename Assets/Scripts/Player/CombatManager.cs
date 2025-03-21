using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Quản lý kỹ năng")]
    public SkillManager skillManager;

    [Header("Quản lý Mana")]
    public StaminaWheel manaManager;

    [Header("Vị trí Spawn hiệu ứng vận chiêu")]
    public Transform skillEffectPoint;

    private Animator animator;
    private GameObject currentChargingEffect; 
    private Skill currentSkill; 

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
                        animator.SetTrigger("attack2");
                        StartCoroutine(ActivateSkillWithDelay(skill));
                    }
                    else
                    {
                        StartCoroutine(StartSpellCast(skill));
                    }
                }
            }           
        }
    }

    IEnumerator ActivateSkillWithDelay(Skill skill)
    {
        yield return new WaitForSeconds(0.2f);
        skill.ActivateSkill(gameObject);
        yield return null;
    }

    IEnumerator StartSpellCast(Skill skill)
    {
        animator.Play("Start_Spell");
        yield return new WaitForSeconds(0.5f);
        StartChargingEffect(skill);

        yield return new WaitUntil(() => Input.GetKeyUp(skill.skillKey));

        animator.Play("End_Spell");
        skill.ActivateSkill(gameObject);
        StopChargingEffect();
        yield return new WaitForSeconds(0.5f);
        animator.Play("Idle");
    }


    void StartChargingEffect(Skill skill)
    {
        if (skill.chargingEffectPrefab != null && currentChargingEffect == null)
        {
            // Lấy hướng Player đang đối mặt
            Vector2 direction = GetComponent<PlayerMovement>().GetFacingDirection();

            // Lấy vị trí offset từ Skill
            Vector2 offset = skill.GetEffectOffset(direction);

            // Cập nhật vị trí của skillEffectPoint theo hướng Player + offset
            skillEffectPoint.localPosition = offset;

            // Tạo hiệu ứng tại vị trí đã điều chỉnh
            currentChargingEffect = Instantiate(skill.chargingEffectPrefab, skillEffectPoint.position, Quaternion.identity, skillEffectPoint);
        }
    }



    void StopChargingEffect()
    {
        if (currentChargingEffect != null)
        {
            Destroy(currentChargingEffect);
            currentChargingEffect = null;
        }
    }
}
