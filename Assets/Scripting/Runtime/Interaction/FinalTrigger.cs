using System;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using DG.Tweening;
using StarterAssets;

public class FinalTrigger : InteractableObject
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject enemySpawnPoint;
    [SerializeField] private FirstPersonController player;

    [Header("Quote Sequence")] 
    [SerializeField] private CanvasGroup quotesPanel;
    [SerializeField] private CanvasGroup quotesCanvasGroup;
    [SerializeField] private TextMeshProUGUI quoteText;
    [TextArea] [SerializeField] private string[] quotes;
    [SerializeField] private float quoteDisplayTime = 3f;
    [SerializeField] private float fadeDuration = 1f;

    public bool isTriggered;
    private NavMeshAgent _navMeshAgent;
    private EnemyAI _enemyAI;

    private void Awake()
    {
        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();
        _enemyAI = enemy.GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        FinalEvents.EndGame += EndGame;
    }

    private void OnDisable()
    {
        FinalEvents.EndGame -= EndGame;
    }

    public override bool Interact()
    {
        if (isTriggered)
            return false;

        SpawnEnemy();
        return true;
    }

    private void SpawnEnemy()
    {
        isTriggered = true;
        _enemyAI.StopAllCouroutines();
        _navMeshAgent.Warp(enemySpawnPoint.transform.position);
        enemy.transform.LookAt(player.gameObject.transform);
    }

    private void EndGame()
    {
        PlayerDeathUIPlayerDeathUIManager.DeathEvents.SetIsPlayerDead();
        quotesCanvasGroup.gameObject.SetActive(true);
        quotesPanel.gameObject.SetActive(true);
        quotesCanvasGroup.alpha = 0f;
        quotesPanel.alpha = 0f;
        quotesPanel.DOFade(1f, 2)
            .OnComplete(ShowQuotesSequence);
    }

    private void ShowQuotesSequence()
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < quotes.Length; i++)
        {
            int index = i;
            sequence.AppendCallback(() =>
            {
                quoteText.text = quotes[index];
            });
            sequence.Append(quotesCanvasGroup.DOFade(1f, fadeDuration));
            sequence.AppendInterval(quoteDisplayTime);
            sequence.Append(quotesCanvasGroup.DOFade(0f, fadeDuration));
        }

        sequence.OnComplete(() =>
        {
            SceneLoader.SceneEvents.AnimateLoadScene("MainMenu");
        });
    }

    public static class FinalEvents
    {
        public static Action EndGame;
    }
}
