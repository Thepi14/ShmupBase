using UnityEngine;

public abstract class Entity : MonoBehaviour
{

}

public interface IHealth
{
    public float Health { get; set; }
    public void Damage(float damage);
}