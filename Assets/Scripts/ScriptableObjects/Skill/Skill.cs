using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Thời gian hồi")]
    public float cooldown;

    [Header("Năng lượng")]
    public int manaCost;

    [Header("Khoảng cách")]
    public float khoangCach = 1f;

    [Header("Kỹ năng")]
    public GameObject skillPrefab;
    private Animator skillAnim;

    [Header("Âm Thanh")]
    public AudioClip soundEffect;


    public void ActivateSkill(GameObject user)
    {
        skillAnim = skillPrefab.GetComponent<Animator>();

        // Tính toán vị trí mới cho prefab
        Vector2 direction = user.GetComponent<PlayerMovement>().GetFacingDirection();
        Vector3 viTriMoi = user.transform.position + (Vector3)(direction * khoangCach);

        GameObject projectile = Instantiate(skillPrefab, viTriMoi, Quaternion.identity);

        projectile.GetComponent<Projectile>().SetDirection(direction);

        // Xoay projectile thủ công
        float angle = 0f;
        if (direction == Vector2.up) angle = -180f;
        else if (direction == Vector2.down) angle = 0f;
        else if (direction == Vector2.left) angle = -90f;
        else if (direction == Vector2.right) angle = 90f;

        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //Debug.Log($"Hướng bắn: {direction}, Góc quay: {angle}, Rotation: {projectile.transform.rotation}, transform.right: {projectile.transform.right}");
    }
}



public enum SkillType 
{
    Attack, 
    Defense, 
    Support 
}
