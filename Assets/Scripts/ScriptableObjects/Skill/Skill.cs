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

    [Header("Khoảng cách")]
    public float khoangCach = 1f;

    [Header("Kỹ năng")]
    public GameObject skillPrefab;

    [Header("Hiệu ứng vận chiêu")]
    public GameObject chargingEffectPrefab; 

    private Animator skillAnim;

    [Header("Âm Thanh")]
    public AudioClip soundEffect;

    [Header("Hiệu ứng vận chiêu - Tùy chỉnh vị trí theo hướng")]
    public Vector2 effectOffsetUp = new Vector2(0f, 1.2f);
    public Vector2 effectOffsetDown = new Vector2(0f, -1f);
    public Vector2 effectOffsetLeft = new Vector2(-1.2f, 0f);
    public Vector2 effectOffsetRight = new Vector2(1.2f, 0f);


    [Header("Skill - Tùy chỉnh vị trí theo hướng")]
    public float up = -180f;
    public float down = 0f;
    public float right = 90f;
    public float left = -90f;


    public void ActivateSkill(GameObject user)
    {
        skillAnim = skillPrefab.GetComponent<Animator>();

        // Lấy hướng từ PlayerMovement
        Vector2 direction = user.GetComponent<PlayerMovement>().GetFacingDirection();
        Vector3 spawnPosition = user.transform.position + (Vector3)(direction * khoangCach);

        // Tạo kỹ năng
        GameObject projectile = Instantiate(skillPrefab, spawnPosition, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);

        // Xoay skill theo hướng bắn
        float angle = 0f;
        if (direction == Vector2.up) angle = up;
        else if (direction == Vector2.down) angle = down;
        else if (direction == Vector2.left) angle = left;
        else if (direction == Vector2.right) angle = right;

        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void DisableSkill(GameObject skill)
    {
        if(skill != null)
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
