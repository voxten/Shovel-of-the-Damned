using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyPanelPuzzle : PuzzleObject
{
    [Header("PassCode")]
    public String PlayerPassCode { get; private set; }
    [SerializeField] private TextMeshPro passCodeText;
    
    [Header("AccessCard")]
    [SerializeField] private AccessCardData accessCardData;
    private List<AccessCardPair> _accessCardPairs;
    [SerializeField] private KeyPanelInsertCard insertCard;

    private void Awake()
    {
        ClearPasscode();
        _accessCardPairs = accessCardData.accessCards;
    }

    private void OnEnable()
    {
        KeyPanelEvents.AddNumber += AddNumber;
        KeyPanelEvents.CheckPasscode += CheckPasscode;
        KeyPanelEvents.ClearPasscode += ClearPasscode;
    }

    private void OnDisable()
    {
        KeyPanelEvents.AddNumber -= AddNumber;
        KeyPanelEvents.CheckPasscode -= CheckPasscode;
        KeyPanelEvents.ClearPasscode -= ClearPasscode;
    }

    private void AddNumber(int number)
    {
        if (PlayerPassCode.Length == 4) return;
        SoundManager.PlaySound3D(Sound.CodeInput, transform);
        PlayerPassCode += number.ToString();
        UpdateText();
    }

    private void CheckPasscode()
    {
        if (PlayerPassCode.Length == 4)
        {
            for (var i = 0; i < _accessCardPairs.Count; i++)
            {
                if (PlayerPassCode == _accessCardPairs[i].upgradeCode)
                {
                    if (Inventory.InventoryEvents.GetAccessCard())
                    {
                        ApprovedCode("Correct code... Now I need to insert card");
                        OnCardInsert();
                        passCodeText.text = "Card...";
                        InteractionSystem.InteractionEvents.ExitPuzzleInteraction();
                        QuitPuzzle();
                        return;
                    }
                    ApprovedCode("Correct code... But I need card for it");
                    ClearPasscode();
                    return;

                }
            }
            DeniedCode("Incorrect...");  
        }
        else
        {
            DeniedCode("Passcode is to short...");
        }
        UpdateText();
    }

    private void OnCardInsert()
    {
        // Get the current card from inventory
        var card = Inventory.InventoryEvents.GetAccessCard();
    
        if (card is AccessCardItem cardItem)
        {
            // Find the current card in the access card data
            int currentIndex = accessCardData.accessCards.FindIndex(c => c.cardType == cardItem.cardPair.cardType);
        
            // If there's a next level, get its upgrade code
            if (currentIndex < accessCardData.accessCards.Count - 1)
            {
                string requiredCode = accessCardData.accessCards[currentIndex + 1].upgradeCode;
                insertCard.SetInsertCard(requiredCode);
            }
            else
            {
                DeniedCode("Already at highest access level...");
            }
        }
    }

    private void UpdateText()
    {
        Debug.Log(PlayerPassCode);
        passCodeText.text = PlayerPassCode + "";
    }

    public void DeniedCode(string narrationText)
    {
        Narration.DisplayText?.Invoke(narrationText);
        SoundManager.PlaySound3D(Sound.CodeDenied, transform);
        ClearPasscode();
    }

    public void ApprovedCode(string narrationText)
    {
        Narration.DisplayText?.Invoke(narrationText);
        SoundManager.PlaySound3D(Sound.CodeApprove, transform);
    }

    public void ClearPasscode()
    {
        passCodeText.text = "";
        PlayerPassCode = "";
    }
    
    public static class KeyPanelEvents
    {
        public static Action<int> AddNumber;
        public static Action CheckPasscode;
        public static Action ClearPasscode;
    }
}
