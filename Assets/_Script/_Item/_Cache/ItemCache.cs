using System.Collections.Generic;
using UnityEngine;

public static class ItemCache
{
    // 데이터를 외부에서 함부로 수정하지 못하도록 private 유지
    private static Dictionary<int, ItemData> datas = new Dictionary<int, ItemData>();

    // 초기화 여부 확인용 플래그
    public static bool IsInitialized { get; private set; } = false;

    public static void ReadData()
    {
        if (IsInitialized) return; // 이미 로드되었다면 중복 로드 방지

        datas.Clear(); // 안전하게 초기화

        // [예시] Resources 폴더의 ScriptableObject들을 로드한다고 가정
        // 실제로는 JSON 파싱, CSV 읽기 등이 들어갈 자리

        //foreach (var item in loadedItems)
        //{
        //    if (datas.ContainsKey(item.id))
        //    {
        //        Debug.LogWarning($"[ItemCache] 중복된 ID가 발견되었습니다: {item.id}");
        //        continue;
        //    }
        //    datas.Add(item.id, item);
        //}

        IsInitialized = true;
        Debug.Log($"[ItemCache] 아이템 데이터 로드 완료: {datas.Count}개");
    }

    public static ItemData GetItem(int id)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[ItemCache] 데이터가 로드되지 않았습니다! ReadData()를 먼저 호출하세요.");
            ReadData(); // 강제로 로드 시도
        }

        if (datas.TryGetValue(id, out ItemData item))
        {
            return item;
        }

        Debug.LogWarning($"[ItemCache] ID {id}에 해당하는 아이템을 찾을 수 없습니다.");
        return null;
    }
}