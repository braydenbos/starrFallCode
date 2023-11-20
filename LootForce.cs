using UnityEngine;

[CreateAssetMenu(menuName = "Create LootForceData")]
public class LootForce : ScriptableObject
{
    public MinMax<Vector2> forceDirection;
    public Force itemDropForce;
    public Vector2 baseSize;
}
