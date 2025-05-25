using System;
using UnityEngine;
using DG.Tweening;

public class KeyPanelInsertCard : InteractableObject
{
    [Header("KeyPanelPuzzle")]
    [SerializeField] private KeyPanelPuzzle keyPanelPuzzle;
    private Collider _collider;
    
    [Header("AccessCard")]
    [SerializeField] private GameObject accessCardObject;
    [SerializeField] private AccessCardData accessCardData;
    private string _requiredUpgradeCode;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float moveDuration = 1f;

    [SerializeField] private EnemyAI enemyAI;

    private float _insertStartY = 0.2f;
    private float _insertEndY = -0.05f;

    private bool _isInserting;

    private bool _firstTime = true;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        
        if (accessCardObject != null)
        {
            var meshRenderer = accessCardObject.GetComponent<Renderer>();
            if (meshRenderer != null)
            {
                var color = meshRenderer.material.color;
                color.a = 0;
                meshRenderer.material.color = color;
            }
            accessCardObject.SetActive(false);
        }
    }
    
    public void SetInsertCard(string upgradeCode)
    {
        _requiredUpgradeCode = upgradeCode;
        _collider.enabled = true;
    }

    public override bool Interact()
    {
        base.Interact();

        Debug.Log("Wchodzi tutaj wgl)");
        if (_isInserting) 
            return false;
        
        _isInserting = true;
        
        SoundManager.PlaySound3D(Sound.CodeInput, transform);
        
        var card = Inventory.InventoryEvents.GetAccessCard();
        
        if (card is AccessCardItem cardItem)
        {
            if (keyPanelPuzzle.PlayerPassCode != _requiredUpgradeCode)
            {
                PlayAnimationSequence(() => 
                {
                    keyPanelPuzzle.DeniedCode("I don't have required card level to use that code...");
                    _isInserting = false;
                    keyPanelPuzzle.ClearPasscode();
                    _collider.enabled = false;
                });
                return false;
            }

            AccessCardType currentType = cardItem.cardPair.cardType;
            int currentIndex = accessCardData.accessCards.FindIndex(c => c.cardType == currentType);
            
            if (currentIndex < accessCardData.accessCards.Count - 1)
            {
                PlayAnimationSequence(() => 
                {
                    AccessCardPair nextCard = accessCardData.accessCards[currentIndex + 1];
                    cardItem.SetNewAccessCard(nextCard);
                    keyPanelPuzzle.ApprovedCode("Card was upgraded to next level");
                    keyPanelPuzzle.ClearPasscode();
                    _isInserting = false;
                    _collider.enabled = false;
                    if (_firstTime)
                    {
                        enemyAI.gameObject.SetActive(true);
                        _firstTime = false;
                    }
                    SavingSystem.SavingSystemEvents.Save();
                });
                return true;
            }
            
            PlayAnimationSequence(() => 
            {
                keyPanelPuzzle.DeniedCode("Already at highest access level");
                keyPanelPuzzle.ClearPasscode();
                _isInserting = false;
                _collider.enabled = false;
            });
            return false;
        }
        
        // If no card item
        keyPanelPuzzle.DeniedCode("I don't have access card...");
        keyPanelPuzzle.ClearPasscode();
        _isInserting = false;
        _collider.enabled = false;
        return false;
    }

    private void PlayAnimationSequence(Action onComplete)
    {
        if (accessCardObject == null)
        {
            onComplete?.Invoke();
            return;
        }

        // Reset card position and visibility
        accessCardObject.SetActive(true);
        var renderer = accessCardObject.GetComponent<Renderer>();
        Vector3 startPos = accessCardObject.transform.localPosition;
        startPos.y = _insertStartY;
        accessCardObject.transform.localPosition = startPos;

        if (renderer != null)
        {
            // Fade in
            renderer.material.DOFade(1, fadeDuration)
                .OnComplete(() => 
                {
                    // Move down
                    Vector3 endPos = accessCardObject.transform.localPosition;
                    endPos.y = _insertEndY;
                    accessCardObject.transform.DOLocalMove(endPos, moveDuration)
                        .OnComplete(() => 
                        {
                            // Fade out
                            renderer.material.DOFade(0, fadeDuration)
                                .OnComplete(() => 
                                {
                                    accessCardObject.SetActive(false);
                                    onComplete?.Invoke();
                                });
                        });
                });
        }
        else
        {
            // If no renderer, just do the movement
            Vector3 endPos = accessCardObject.transform.localPosition;
            endPos.y = _insertEndY;
            accessCardObject.transform.DOLocalMove(endPos, moveDuration)
                .OnComplete(() => 
                {
                    accessCardObject.SetActive(false);
                    onComplete?.Invoke();
                });
        }
    }
}