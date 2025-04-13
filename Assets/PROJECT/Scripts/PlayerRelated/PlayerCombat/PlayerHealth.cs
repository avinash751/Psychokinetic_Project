using System;
using UnityEngine;

public class PlayerHealth : ObjectHealth
{
    public static Action OnPlayerDeath;
    public static Action<int,GameObject> OnPlayerHealthInitialized;
    string key = "PlayerDamage";

    private void Start()
    {
        OnPlayerHealthInitialized?.Invoke(MaxHealth, gameObject);
    }
    
    protected override void OnTakingDamage()
    {
        AudioManager.Instance?.PlayAudio(key);
    }

    protected override void ObjectDeath()
    {
        OnPlayerDeath?.Invoke();
    }
}
