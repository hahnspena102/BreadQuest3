using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
public abstract class EntityData : ScriptableObject
{
    [SerializeField] private float health;
    [SerializeField] private float speed;

    public global::System.Single Health { get => health; set => health = value; }
    public global::System.Single Speed { get => speed; set => speed = value; }
}
