using System.Collections.Generic;
using UnityEngine;

public class AfterimagePool : MonoBehaviour
{
    // 어떤 스크립트에서든 쉽게 접근할 수 있도록 만드는 싱글톤 패턴
    public static AfterimagePool instance;

    [Header("오브젝트 풀 설정")]
    public GameObject afterimagePrefab; // 잔상으로 사용할 프리팹
    public int poolSize = 20; // 처음에 생성해 둘 잔상의 개수

    // 비활성화된 잔상들을 보관할 큐(Queue)
    private Queue<AfterimageSprite> poolQueue = new Queue<AfterimageSprite>();

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        instance = this;
        InitializePool();
    }

    // 처음에 정해진 개수만큼 잔상을 미리 생성해서 풀에 넣어두는 함수
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(afterimagePrefab, transform);
            obj.SetActive(false); // 비활성화 상태로 생성
            poolQueue.Enqueue(obj.GetComponent<AfterimageSprite>());
        }
    }

    // 풀에서 잔상을 하나 꺼내오는 함수
    public AfterimageSprite GetFromPool()
    {
        if (poolQueue.Count > 0)
        {
            AfterimageSprite afterimage = poolQueue.Dequeue();
            afterimage.gameObject.SetActive(true);
            return afterimage;
        }
        // 만약 풀이 비었다면 경고를 남기고 null 반환 (혹은 동적으로 추가 생성도 가능)
        Debug.LogWarning("Afterimage pool is empty!");
        return null;
    }

    // 사용이 끝난 잔상을 다시 풀에 되돌려놓는 함수
    public void ReturnToPool(AfterimageSprite afterimage)
    {
        afterimage.gameObject.SetActive(false);
        poolQueue.Enqueue(afterimage);
    }
}