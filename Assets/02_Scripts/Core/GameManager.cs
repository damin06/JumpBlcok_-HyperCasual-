using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UnityEvent<int> OnChangedScore;

    private int Score = 0;
    public Color _color { private set; get; }

    [Header("Pool")]
    [SerializeField] private PollingListSO _poolingList;

    [Space]

    [Header("Reference")]
    [SerializeField] private Image _gameOverPanel;

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

#region Score

    public float GetScore()
    {
        return Score;
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

#endregion

    public void OnGaemOver()
    {
        _gameOverPanel.gameObject.SetActive(true);
        _gameOverPanel.DOFade(0.75f, 1f).SetEase(Ease.InSine);
        //_gameOverUI.
    }
}
