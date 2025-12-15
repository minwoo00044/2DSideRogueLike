using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ItemCache
{
    private static Dictionary<string, ItemData> datas = new Dictionary<string, ItemData>();
    public static bool IsInitialized { get; private set; } = false;

    public static void ReadData<T>(string csvFileName) where T : ItemData
    {
        if (IsInitialized) return;

        datas.Clear();
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);

        if (csvData == null)
        {
            Debug.LogError($"[ItemCache] Resources 폴더에서 {csvFileName}을 찾을 수 없습니다.");
            return;
        }

        string[] lines = csvData.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1) return;

        string[] headers = lines[0].Split(',');
        Type itemType = typeof(T);

        for (int i = 1; i < lines.Length; i++)
        {
            // CSV 쉼표 분리 (따옴표 처리가 없는 간단한 버전)
            string[] values = lines[i].Split(',');
            if (values.Length != headers.Length)
            {
                Debug.LogWarning($"[ItemCache] {i}번째 줄 데이터 개수 불일치. (Header: {headers.Length}, Value: {values.Length})");
                continue;
            }

            T instance = (T)Activator.CreateInstance(itemType);

            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim();
                string value = values[j].Trim();
                if (string.IsNullOrEmpty(value)) continue;

                // BindingFlags 추가: 부모 클래스의 필드도 찾기 위해 FlattenHierarchy 등 고려 가능하나, Public/NonPublic으로 충분할 수 있음
                FieldInfo field = itemType.GetField(header,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.IgnoreCase);

                if (field != null)
                {
                    try
                    {
                        object convertedValue = null;

                        // 1. Enum 처리
                        if (field.FieldType.IsEnum)
                        {
                            convertedValue = Enum.Parse(field.FieldType, value);
                        }
                        // 2. Vector2 처리 (형식: "x|y")
                        else if (field.FieldType == typeof(Vector2))
                        {
                            string[] vec = value.Split('|');
                            if (vec.Length == 2)
                            {
                                float x = float.Parse(vec[0]);
                                float y = float.Parse(vec[1]);
                                convertedValue = new Vector2(x, y);
                            }
                        }
                        // 3. Vector3 처리 (형식: "x|y|z")
                        else if (field.FieldType == typeof(Vector3))
                        {
                            string[] vec = value.Split('|');
                            if (vec.Length == 3)
                            {
                                float x = float.Parse(vec[0]);
                                float y = float.Parse(vec[1]);
                                float z = float.Parse(vec[2]);
                                convertedValue = new Vector3(x, y, z);
                            }
                        }
                        // 4. 기본 타입 처리
                        else
                        {
                            convertedValue = Convert.ChangeType(value, field.FieldType);
                        }

                        if (convertedValue != null)
                        {
                            field.SetValue(instance, convertedValue);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[ItemCache] 파싱 에러 (Field: {header}, Value: {value}): {e.Message}");
                    }
                }
                // 디버깅용: 필드를 못 찾았을 때 경고 (너무 많이 뜰 수 있으니 주의)
                // else { Debug.LogWarning($"필드 '{header}'를 찾을 수 없습니다."); }
            }

            if (!datas.ContainsKey(instance.ItemName))
            {
                datas.Add(instance.ItemName, instance);
            }
        }

        IsInitialized = true;
        Debug.Log($"[ItemCache] {typeof(T).Name} 데이터 로드 완료: {datas.Count}개");
    }

    public static ItemData GetItem(string itemName)
    {
        if (datas.TryGetValue(itemName, out ItemData item))
        {
            return item;
        }
        return null;
    }
}