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

    private AudioSource chargingAudioSource;
    private Animator animator;
    private GameObject currentChargingEffect;
    private Skill currentSkill;
    private PlayerMovement playerMovement;

    private Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();

    private void Start()
    {
        animator = GetComponent<Animator>();
        chargingAudioSource = GetComponentInChildren<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        List<Skill> keys = new List<Skill>(skillCooldowns.Keys);
        foreach (Skill skill in keys)
        {
            if (skillCooldowns[skill] > 0)
            {
                skillCooldowns[skill] -= Time.deltaTime;
                if (skillCooldowns[skill] <= 0)
                {
                    skillCooldowns[skill] = 0;
                }
            }
        }

        for (int i = 0; i < skillManager.skills.Length; i++)
        {
            Skill skill = skillManager.skills[i];

            if (Input.GetKeyDown(skill.skillKey))
            {
                if (skillCooldowns.ContainsKey(skill) && skillCooldowns[skill] > 0)
                {
                    Debug.Log($"{skill.skillName} đang hồi chiêu! {skillCooldowns[skill]:F1}s còn lại.");
                    continue;
                }

                if (manaManager.CanStartSkill(skill.manaCost))
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
        // Trừ mana trước khi kích hoạt kỹ năng
        if (manaManager.ConsumeMana(skill.manaCost))
        {
            // Đợi một frame để đảm bảo UI mana được cập nhật
            yield return null;
            skill.ActivateSkill(gameObject, skill.maxScale); // Sử dụng maxScale
            ApplyCooldown(skill);
        }
        else
        {
            Debug.Log("Không đủ mana để kích hoạt kỹ năng!");
        }
    }

    IEnumerator StartSpellCast(Skill skill)
    {
        playerMovement.SetCasting(true);

        animator.Play("Start_Spell");
        yield return new WaitForSeconds(0.5f);
        currentSkill = skill;
        StartChargingEffect(skill);

        if (currentSkill.soundEffect != null)
        {
            chargingAudioSource.clip = currentSkill.soundEffect;
            chargingAudioSource.Play();
        }

        float manaCost = skill.manaCost;
        float manaConsumed = 0f;
        float manaPerSecond = manaCost / skill.castTime;
        float holdTime = 0f; // Theo dõi thời gian giữ phím

        // Kiểm tra xem người chơi có giữ phím đủ lâu không
        float minHoldTime = 0.1f; // Ngưỡng thời gian tối thiểu để coi là "giữ phím" (0.1 giây)
        while (Input.GetKey(skill.skillKey) && holdTime < minHoldTime)
        {
            holdTime += Time.deltaTime;
            yield return null;
        }

        // Nếu không giữ phím đủ lâu (nhấn nhanh), không làm gì cả
        if (!Input.GetKey(skill.skillKey) && holdTime < minHoldTime)
        {
            Debug.Log($"{skill.skillName} không được kích hoạt vì không giữ phím đủ lâu!");
        }
        else
        {
            // Nếu giữ phím đủ lâu, bắt đầu vận chiêu
            while (Input.GetKey(skill.skillKey))
            {
                float manaToConsume = manaPerSecond * Time.deltaTime;
                if (manaConsumed + manaToConsume > manaCost)
                {
                    manaToConsume = manaCost - manaConsumed;
                }

                if (manaManager.ConsumeMana(manaToConsume))
                {
                    manaConsumed += manaToConsume;
                    holdTime += Time.deltaTime; // Tăng thời gian giữ phím
                }
                else
                {
                    Debug.Log("Không đủ mana để tiếp tục vận chiêu!");
                    break;
                }

                yield return null;
            }

            // Tính scale dựa trên thời gian giữ phím
            float scale = Mathf.Lerp(skill.minScale, skill.maxScale, holdTime / skill.castTime);
            scale = Mathf.Clamp(scale, skill.minScale, skill.maxScale);

            // Kích hoạt kỹ năng (cả khi thả sớm hoặc đủ mana)
            Debug.Log($"{skill.skillName} được kích hoạt với scale: {scale}");
            skill.ActivateSkill(gameObject, scale);
            ApplyCooldown(skill);
        }

        animator.Play("End_Spell");
        chargingAudioSource.Stop();
        StopChargingEffect();
        yield return new WaitForSeconds(0.5f);
        animator.Play("Idle");

        playerMovement.SetCasting(false);
    }

    void ApplyCooldown(Skill skill)
    {
        if (!skillCooldowns.ContainsKey(skill))
        {
            skillCooldowns.Add(skill, skill.cooldown);
        }
        else
        {
            skillCooldowns[skill] = skill.cooldown;
        }
        Debug.Log($"{skill.skillName} bắt đầu hồi chiêu {skill.cooldown} giây.");
    }

    void StartChargingEffect(Skill skill)
    {
        if (skill.chargingEffectPrefab != null && currentChargingEffect == null)
        {
            Vector2 direction = GetComponent<PlayerMovement>().GetFacingDirection();
            Vector2 offset = skill.GetEffectOffset(direction);
            skillEffectPoint.localPosition = offset;

            currentChargingEffect = Instantiate(skill.chargingEffectPrefab, skillEffectPoint.position, Quaternion.identity, skillEffectPoint);

            float angle = 0f;
            if (direction == Vector2.up) angle = skill.up;
            else if (direction == Vector2.down) angle = skill.down;
            else if (direction == Vector2.left) angle = skill.left;
            else if (direction == Vector2.right) angle = skill.right;

            currentChargingEffect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void StopChargingEffect()
    {
        if (currentChargingEffect != null)
        {
            Destroy(currentChargingEffect);
            currentChargingEffect = null;
            currentSkill = null;
        }
    }
}