using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AfterimageSprite : MonoBehaviour
{
    [Header("�ܻ� ����")]
    public float fadeDuration = 0.5f;

    [Header("�ڵ� ������ ������")]
    public GameObject playerPrefabForSetup;

    private Dictionary<string, SpriteRenderer> _rendererDictionary;
    private float _fadeTimer;

#if UNITY_EDITOR
    [ContextMenu("Auto-Setup Hierarchy From Player Prefab")]
    private void AutoSetupHierarchy()
    {
        if (playerPrefabForSetup == null)
        {
            Debug.LogError("Player Prefab For Setup �ʵ忡 ���� �÷��̾� �������� �Ҵ����ּ���!");
            return;
        }

        // 1. ���� �ڽ� ������Ʈ ��� ����
        foreach (Transform child in transform.Cast<Transform>().ToList())
        {
            DestroyImmediate(child.gameObject);
        }

        // 2. ���� �������� ���� ������ ��������� ���� ����
        RecursiveCopyHierarchy(playerPrefabForSetup.transform, transform);

        UnityEditor.EditorUtility.SetDirty(gameObject);
        Debug.Log("�ܻ� �������� ��ü ���� ���� �ڵ� ������ �Ϸ��߽��ϴ�!");
    }

    // ��������� �ڽ� ������Ʈ�� �����ϴ� �Լ�
    private void RecursiveCopyHierarchy(Transform sourceParent, Transform targetParent)
    {
        foreach (Transform sourceChild in sourceParent)
        {
            // �� ������Ʈ ���� �� �θ� ����
            GameObject newChild = new GameObject(sourceChild.name);
            newChild.transform.SetParent(targetParent);

            // ���� ��ġ, ȸ��, ������ ����
            newChild.transform.localPosition = sourceChild.localPosition;
            newChild.transform.localRotation = sourceChild.localRotation;
            newChild.transform.localScale = sourceChild.localScale;

            // ������ SpriteRenderer�� �ִٸ�, ���纻���� �߰��ϰ� ���� ����
            if (sourceChild.TryGetComponent<SpriteRenderer>(out SpriteRenderer sourceRenderer))
            {
                SpriteRenderer newRenderer = newChild.AddComponent<SpriteRenderer>();
                newRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
                newRenderer.sortingOrder = sourceRenderer.sortingOrder - 50; // �÷��̾�� �ڿ� �׷������� ū �� ����
            }

            // �� �ڽ� ������Ʈ�� �ڽĵ��� ����ؼ� ��������� ����
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
            // ������ �̸��� Ű�� ���� ��츦 ����� ������ġ
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