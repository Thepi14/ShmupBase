using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BasicBullet : Bullet
{
    protected new Collider2D collider;
    protected SpriteRenderer spriteRenderer;

    public float acceleration = 0f;
    public float angularVelocity = 0f;

    protected float currentAcceleration = 0f;

    protected override void Start()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        transform.position += transform.right * (speed + currentAcceleration) * Time.fixedDeltaTime;
        currentAcceleration += acceleration * Time.fixedDeltaTime;

        transform.rotation *= Quaternion.Euler(0f, 0f, angularVelocity * Time.fixedDeltaTime);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hit");
        if (collision == null)
            return;
        if (collision.GetComponent<IHealth>() is var iHealth && iHealth != null)
        {
            iHealth.Damage(damage);
        }
    }
}
