using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ItemCache
{
    private static Dictionary<string, ItemData> datas = new Dictionary<string, ItemData>();
    public static bool IsInitialized { get; private set; } = false;

    // 제네릭 메서드로 변경하여 확장성 확보 (T는 ItemData를 상속받아야 함)
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

        if (lines.Length <= 1) return; // 헤더만 있거나 비어있으면 리턴

        string[] headers = lines[0].Split(',');
        Type itemType = typeof(T); // 리플렉션 대신 제네릭 타입 사용으로 안전성 확보

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            if (values.Length != headers.Length)
            {
                Debug.LogWarning($"[ItemCache] {i}번째 줄의 데이터 개수가 헤더와 다릅니다. 건너뜁니다.");
                continue;
            }

            // 인스턴스 생성
            T instance = (T)Activator.CreateInstance(itemType);

            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim();
                string value = values[j].Trim();
                if (string.IsNullOrEmpty(value)) continue;

                FieldInfo field = itemType.GetField(header, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                if (field != null)
                {
                    try
                    {
                        object convertedValue;

                        // Enum 처리 로직 추가
                        if (field.FieldType.IsEnum)
                        {
                            convertedValue = Enum.Parse(field.FieldType, value);
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(value, field.FieldType);
                        }

                        field.SetValue(instance, convertedValue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[ItemCache] 데이터 파싱 에러 (Field: {header}, Value: {value}): {e.Message}");
                    }
                }
            }

            // 딕셔너리 추가 (중복 키 방지)
            if (!datas.ContainsKey(instance.ItemName))
            {
                datas.Add(instance.ItemName, instance);
                Debug.Log($"LoadedItem:{instance.ItemName}");
            }
            else
            {
                Debug.LogWarning($"[ItemCache] 중복된 ID 발견: {instance.ItemID}");
            }
        }

        IsInitialized = true;
        Debug.Log($"[ItemCache] {typeof(T).Name} 데이터 로드 완료: {datas.Count}개");
    }


    public static ItemData GetItem(string itemName)
    {
        // 초기화가 안 되어 있다면 기본값으로 MeleeWeaponData 로드 시도 (상황에 따라 변경)
        if (!IsInitialized)
        {
            Debug.LogWarning("[ItemCache] 초기화되지 않아 MeleeWeaponData를 자동으로 로드합니다.");
            // 주의: 실제 사용 시에는 명시적으로 호출하는 것이 좋습니다.
            // 여기서는 예시로 MeleeWeaponData를 가정합니다. 
            // 실제 구현에서는 이 부분을 제거하고 외부(GameManager)에서 ReadData를 호출하는 것을 권장합니다.
            // ReadData<MeleeWeaponData>("MeleeWeaponData"); 
            return null;
        }

        if (datas.TryGetValue(itemName, out ItemData item))
        {
            return item;
        }

        return null;
    }
}