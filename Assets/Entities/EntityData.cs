using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
public abstract class EntityData : ScriptableObject
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float speed;

    public global::System.Single MaxHealth { get => maxHealth; set => maxHealth = value; }
    public global::System.Single Speed { get => speed; set => speed = value; }
}
