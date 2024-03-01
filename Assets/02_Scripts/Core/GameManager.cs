using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;

public enum GameState { Home, InGame, End }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UnityEvent<int> OnChangedScore;

    public GameState GameState = GameState.Home;

    private int Score = 0;
    public Color _color { private set; get; }

    [Header("Reference")]
    [SerializeField] private Material m_skyMat;
    [SerializeField] private Transform _agent;

    [Space]

    [Header("Pool")]
    [SerializeField] private PollingListSO _poolingList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManager is running! Check!");
        }
        Instance = this;

        MakePool();
    }

    private void Start()
    {
        //_color = Random.ColorHSV(0, 1,0.8f, 1, 0.8f, 1);
        _color = Color.white;
        m_skyMat.color = _color;

        UIManager.Instance.ShoeHomeUI();
    }

    private void Update()
    {
        if (GameState != GameState.Home)
            return;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return;

                if (touch.phase == TouchPhase.Ended)
                    GameStart();
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                GameStart();
            }
        }
    }

    private void GameStart()
    {
        UIManager.Instance.HideHomeUI();

        GameState = GameState.InGame;
        _agent.gameObject.SetActive(true);
    }

    private void MakePool()
    {
        PoolManager.Instance = new PoolManager(transform);

        _poolingList.lis.ForEach(p => PoolManager.Instance.CreatePool(p.prefab, p.poolCount));
    }

    private void SetRandomColor()
    {
        _color = Random.ColorHSV(0, 1,0.8f, 1, 0.8f, 1);
        m_skyMat.DOColor(_color, 1);
    }

    public void ReSpawn()
    {
        UIManager.Instance.OnGameRestartSeq();

        ModifySocre(0);

        _color = Color.white;
        m_skyMat.DOColor(_color, 1);

        BlockManager.Instance.ClearBlock();
        BlockManager.Instance.SpanwBlockWithPos(Vector3.zero);

        if(GameObject.Find("Player").TryGetComponent<AgentController>(out AgentController _agent))
        {
            _agent.ReSpawnPlayer();
        }

        GameState = GameState.Home;
    }

#region Score

    public int GetScore()
    {
        return Score;
    }

    public void AddScore(int score = 1)
    {
        ModifySocre(Score + score);
    }

    private void ModifySocre(int socre)
    {
        Score = socre;
        OnChangedScore.Invoke(Score);

        if (Score % 10 == 0)
            SetRandomColor();
    }

#endregion
}
