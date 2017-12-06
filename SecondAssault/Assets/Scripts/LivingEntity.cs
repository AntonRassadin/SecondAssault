using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable {

    public event Action OnDeath;
    [SerializeField]
    protected float startingHealth = 100f;

    protected float health;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void Die()
    {

        if (OnDeath != null)
        {
            OnDeath();
        }
    }
}
