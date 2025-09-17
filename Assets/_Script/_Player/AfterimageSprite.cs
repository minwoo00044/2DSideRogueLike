using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AfterimageSprite : MonoBehaviour
{
    [Header("잔상 설정")]
    public float fadeDuration = 0.5f;

    [Header("자동 구성용 프리팹")]
    public GameObject playerPrefabForSetup;

    private Dictionary<string, SpriteRenderer> _rendererDictionary;
    private float _fadeTimer;

#if UNITY_EDITOR
    [ContextMenu("Auto-Setup Hierarchy From Player Prefab")]
    private void AutoSetupHierarchy()
    {
        if (playerPrefabForSetup == null)
        {
            Debug.LogError("Player Prefab For Setup 필드에 원본 플레이어 프리팹을 할당해주세요!");
            return;
        }

        // 1. 기존 자식 오브젝트 모두 삭제
        foreach (Transform child in transform.Cast<Transform>().ToList())
        {
            DestroyImmediate(child.gameObject);
        }

        // 2. 원본 프리팹의 계층 구조를 재귀적으로 복사 시작
        RecursiveCopyHierarchy(playerPrefabForSetup.transform, transform);

        UnityEditor.EditorUtility.SetDirty(gameObject);
        Debug.Log("잔상 프리팹의 전체 계층 구조 자동 구성을 완료했습니다!");
    }

    // 재귀적으로 자식 오브젝트를 복사하는 함수
    private void RecursiveCopyHierarchy(Transform sourceParent, Transform targetParent)
    {
        foreach (Transform sourceChild in sourceParent)
        {
            // 새 오브젝트 생성 및 부모 설정
            GameObject newChild = new GameObject(sourceChild.name);
            newChild.transform.SetParent(targetParent);

            // 로컬 위치, 회전, 스케일 복사
            newChild.transform.localPosition = sourceChild.localPosition;
            newChild.transform.localRotation = sourceChild.localRotation;
            newChild.transform.localScale = sourceChild.localScale;

            // 원본에 SpriteRenderer가 있다면, 복사본에도 추가하고 설정 복사
            if (sourceChild.TryGetComponent<SpriteRenderer>(out SpriteRenderer sourceRenderer))
            {
                SpriteRenderer newRenderer = newChild.AddComponent<SpriteRenderer>();
                newRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
                newRenderer.sortingOrder = sourceRenderer.sortingOrder - 50; // 플레이어보다 뒤에 그려지도록 큰 값 빼기
            }

            // 이 자식 오브젝트의 자식들을 계속해서 재귀적으로 복사
            if (sourceChild.childCount > 0)
            {
                RecursiveCopyHierarchy(sourceChild, newChild.transform);
            }
        }
    }
#endif

    void Awake()
    {
        _rendererDictionary = new Dictionary<string, SpriteRenderer>();
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renderers)
        {
            // 동일한 이름의 키가 있을 경우를 대비한 안전장치
            if (!_rendererDictionary.ContainsKey(r.name))
            {
                _rendererDictionary.Add(r.name, r);
            }
        }
    }

    void OnEnable()
    {
        _fadeTimer = fadeDuration;
    }

    void Update()
    {
        _fadeTimer -= Time.deltaTime;
        float alpha = Mathf.Clamp01(_fadeTimer / fadeDuration);

        foreach (var renderer in _rendererDictionary.Values)
        {
            if (renderer.gameObject.activeSelf && renderer.material.HasProperty("_Color"))
            {
                Color newColor = renderer.color;
                newColor.a = alpha;
                renderer.color = newColor;
            }
        }

        if (_fadeTimer <= 0)
        {
            AfterimagePool.instance.ReturnToPool(this);
        }
    }

    public void Setup(Transform playerTransform)
    {
        transform.position = playerTransform.position;
        transform.localScale = playerTransform.localScale;

        foreach (var renderer in _rendererDictionary.Values)
        {
            renderer.gameObject.SetActive(false);
        }

        SpriteRenderer[] playerRenderers = playerTransform.GetComponentsInChildren<SpriteRenderer>();

        foreach (var playerRenderer in playerRenderers)
        {
            if (playerRenderer.gameObject.activeSelf)
            {
                if (_rendererDictionary.TryGetValue(playerRenderer.name, out SpriteRenderer targetRenderer))
                {
                    targetRenderer.gameObject.SetActive(true);
                    targetRenderer.sprite = playerRenderer.sprite;
                    targetRenderer.color = playerRenderer.color;
                }
            }
        }
    }
}