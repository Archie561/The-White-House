using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TradeOffer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TradeObject _tradeObjectPrefab;
    [SerializeField] private Transform _exportParent;
    [SerializeField] private Transform _importParent;

    private List<TradeObject> _exportObjects;
    private List<TradeObject> _importObjects;

    public IReadOnlyList<TradeObject> ExportObjects => _exportObjects;
    public IReadOnlyList<TradeObject> ImportObjects => _importObjects;


    private Stack<TradeObjectType> _unusedTradeObjectTypes;

    private int _tradeObjectsCount;

    public event Action<TradeOffer> OnClick;

    private void Start()
    {
        _exportObjects = new List<TradeObject>();
        _importObjects = new List<TradeObject>();
        _unusedTradeObjectTypes = new Stack<TradeObjectType>(Enum.GetValues(typeof(TradeObjectType))
            .Cast<TradeObjectType>()
            .OrderBy(x => UnityEngine.Random.value)
            .ToList());

        _tradeObjectsCount = UnityEngine.Random.Range(1, 3);

        for (int i = 0; i < _tradeObjectsCount; i++)
        {
            var exportObject = GenerateRandomTradeObject(_exportParent);
            if (exportObject != null)
            {
                _exportObjects.Add(exportObject);
            }
            var importObject = GenerateRandomTradeObject(_importParent);
            if (importObject != null)
            {
                _importObjects.Add(importObject);
            }
        }
    }

    private TradeObject GenerateRandomTradeObject(Transform parent)
    {
        if (_unusedTradeObjectTypes.Count == 0)
        {
            Debug.LogWarning("Not enough unused trade object types to generate a new trade offer.");
            return null;
        }

        var randomTradeObjectType = _unusedTradeObjectTypes.Pop();
        var randomAmount = UnityEngine.Random.Range(1, 31);

        var tradeObject = Instantiate(_tradeObjectPrefab, parent); ;
        tradeObject.SetType(randomTradeObjectType);
        tradeObject.SetAmount(randomAmount);

        return tradeObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
}
