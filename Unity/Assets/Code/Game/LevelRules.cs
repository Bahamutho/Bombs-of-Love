﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelRules : ScriptableObject
{
    public int Lives = 3;
    public float RoundTime = 60;
    public bool LoseOnTimeUp = false;
    public bool SuddenDeathOnTimeUp = false;
    public bool RandomBlockStart = true;
    public bool LoseUpgradesOnNewRound = true;
    public int StartBombs = 1;
    public int StartBonusRange = 0;
    public int StartBonusDamage = 0;
    public float ExtraTimePowerUp = 5.0f;
    public List<PowerUpDropChance> PowerUps;
}
