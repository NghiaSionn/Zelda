using UnityEngine;

[CreateAssetMenu]
public class Skill : ScriptableObject
{
    [Header("Tên kỹ năng")]
    public string skillName;

    [Header("Sử dụng Animator trước khi tung skill nhưng cầm vũ khí")]
    public bool useAnimatorWithWeapon;

    [Header("Nút kỹ năng")]
    public KeyCode skillKey;

    [Header("Thể loại kỹ năng")]
    public SkillType skillType;

    [Header("Dạng kỹ năng")]
    public SkillForm skillForm;

    [Header("Thời gian hồi")]
    public float cooldown;

    [Header("Thời gian vận chiêu")]
    public float castTime = 2f;

    [Header("Năng lượng")]
    public int manaCost;

    [Header("Thời gian tồn tại của kỹ năng")]
    public float duration = 5f;

    [Header("Khoảng cách (dùng cho StraightLine)")]
    public float khoangCach = 1f;

    [Header("Kỹ năng")]
    public GameObject skillPrefab;

    [Header("Hiệu ứng vận chiêu (xử lý trong CombatManager)")]
    public GameObject chargingEffectPrefab;

    [Header("Hiệu ứng va chạm (dùng EffectPool)")]
    public GameObject impactEffectPrefab;

    [Header("Scale của kỹ năng")]
    public float minScale = 0.5f; // Scale tối thiểu
    public float maxScale = 1f;   // Scale tối đa

    private Animator skillAnim;

    [Header("Âm Thanh gồng chiêu")]
    public AudioClip soundEffect;

    [Header("Hiệu ứng vận chiêu - Tùy chỉnh vị trí theo hướng")]
    public Vector2 effectOffsetUp = new Vector2(0f, 1.2f);
    public Vector2 effectOffsetDown = new Vector2(0f, -1f);
    public Vector2 effectOffsetLeft = new Vector2(-1.2f, 0f);
    public Vector2 effectOffsetRight = new Vector2(1.2f, 0f);

    [Header("Skill - Tùy chỉnh góc xoay theo hướng")]
    public float up = -180f;
    public float down = 0f;
    public float right = 90f;
    public float left = -90f;

    // Thêm tham số scale để điều chỉnh kích thước kỹ năng
    public void ActivateSkill(GameObject user, float scale = 1f)
    {
        Vector3 spawnPosition = Vector3.zero;
        Vector2 direction = Vector2.zero;
        float angle = 0f;

        switch (skillForm)
        {
            case SkillForm.StraightLine:
                direction = user.GetComponent<PlayerMovement>().GetFacingDirection();
                spawnPosition = user.transform.position + (Vector3)(direction * khoangCach);

                if (direction == Vector2.up) angle = up;
                else if (direction == Vector2.down) angle = down;
                else if (direction == Vector2.left) angle = left;
                else if (direction == Vector2.right) angle = right;
                GetEffectOffset(direction);
                break;

            case SkillForm.Point:
                Vector3 mousePosition = Input.mousePosition;
                spawnPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                spawnPosition.z = 0f;

                direction = (spawnPosition - user.transform.position).normalized;
                if (direction == Vector2.up) angle = up;
                else if (direction == Vector2.down) angle = down;
                else if (direction == Vector2.left) angle = left;
                else if (direction == Vector2.right) angle = right;


                GetEffectOffset(direction);
                break;

            case SkillForm.Support:
                spawnPosition = user.transform.position;
                direction = Vector2.zero;
                angle = 0f;
                break;

            case SkillForm.Another:
                spawnPosition = user.transform.position;
                direction = Vector2.zero;
                angle = 0f;
                break;
        }

        if (skillPrefab != null)
        {
            skillAnim = skillPrefab.GetComponent<Animator>();
            GameObject projectile = Instantiate(skillPrefab, spawnPosition, Quaternion.identity);

            // Áp dụng scale cho kỹ năng
            projectile.transform.localScale = Vector3.one * scale;

            if (skillForm == SkillForm.StraightLine || skillForm == SkillForm.Point)
            {
                projectile.GetComponent<Projectile>().SetDirection(direction);
            }

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (impactEffectPrefab != null)
            {
                projectileScript.SetImpactEffect(impactEffectPrefab);
            }
            projectileScript.SetDuration(duration);

            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void DisableSkill(GameObject skill)
    {
        if (skill != null)
        {
            skill.gameObject.SetActive(false);
        }
    }

    public Vector2 GetEffectOffset(Vector2 direction)
    {
        if (direction == Vector2.up) return effectOffsetUp;
        if (direction == Vector2.down) return effectOffsetDown;
        if (direction == Vector2.left) return effectOffsetLeft;
        if (direction == Vector2.right) return effectOffsetRight;
        return Vector2.zero;
    }
}

public enum SkillType
{
    Attack,
    Defense,
    Support
}

public enum SkillForm
{
    Another,
    StraightLine,
    Point,
    Support
}