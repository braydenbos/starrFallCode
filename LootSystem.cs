using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;
public class LootSystem : GenericSingleton<LootSystem>
{
    private LootTable _lootTable;
    private LootForce _lootForce;
    private Transform _lootTransform;
    private Roll _lastRoll = new Roll();
    private int _totalRolls;

    private GameObject _lastPickedItem;

    public class Roll
    {
        public readonly List<GameObject> guaranteedItems = new List<GameObject>();
        public readonly List<GameObject> chanceItems = new List<GameObject>();
    }
    

    public void ChooseLoot(LootTable lootTable,LootForce lootForce,Transform lootTransform)
    {
        _lootTable = lootTable;
        _lootForce = lootForce;
        _lootTransform = lootTransform;
        
        _lastPickedItem = null;
        
        if (_lootTable.rollAmountOptions == ValueOptions.oneValue)
        {
            _totalRolls = _lootTable.rollAmount;
            DropRequiredItems();
            for (int i = 0; i < _lootTable.rollAmount; i++)
            {
                DropChanceItems();
            }
        }
        else if (_lootTable.rollAmountOptions == ValueOptions.BetweenTwoValues)
        {
            var rollAmount = Random.Range(_lootTable.minMaxRollAmount.Min, _lootTable.minMaxRollAmount.Max + 1);
            _totalRolls = rollAmount;
            DropRequiredItems();
            for (int i = 0; i < rollAmount; i++)
            {
                DropChanceItems();
            }
        }

    }

    public Roll RollLoot(LootTable lootTable,LootForce lootForce,Transform lootTransform)
    {
        Roll roll = new Roll();
        _lastRoll = roll;
        ChooseLoot(lootTable,lootForce,lootTransform);
        return _lastRoll;
    }
    
    
    private void DropRequiredItems()
    {
        if (!_lootTable.HasGuaranteedDrops) {return;}
        foreach (var lootDropVariant in _lootTable.guaranteedDrops	)
        {
            if (lootDropVariant.valueOptions == ValueOptions.oneValue)
            {
                DropItems(lootDropVariant.dropVariant,lootDropVariant.amount,true);
            }
            else
            {
                var dropAmount =  Random.Range(lootDropVariant.minMaxAmount.Min,lootDropVariant.minMaxAmount.Max +1);
                DropItems(lootDropVariant.dropVariant,dropAmount,true);
            }
        }

    }
    
    private void DropChanceItems( )
    {
        if (!WillChanceItemsDrop() || !_lootTable.HasLootDrops){return;}
        var element = GetElement(CalculateDropChance());
        
        var lootDropVariant = PickRandomLootDropVariants(element.lootDropVariants);

        if (lootDropVariant.valueOptions == ValueOptions.oneValue)
        {
            DropItems(lootDropVariant.dropVariant,lootDropVariant.amount,false);
        }
        else
        {
            var dropAmount =  Random.Range(lootDropVariant.minMaxAmount.Min,lootDropVariant.minMaxAmount.Max +1);
            DropItems(lootDropVariant.dropVariant,dropAmount,false);
        }
    }
    
    private bool WillChanceItemsDrop()
    {
        return Random.value >= 0.0;
    }
    
    private float CalculateDropChance()
    {
        return Random.Range(0, CalculateTotalDropChance());
    }
    private float CalculateTotalDropChance()
    {
        float totalDropChance = 0;
        foreach (var elements in _lootTable.lootDropList)
        {
            totalDropChance += elements.chance;
        }

        return totalDropChance;
    }
    
    private DropVariantDetails PickRandomLootDropVariants(IReadOnlyList<DropVariantDetails> targetItems)
    {
        return targetItems[Random.Range(0, targetItems.Count)];
    }
    private LootDrops GetElement(float randomNumber)
    {
        foreach (var element in _lootTable.lootDropList)
        {
            if (randomNumber <= element.chance	)
            {
                return element;
            }

            randomNumber -= element.chance	;
        }

        return default;
    }
    
    private void DropItems(GameObject item,int dropAmount,bool isGuaranteed)
    {
        for (var i = 0; i < dropAmount; i++)
        {
            if (_totalRolls == 0) return;
            if (Application.isPlaying)InstantiateItem(item);
            if(isGuaranteed)_lastRoll.guaranteedItems.Add(item);
            else
            {
                _lastRoll.chanceItems.Add(item);
                _totalRolls--;
            }
        }
    }
    
    private void InstantiateItem(GameObject item)
    {
        var spawnedItem = Instantiate(item, _lootTransform.transform.position,quaternion.identity);
        spawnedItem.AddComponent<ForceBody>();
        var force = _lootForce.itemDropForce.Clone();
        if (_lootTransform != null)
            force.Direction = new Vector3(
                Random.Range(_lootForce.forceDirection.Min.x, _lootForce.forceDirection.Max.x) * (_lootTransform.localScale.x / _lootForce.baseSize.x),
                Random.Range(_lootForce.forceDirection.Min.y, _lootForce.forceDirection.Max.y) * (_lootTransform.localScale.y / _lootForce.baseSize.y),
                0);
        spawnedItem.GetComponent<ForceBody>().Add(force, new CallbackConfig());
    }
}