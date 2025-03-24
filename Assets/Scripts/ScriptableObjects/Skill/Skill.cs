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

    [Header("Năng lượng")]
    public int manaCost;

    [Header("Thời gian tồn tại của kỹ năng")]
    public float duration = 5f; // Thời gian tồn tại tối đa của skillPrefab

    [Header("Khoảng cách (dùng cho StraightLine)")]
    public float khoangCach = 1f;

    [Header("Kỹ năng")]
    public GameObject skillPrefab;

    [Header("Hiệu ứng vận chiêu (xử lý trong CombatManager)")]
    public GameObject chargingEffectPrefab;

    [Header("Hiệu ứng va chạm (dùng EffectPool)")]
    public GameObject impactEffectPrefab; // Hiệu ứng khi kỹ năng va chạm (nếu có)

    private Animator skillAnim;

    [Header("Âm Thanh")]
    public AudioClip soundEffect;

    [Header("Hiệu ứng vận chiêu - Tùy chỉnh vị trí theo hướng (dùng cho StraightLine)")]
    public Vector2 effectOffsetUp = new Vector2(0f, 1.2f);
    public Vector2 effectOffsetDown = new Vector2(0f, -1f);
    public Vector2 effectOffsetLeft = new Vector2(-1.2f, 0f);
    public Vector2 effectOffsetRight = new Vector2(1.2f, 0f);

    [Header("Skill - Tùy chỉnh góc xoay theo hướng (dùng cho StraightLine)")]
    public float up = -180f;
    public float down = 0f;
    public float right = 90f;
    public float left = -90f;

    public void ActivateSkill(GameObject user)
    {
        Vector3 spawnPosition = Vector3.zero;
        Vector2 direction = Vector2.zero;
        float angle = 0f;

        // Xử lý vị trí và hướng dựa trên SkillForm
        switch (skillForm)
        {
            case SkillForm.StraightLine:
                // Lấy hướng từ PlayerMovement
                direction = user.GetComponent<PlayerMovement>().GetFacingDirection();
                spawnPosition = user.transform.position + (Vector3)(direction * khoangCach);

                // Xoay skill theo hướng bắn
                if (direction == Vector2.up) angle = up;
                else if (direction == Vector2.down) angle = down;
                else if (direction == Vector2.left) angle = left;
                else if (direction == Vector2.right) angle = right;
                break;

            case SkillForm.Point:
                // Lấy vị trí con chuột trên màn hình
                Vector3 mousePosition = Input.mousePosition;
                spawnPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                spawnPosition.z = 0f; // Đảm bảo z = 0 trong 2D

                // Tính hướng từ nhân vật đến vị trí con chuột
                direction = (spawnPosition - user.transform.position).normalized;
                if (direction == Vector2.up) angle = up;
                else if (direction == Vector2.down) angle = down;
                else if (direction == Vector2.left) angle = left;
                else if (direction == Vector2.right) angle = right;

                // Xoay skill theo hướng con chuột
                //angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                break;

            case SkillForm.Support:
                // Đặt vị trí kỹ năng ngay tại nhân vật
                spawnPosition = user.transform.position;
                direction = Vector2.zero; // Không cần hướng
                angle = 0f; // Không cần xoay
                break;

            case SkillForm.Another:
                // Để trống hoặc thêm logic cho dạng kỹ năng khác nếu cần
                spawnPosition = user.transform.position;
                direction = Vector2.zero;
                angle = 0f;
                break;
        }

        // Tạo kỹ năng
        if (skillPrefab != null)
        {
            skillAnim = skillPrefab.GetComponent<Animator>();
            GameObject projectile = Instantiate(skillPrefab, spawnPosition, Quaternion.identity);

            // Gán hướng cho projectile (nếu cần)
            if (skillForm == SkillForm.StraightLine || skillForm == SkillForm.Point)
            {
                projectile.GetComponent<Projectile>().SetDirection(direction);
            }

            // Gán impactEffectPrefab và duration cho projectile (nếu có)
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (impactEffectPrefab != null)
            {
                projectileScript.SetImpactEffect(impactEffectPrefab);
            }
            projectileScript.SetDuration(duration);

            // Xoay skill
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Phát âm thanh (nếu có)
        if (soundEffect != null)
        {
            AudioSource.PlayClipAtPoint(soundEffect, user.transform.position);
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