using UnityEngine;
using System.Linq; // LINQ�� ����ϱ� ���� �߰�

public class AdvancedEquipmentController : MonoBehaviour
{
    [Header("���� �ʼ�")]
    [SerializeField] private GameObject playerCharacter; // �ν����Ϳ��� �÷��̾� SPUM ĳ���� ����

    // ĳ������ �ٽ� ��ũ��Ʈ ����
    private SPUM_Prefabs _spumPrefabs;
    private SPUM_SpriteList _spumSpriteList;

    void Start()
    {
        if (playerCharacter == null)
        {
            Debug.LogError("Player Character�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // ĳ���Ϳ��� �ٽ� ��ũ��Ʈ���� �����ɴϴ�.
        _spumPrefabs = playerCharacter.GetComponent<SPUM_Prefabs>();
        _spumSpriteList = playerCharacter.GetComponent<SPUM_SpriteList>();

        if (_spumPrefabs == null || _spumSpriteList == null)
        {
            Debug.LogError("ĳ���Ϳ��� SPUM_Prefabs �Ǵ� SPUM_SpriteList�� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// ������ �̸��� ���������� ��� ��ü�ϴ� ���� �Լ�
    /// </summary>
    /// <param name="itemName">ã���� �ϴ� �������� �̸� (SpumTextureData�� Name)</param>
    /// <param name="itemPartType">�������� ���� (SpumTextureData�� PartType, ��: "Cloth", "Weapon")</param>
    public void EquipItem(string itemName, string itemPartType)
    {
        if (_spumPrefabs == null) return;

        // 1. ��� ��Ű���� ������ ���ϴ� �������� �ؽ�ó �����͸� ã���ϴ�.
        SpumTextureData targetTextureData = null;
        foreach (var package in _spumPrefabs.spumPackages)
        {
            targetTextureData = package.SpumTextureData.FirstOrDefault(data =>
                data.Name == itemName && data.PartType == itemPartType
            );

            if (targetTextureData != null)
            {
                break; // �������� ã������ ���� ����
            }
        }

        // ������ �����͸� ã�� ���� ���
        if (targetTextureData == null)
        {
            Debug.LogWarning($"������ '{itemName}' (����: {itemPartType})�� spumPackages���� ã�� �� �����ϴ�.");
            return;
        }

        // 2. ã�� �ؽ�ó �������� ��θ� ����Ͽ� ĳ������ ������ �����մϴ�.
        UpdateSprite(itemPartType, targetTextureData.Path);

        // 3. (������) ���� �� ��� ������ �ִϸ��̼��� �ִٸ� �ִϸ��̼ǵ� ��ü�մϴ�.
        // ����: "GreatSword" �������� "GreatSword_Attack" �ִϸ��̼��� ����ϵ��� ����
        // �� �κ��� ������ ��ȹ�� ���� �����ϰ� ������ �� �ֽ��ϴ�.
        // ���⼭�� ������ �̸����� ���� �ִϸ��̼��� ã�� ���ø� �����ݴϴ�.
        UpdateAnimation(itemName);

        Debug.Log($"'{itemName}' ���� �Ϸ�!");
    }

    /// <summary>
    /// Ư�� ������ ��������Ʈ�� ���ο� ��η� ������Ʈ�ϰ� �����մϴ�.
    /// </summary>
    private void UpdateSprite(string partType, string resourcePath)
    {
        // partType�� ���� ������ ��� ����Ʈ�� �����մϴ�.
        var pathList = GetPathListByPartType(partType);
        if (pathList == null)
        {
            Debug.LogError($"�������� �ʴ� ���� Ÿ���Դϴ�: {partType}");
            return;
        }

        // ��θ� ������Ʈ�ϰ� ResyncData�� ������ ���ΰ�ħ�մϴ�.
        pathList.Clear();
        pathList.Add(resourcePath);
        _spumSpriteList.ResyncData();
    }

    /// <summary>
    /// ������ �̸��� ���õ� �ִϸ��̼��� ã�� ��ü�մϴ�.
    /// </summary>
    private void UpdateAnimation(string itemName)
    {
        // ����: "GreatSword" ��� �̸��� ���Ե� ���� �ִϸ��̼� Ŭ���� ã�Ƽ� ��ü
        SpumAnimationClip newAttackClipData = null;
        foreach (var package in _spumPrefabs.spumPackages)
        {
            newAttackClipData = package.SpumAnimationData.FirstOrDefault(clip =>
                clip.StateType == "ATTACK" && clip.Name.Contains(itemName)
            );
            if (newAttackClipData != null) break;
        }

        if (newAttackClipData != null)
        {
            var animClip = Resources.Load<AnimationClip>(newAttackClipData.ClipPath.Replace(".anim", ""));
            if (animClip != null)
            {
                // ���� �ִϸ��̼� ����� ù ��°�� ���ο� Ŭ������ ��ü
                if (_spumPrefabs.ATTACK_List.Count > 0)
                {
                    _spumPrefabs.ATTACK_List[0] = animClip;
                    Debug.Log($"���� �ִϸ��̼��� '{newAttackClipData.Name}'���� ��ü�߽��ϴ�.");
                }
            }
        }
    }


    /// <summary>
    /// ���� Ÿ�� ���ڿ��� ���� �ش��ϴ� ��� ����Ʈ�� ��ȯ�մϴ�.
    /// </summary>
    private System.Collections.Generic.List<string> GetPathListByPartType(string partType)
    {
        switch (partType)
        {
            case "Hair":
                return _spumSpriteList._hairListString;
            case "Cloth":
                return _spumSpriteList._clothListString;
            case "Armor":
                return _spumSpriteList._armorListString;
            case "Pant":
                return _spumSpriteList._pantListString;
            case "Weapons":
                return _spumSpriteList._weaponListString;
            case "Back":
                return _spumSpriteList._backListString;
            default:
                return null;
        }
    }


    // ----- �׽�Ʈ�� �ڵ� -----
    void Update()
    {
        // '1' Ű�� ���� 'Cloth_5' ��� �̸��� �� ���� �õ�
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // �� �̸��� SpumPackage ���� SpumTextureData�� ���ǵ� Name�� ��ġ�ؾ� �մϴ�.
            EquipItem("Cloth_5", "Cloth");
        }

        // '2' Ű�� ���� 'Weapon_2' ��� �̸��� ���� ���� �õ�
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItem("Sword_2", "Weapons");
        }
    }
}