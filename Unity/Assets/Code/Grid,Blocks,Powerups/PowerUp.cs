﻿using UnityEngine;
using System.Collections;
using System;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpTypes
    {
        BombRange,
        MoreBombs,
        ExtraLife,
        ExtraTime
    }

    public PowerUpTypes Type;
    public AudioClip PickUpSound;

    public void Awake()
    {
        GameState.Instance.EventHookups.OnGameOver.AddListener(CleanUp);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // Only players can pick up power ups
        if (other.GetComponent<Player>() == null)
            return;

        PowerUpPlayer(other.gameObject);

        AudioSource.PlayClipAtPoint(PickUpSound, transform.position);

        CleanUp();
    }

    private void PowerUpPlayer(GameObject playerObj)
    {
        Bomber bomber = playerObj.GetComponent<Bomber>();
        Health health = playerObj.GetComponent<Health>();
        Player player = playerObj.GetComponent<Player>();

        switch (Type)
        {
            default:
            case PowerUpTypes.BombRange:
                bomber.BonusRange++;
                return;
            case PowerUpTypes.MoreBombs:
                bomber.AvailableBombs++;
                bomber.MaxBombs++;
                return;
            case PowerUpTypes.ExtraLife:
                player.Lives++;
                return;
            case PowerUpTypes.ExtraTime:
                GameTimer.Instance.CurrentTime += GameLogic.Instance.Rules.ExtraTimePowerUp;
                return;
        }
    }

    private void CleanUp()
    {
        // PickUpSound Trigger
        GameObject.Destroy(this.gameObject);
    }
}
