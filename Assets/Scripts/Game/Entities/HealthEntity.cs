using UnityEngine;

public class HealthEntity : Entity, IHealth
{
    [SerializeField]
    protected float health = 5f;
    public float Health { get => health; set => health = value; }

    public void Damage(float damage)
    {
        Health -= damage;
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
