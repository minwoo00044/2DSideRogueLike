using System.Collections.Generic;
using UnityEngine;

public class AfterimagePool : MonoBehaviour
{
    // � ��ũ��Ʈ������ ���� ������ �� �ֵ��� ����� �̱��� ����
    public static AfterimagePool instance;

    [Header("������Ʈ Ǯ ����")]
    public GameObject afterimagePrefab; // �ܻ����� ����� ������
    public int poolSize = 20; // ó���� ������ �� �ܻ��� ����

    // ��Ȱ��ȭ�� �ܻ���� ������ ť(Queue)
    private Queue<AfterimageSprite> poolQueue = new Queue<AfterimageSprite>();

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        instance = this;
        InitializePool();
    }

    // ó���� ������ ������ŭ �ܻ��� �̸� �����ؼ� Ǯ�� �־�δ� �Լ�
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(afterimagePrefab, transform);
            obj.SetActive(false); // ��Ȱ��ȭ ���·� ����
            poolQueue.Enqueue(obj.GetComponent<AfterimageSprite>());
        }
    }

    // Ǯ���� �ܻ��� �ϳ� �������� �Լ�
    public AfterimageSprite GetFromPool()
    {
        if (poolQueue.Count > 0)
        {
            AfterimageSprite afterimage = poolQueue.Dequeue();
            afterimage.gameObject.SetActive(true);
            return afterimage;
        }
        // ���� Ǯ�� ����ٸ� ��� ����� null ��ȯ (Ȥ�� �������� �߰� ������ ����)
        Debug.LogWarning("Afterimage pool is empty!");
        return null;
    }

    // ����� ���� �ܻ��� �ٽ� Ǯ�� �ǵ������� �Լ�
    public void ReturnToPool(AfterimageSprite afterimage)
    {
        afterimage.gameObject.SetActive(false);
        poolQueue.Enqueue(afterimage);
    }
}