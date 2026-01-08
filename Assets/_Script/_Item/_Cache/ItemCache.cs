using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ItemCache
{
    // Type별로 별도의 딕셔너리를 관리합니다.
    private static Dictionary<Type, Dictionary<string, ItemData>> allDatas = new Dictionary<Type, Dictionary<string, ItemData>>();
    private static List<Type> initializedTypes = new List<Type>();
    public static void ReadData<T>(string csvFileName) where T : ItemData
    {
        Type itemType = typeof(T);

        // 이미 해당 타입이 로드되었다면 중복 로드 방지
        if (initializedTypes.Contains(itemType)) return;

        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        if (csvData == null)
        {
            Debug.LogError($"[ItemCache] {csvFileName}을 찾을 수 없습니다.");
            return;
        }

        // 해당 타입 전용 딕셔너리 생성
        if (!allDatas.ContainsKey(itemType))
            allDatas[itemType] = new Dictionary<string, ItemData>();

        string[] lines = csvData.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1) return;

        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            if (values.Length != headers.Length) continue;

            T instance = Activator.CreateInstance<T>();

            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim();
                string value = values[j].Trim();
                if (string.IsNullOrEmpty(value)) continue;

                // FlattenHierarchy를 추가하여 부모 클래스의 필드까지 탐색
                FieldInfo field = itemType.GetField(header,
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);

                if (field != null)
                {
                    try
                    {
                        field.SetValue(instance, ConvertValue(value, field.FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[ItemCache] {header} 파싱 에러: {e.Message}");
                    }
                }
            }

            if (!allDatas[itemType].ContainsKey(instance.ItemName))
                allDatas[itemType].Add(instance.ItemName, instance);
        }

        initializedTypes.Add(itemType);
        Debug.Log($"[ItemCache] {itemType.Name} 로드 완료 ({allDatas[itemType].Count}개)");
    }

    // 제네릭 반환을 통해 캐스팅 불편함 해소
    public static T GetItem<T>(string itemName) where T : ItemData
    {
        if (allDatas.TryGetValue(typeof(T), out var dict))
        {
            if (dict.TryGetValue(itemName, out var item))
                return item as T;
        }
        return null;
    }
    public static ItemData GetItem(string itemName)
    {
        // 모든 딕셔너리를 순회하며 이름이 일치하는 아이템을 찾음
        foreach (var dict in allDatas.Values)
        {
            if (dict.TryGetValue(itemName, out var item)) return item;
        }
        return null;
    }

    // 값 변환 로직 분리 (가독성)
    private static object ConvertValue(string value, Type type)
    {
        if (type.IsEnum) return Enum.Parse(type, value);
        if (type == typeof(Vector2))
        {
            var s = value.Split('|');
            return new Vector2(float.Parse(s[0]), float.Parse(s[1]));
        }
        if (type == typeof(Vector3))
        {
            var s = value.Split('|');
            return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
        }
        return Convert.ChangeType(value, type);
    }
}