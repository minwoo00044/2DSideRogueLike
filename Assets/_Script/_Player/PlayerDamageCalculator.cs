using System.Collections.Generic;
using UnityEngine;

// [핵심] 값을 담을 작은 그릇을 만듭니다.
// 누가 줬는지(Source)와 값(Value)을 함께 기억합니다.
[System.Serializable]
public class StatModifier
{
    public string Source; // 예: "IronRing", "FireBuff"
    public float Value;   // 예: 10.0f, 0.5f

    public StatModifier(string source, float value)
    {
        Source = source;
        Value = value;
    }
}

public class PlayerDamageCalculator
{
    public float BaseDamage = 10.0f;

    // Dictionary 대신 List를 사용합니다.
    // List는 순서가 있고, 똑같은 내용이 여러 개 들어와도 상관없습니다. (중복 허용)
    [SerializeField] // 유니티 인스펙터에서 보려고 끔
    private List<StatModifier> _flatModifiers = new List<StatModifier>();

    [SerializeField]
    private List<StatModifier> _percentModifiers = new List<StatModifier>();

    // 1. 버프/아이템 추가
    public void AddModifier(string sourceName, float value, bool isPercent)
    {
        // 딕셔너리처럼 "이미 있나?" 검사할 필요가 없습니다.
        // 그냥 리스트에 쓱 집어넣으면 됩니다.
        StatModifier newMod = new StatModifier(sourceName, value);

        if (isPercent)
            _percentModifiers.Add(newMod);
        else
            _flatModifiers.Add(newMod);

        // (선택) 여기서 CalculateDamage()를 미리 호출해서 캐싱해도 됩니다.
    }

    // 2. 버프/아이템 제거 (여기가 조금 다름)
    public void RemoveModifier(string sourceName)
    {
        // 리스트는 키(Key)가 없으므로, 하나씩 검사해서 이름이 같은 놈을 찾아 지워야 합니다.

        // [옵션 A] 이름이 같은 걸 전부 다 지운다. (버프 해제 시 적합)
        // _flatModifiers.RemoveAll(mod => mod.Source == sourceName);

        // [옵션 B] 이름이 같은 것 중 '맨 앞에 있는 하나'만 지운다. (아이템 해제 시 적합)
        // 쌍가락지 중 하나만 뺄 때 유용함
        RemoveFirstMatch(_flatModifiers, sourceName);
        RemoveFirstMatch(_percentModifiers, sourceName);
    }

    // 리스트에서 하나만 찾아서 지우는 헬퍼 함수
    private void RemoveFirstMatch(List<StatModifier> list, string name)
    {
        // 리스트를 순회하며 이름이 같은 첫 번째 녀석을 찾음
        var target = list.Find(mod => mod.Source == name);
        if (target != null)
        {
            list.Remove(target);
        }
    }

    // 3. 데미지 계산 (딕셔너리와 거의 같음)
    public float CalculateDamage()
    {
        float totalDamage = BaseDamage;

        // 리스트 순회 (Loop)
        foreach (var mod in _flatModifiers)
        {
            totalDamage += mod.Value;
        }

        float totalMultiplier = 1.0f;
        foreach (var mod in _percentModifiers)
        {
            totalMultiplier += mod.Value;
        }

        return totalDamage * totalMultiplier;
    }
}