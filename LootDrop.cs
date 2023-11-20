using UnityEngine;

public class LootDrop : MonoBehaviour
{
    [SerializeField] public LootTable lootTable;
    [SerializeField] public LootForce lootForce;
    public void DropLoot()
    {
        LootSystem.Instance.ChooseLoot(lootTable,lootForce,transform);
    }
}
