using System.Collections.Generic;
using UnityEngine;

public static class SpumDataCache
{
    private static Dictionary<string, SpumTextureData> _cache = new Dictionary<string, SpumTextureData>();

    /// <summary>
    /// 게임 시작 시 SPUM Prefabs 데이터를 순회하며 캐시를 초기화합니다.
    /// </summary>
    /// <param name="prefabs">SPUM_Prefabs 컴포넌트</param>
    public static void Initialize(SPUM_Prefabs prefabs)
    {
        _cache.Clear();
        if (prefabs == null) return;

        foreach (var package in prefabs.spumPackages)
        {
            if (package.SpumTextureData == null) continue;

            foreach (var data in package.SpumTextureData)
            {
                // 중복 키 방지: 유니티 에셋 특성상 중복 이름이 있을 수 있으므로 체크
                if (!_cache.ContainsKey(data.Name))
                {
                    _cache.Add(data.Name, data);
                }
            }
        }
        Debug.Log($"[SpumDataCache] Initialized with {_cache.Count} items.");
    }

    /// <summary>
    /// 아이템 이름을 통해 SpumTextureData를 O(1)로 조회합니다.
    /// </summary>
    /// <param name="name">찾을 아이템 이름</param>
    /// <returns>SpumTextureData 객체 또는 null</returns>
    public static SpumTextureData GetSpumData(string name)
    {
        if (_cache.TryGetValue(name, out var data))
        {
            return data;
        }
        return null;
    }
}
