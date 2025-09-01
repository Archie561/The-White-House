using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TradeManager : MonoBehaviour
{
    [SerializeField] private TradeOffer _tradeOfferPrefab;
    [SerializeField] private Transform _tradeOfferParent;

    public UnityEvent<TradeObjectType, TradeObjectData> OnDataModified;

    public void Start()
    {

        for (int i = 0; i < 4; i++)
        {
            var tradeOffer = Instantiate(_tradeOfferPrefab, _tradeOfferParent);
            tradeOffer.OnClick += OnOfferSigned;
        }

        foreach (TradeObjectType type in Enum.GetValues(typeof(TradeObjectType)))
            UpdateTradeObjectAmount(type, 0);
    }

    private void OnOfferSigned(TradeOffer offer)
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);

        var exportObjects = offer.ExportObjects;
        var importObjects = offer.ImportObjects;

        foreach (var exportObject in exportObjects)
            UpdateTradeObjectAmount(exportObject.Type, -exportObject.Amount);
        foreach (var importObject in importObjects)
            UpdateTradeObjectAmount(importObject.Type, importObject.Amount);

        offer.OnClick -= OnOfferSigned;
        Destroy(offer.gameObject);
    }

    private void UpdateTradeObjectAmount(TradeObjectType type, int amount)
    {
        var data = GameDataManager.GetTradeObjectData(type);

        if (!data.TryUpdateAmount(amount))
            Debug.Log($"Ресурсу {type} недостатньо!");

        OnDataModified?.Invoke(type, data);
    }

    public void ExitToMainScene()
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        LevelManager.Instance.LoadScene("Main");
    }
}
