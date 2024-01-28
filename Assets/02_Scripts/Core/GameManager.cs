using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PollingListSO _poolingList;
    private int Score = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManager is running! Check!");
        }
        Instance = this;

        MakePool();
    }

    private void MakePool()
    {
        PoolManager.Instance = new PoolManager(transform);

        _poolingList.lis.ForEach(p => PoolManager.Instance.CreatePool(p.prefab, p.poolCount));
    }

    public void PlusScore(int score = 1)
    {
        Score += score;
    }

    public int GetScore()
    {
        return Score;
    }
}
