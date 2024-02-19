using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UnityEvent<int> OnChangedScore;
    private int Score = 0;

    [SerializeField] private PollingListSO _poolingList;
    public Color _color { private set; get; }

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
        ModifySocre(Score + score);
    }

    private void ModifySocre(int socre)
    {
        Score = socre;
        OnChangedScore.Invoke(Score);
    }

    public float GetScore()
    {
        return Score;
    }
}
