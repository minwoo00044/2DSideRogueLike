using UnityEngine;
using System.Linq; // LINQ를 사용하기 위해 추가

public class AdvancedEquipmentController : MonoBehaviour
{
    public static AdvancedEquipmentController Instance { get; set; }
    [Header("연결 필수")]
    [SerializeField] private GameObject playerCharacter; // 인스펙터에서 플레이어 SPUM 캐릭터 연결
    [SerializeField]private PlayerController playerController;  

    // 캐릭터의 핵심 스크립트 참조
    private SPUM_Prefabs _spumPrefabs;
    private SPUM_SpriteList _spumSpriteList;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        if (playerCharacter == null)
        {
            Debug.LogError("Player Character가 할당되지 않았습니다!");
            return;
        }

        // 캐릭터에서 핵심 스크립트들을 가져옵니다.
        _spumPrefabs = playerCharacter.GetComponent<SPUM_Prefabs>();
        _spumSpriteList = playerCharacter.GetComponent<SPUM_SpriteList>();
        playerController.GetComponent<PlayerController>();

        if (_spumPrefabs == null || _spumSpriteList == null)
        {
            Debug.LogError("캐릭터에서 SPUM_Prefabs 또는 SPUM_SpriteList를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 지정된 이름의 아이템으로 장비를 교체하는 메인 함수
    /// </summary>
    /// <param name="itemName">찾고자 하는 아이템의 이름 (SpumTextureData의 Name)</param>
    /// <param name="itemPartType">아이템의 부위 (SpumTextureData의 PartType, 예: "Cloth", "Weapon")</param>
    public void EquipItem(string itemName)
    {
        if (_spumPrefabs == null) return;

        // 1. 모든 패키지를 뒤져서 원하는 아이템의 텍스처 데이터를 찾습니다.
        SpumTextureData targetTextureData = null;
        foreach (var package in _spumPrefabs.spumPackages)
        {
            targetTextureData = package.SpumTextureData.FirstOrDefault(data =>
                data.Name == itemName
            );

            if (targetTextureData != null)
            {
                break; // 아이템을 찾았으면 루프 종료
            }
        }

        // 아이템 데이터를 찾지 못한 경우
        if (targetTextureData == null)
        {
            Debug.LogWarning($"아이템 '{itemName}'를 spumPackages에서 찾을 수 없습니다.");
            return;
        }
        var itemData = ItemCache.GetItem(itemName);
        if (itemData == null)
        {
            Debug.LogWarning($"아이템 '{itemName}'를 db에서 찾을 수 없습니다.");
            return;
        }
        //playerController.items[itemData.ItemType] = it
        // 2. 찾은 텍스처 데이터의 경로를 사용하여 캐릭터의 외형을 변경합니다.
        UpdateSprite(itemData.ItemType.ToString(), targetTextureData.Path);
        playerController.ChangeItem(itemData);

        // 3. (선택적) 만약 이 장비에 고유한 애니메이션이 있다면 애니메이션도 교체합니다.
        // 예시: "GreatSword" 아이템은 "GreatSword_Attack" 애니메이션을 사용하도록 설정
        // 이 부분은 게임의 기획에 따라 복잡하게 구현될 수 있습니다.
        // 여기서는 간단히 이름으로 관련 애니메이션을 찾는 예시를 보여줍니다.
        UpdateAnimation(itemName);

        Debug.Log($"'{itemName}' 장착 완료!");
    }

    /// <summary>
    /// 특정 부위의 스프라이트를 새로운 경로로 업데이트하고 적용합니다.
    /// </summary>
    private void UpdateSprite(string partType, string resourcePath)
    {
        // partType에 따라 적절한 경로 리스트를 선택합니다.
        var pathList = GetPathListByPartType(partType);
        if (pathList == null)
        {
            Debug.LogError($"지원하지 않는 파츠 타입입니다: {partType}");
            return;
        }

        // 경로를 업데이트하고 ResyncData로 외형을 새로고침합니다.
        pathList.Clear();
        pathList.Add(resourcePath);
        _spumSpriteList.ResyncData();
    }

    /// <summary>
    /// 아이템 이름과 관련된 애니메이션을 찾아 교체합니다.
    /// </summary>
    private void UpdateAnimation(string itemName)
    {
        // 예시: "GreatSword" 라는 이름이 포함된 공격 애니메이션 클립을 찾아서 교체
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
                // 공격 애니메이션 목록의 첫 번째를 새로운 클립으로 교체
                if (_spumPrefabs.ATTACK_List.Count > 0)
                {
                    _spumPrefabs.ATTACK_List[0] = animClip;
                    Debug.Log($"공격 애니메이션을 '{newAttackClipData.Name}'으로 교체했습니다.");
                }
            }
        }
    }


    /// <summary>
    /// 파츠 타입 문자열에 따라 해당하는 경로 리스트를 반환합니다.
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


    // ----- 테스트용 코드 -----
    void Update()
    {
        //// '1' 키를 눌러 'Cloth_5' 라는 이름의 옷 장착 시도
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    // 이 이름은 SpumPackage 내의 SpumTextureData에 정의된 Name과 일치해야 합니다.
        //    EquipItem("Cloth_5", "Cloth");
        //}

        //// '2' 키를 눌러 'Weapon_2' 라는 이름의 무기 장착 시도
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    EquipItem("Sword_2", "Weapons");
        //}
    }
}