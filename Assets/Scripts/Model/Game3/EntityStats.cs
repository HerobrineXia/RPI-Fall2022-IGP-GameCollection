using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public struct EntityStats
{
    private float _currentHealth;
    private float _maxHealth;
    private float _speed;
    private float _attackDamage;
    private float _attackMultiplier;
    private float _attackReal;
    private float _bulletPerSecond;
    private float _currentCD;
    private float _attackRange;
    private float _attackSpeed;
    private float _armor;
    private float _luck;
    private bool died;

    public EntityStats(float maxHealth, float speed, float attackDamage, float attackMultiplier,
                        float bulletPerSecond, float attackSpeed, float attackRange, float armor, float luck)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _speed = speed;
        _attackMultiplier = attackMultiplier;
        _attackDamage = attackDamage;
        _attackReal = attackMultiplier * attackDamage;
        _bulletPerSecond = bulletPerSecond;
        _currentCD = 0.0f;
        _attackSpeed = attackSpeed;
        _attackRange = attackRange;
        _armor = armor;
        _luck = luck;
        died = false;
    }

    public float maxHealth {
        get { return _maxHealth; }
        set { _maxHealth = Math.Clamp(value, 1.0f, float.MaxValue); currentHealth = Math.Clamp(currentHealth, 0.0f, maxHealth); }
    }

    public float speed {
        get { return Math.Clamp(_speed, 0.5f, 2.0f); }
        set { _speed = value; }
    }

    public float attackMultiplier {
        get { return Math.Clamp(_attackMultiplier, 0.0f, float.MaxValue); }
        set { _attackMultiplier = value; attackReal = attackMultiplier * attackDamage; }
    }

    public float attackDamage {
        get { return Math.Clamp(_attackDamage, 0.0f, float.MaxValue); }
        set { _attackDamage = value; attackReal = attackMultiplier * attackDamage; }
    }

    public float attackReal {
        get { return Math.Clamp(_attackReal, 0.0f, float.MaxValue); }
        set { _attackReal = value; }
    }

    public float bulletPerSecond {
        get { return Math.Clamp(_bulletPerSecond, 0.1f, float.MaxValue); }
        set { _bulletPerSecond = value; currentCD += 0.0f; }
    }

    public float currentCD {
        get { return Mathf.Clamp(_currentCD, 0.0f, float.MaxValue); }
        set { _currentCD = value; }

    }

    public float attackSpeed {
        get { return Math.Clamp(_attackSpeed, 0.1f, float.MaxValue); }
        set { _attackSpeed = value; }
    }

    public float attackRange {
        get { return Math.Clamp(_attackRange, 0.1f, float.MaxValue); }
        set { _attackRange = value; }
    }

    public float armor {
        get { return Math.Clamp(_armor, 0.0f, float.MaxValue); }
        set { _armor = value; }
    }

    public float luck {
        get { return _luck; }
        set { _luck = luck; }
    }

    public float currentHealth {
        get { return _currentHealth; }
        set {
            _currentHealth = Math.Clamp(value, 0.0f, maxHealth);
            if (Mathf.Approximately(currentHealth, 0.0f)) {
                this.died = true;
            }
        }
    }

    public bool CanShoot() {
        return Mathf.Approximately(currentCD, 0.0f);
    }

    public void OnShoot() {
        currentCD = 1.0f / bulletPerSecond;
    }

    public bool IsDead() {
        return died;
    }

    public float OnDamageReceive(float damage) {
        float actualDamage = CalculateDamage(damage);
        currentHealth -= actualDamage;
        return currentHealth;
    }

    private float CalculateDamage(float damage) {
        float actualDamage = 0.0f;
        if (currentHealth != 0) {
            actualDamage = Math.Max(0.1f, damage - _armor);
            actualDamage = Math.Min(currentHealth, actualDamage);
        }
        return actualDamage;
    }
}

