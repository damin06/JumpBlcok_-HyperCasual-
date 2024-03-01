using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Reference")]

    [Header("InGame")]
    [SerializeField] private TextMeshProUGUI m_socreTXT;

    [Header("GameOver")]
    [SerializeField] private Image m_gameOverPanel;
    [SerializeField] private TextMeshProUGUI m_gameOverScore;

    [Header("Home")]
    [SerializeField] private TextMeshProUGUI m_titleTXT;
    [SerializeField] protected TextMeshProUGUI m_tapeMessageTXT;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple UIManager is running! Check!");
        }
        Instance = this;
    }

    public void ModifyScoreTXT(int score)
    {
        m_socreTXT.text = score.ToString();
    }

    public void OnGameOverSeq()
    {
        Sequence gameOverSequence = DOTween.Sequence();

        int score = GameManager.Instance.GetScore();
        m_gameOverScore.text = score.ToString();

        gameOverSequence.OnStart(() =>
        {
            m_gameOverPanel.gameObject.SetActive(true);
            m_gameOverPanel.DOFade(0, 0);
            m_gameOverScore.DOFade(0, 0);
        })
        .Append(m_gameOverPanel.DOFade(0.7f, 0.75f).SetEase(Ease.InSine))
        .Insert(0.5f, m_gameOverScore.DOFade(1f, 0.75f).SetEase(Ease.InSine))
        .OnStepComplete(() => 
        {
            StartCoroutine(Count(score, 0));
        });
    }

    public void OnGameRestartSeq()
    {
        Sequence gameRestartSequence = DOTween.Sequence();

        gameRestartSequence
            .Append(m_gameOverScore.DOFade(0, 0.5f).SetEase(Ease.InSine))
            .Insert(0.5f, m_gameOverPanel.DOFade(0, 0.4f).SetEase(Ease.InSine))
            .OnStepComplete(() =>
            {
                m_gameOverPanel.gameObject.SetActive(false);
            });
    }


    IEnumerator Count(float target, float current)
    {
        float duration = 0.6f; 
        float offset = (target - current) / duration;

        while (current < target)
        {
            current += offset * Time.deltaTime;
            m_gameOverScore.text = ((int)current).ToString();
            yield return null;
        }

        current = target;
        m_gameOverScore.text = ((int)current).ToString();
    }

    public void ShoeHomeUI()
    {
        m_titleTXT.gameObject.SetActive(true);
        m_tapeMessageTXT.gameObject.SetActive(true);

        m_titleTXT.DOFade(1, 0.5f).SetEase(Ease.InSine);
        StartCoroutine(TapMessage());
    }

    float FadeTime = 0.8f;
    private IEnumerator TapMessage()
    {
        m_tapeMessageTXT.DOFade(1, FadeTime).SetEase(Ease.InSine);
        yield return new WaitForSeconds(FadeTime);
        m_tapeMessageTXT.DOFade(0, FadeTime).SetEase(Ease.InSine);
        yield return new WaitForSeconds(FadeTime);
        StartCoroutine(TapMessage());
    }

    public void HideHomeUI()
    {
        m_titleTXT.DOFade(0, 0.5f).SetEase(Ease.InSine)
            .OnComplete(()=>{
            m_titleTXT.gameObject.SetActive(false);
        });

        StopAllCoroutines();
        m_tapeMessageTXT.DOFade(0, 0.5f).SetEase(Ease.InSine)
            .OnComplete(() => {
                m_tapeMessageTXT.gameObject.SetActive(false);
            });
    }
}
