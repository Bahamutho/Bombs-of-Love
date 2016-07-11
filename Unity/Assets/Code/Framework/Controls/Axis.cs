﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

[System.Serializable]
public class Axis //: Control
{
    public string Name;
    
    public List<AxisKey> AxisKeys;

    [SerializeField]
    private int lastAxis;

    [SerializeField]
    private ControlType lastInputType = ControlType.PC;
    private int xbox;

    public Axis(int xbox = 0, string name = "defaultAxis")
    {
        AxisKeys = new List<AxisKey>();
        this.xbox = 0;
        this.Name = name;
    }

    public float Value()
    {
        float value = 0;
        int curAxis = 0;

        // Handles priority on last active 
        for(int i = 0; i < AxisKeys.Count; i++)
        {
            float v = AxisKeys[i].Value(xbox);
            if (v != 0 && (value == 0 || (value != 0 && lastAxis == i)))
            {
                value = v;
                curAxis = i;
                lastInputType = ControlHelper.AxisKeyToControl(AxisKeys[i].Type);
            } 
        }
        lastAxis = curAxis;
        return value;
    }

    public void XboxAxis(XboxAxis axis)
    {
        XboxAxis(axis.ToString());
    }

    public void XboxAxis(string axis)
    {
        AxisKeys.Add(AxisKey.XboxAxis(axis));
    }

    public void XboxDpad(AxisKey.HorVert horizontalOrVertical)
    {
        AxisKeys.Add(AxisKey.XboxDpad(horizontalOrVertical));
    }

    public void PC(string neg, string pos)
    {
        AxisKeys.Add(AxisKey.PC(neg,pos));
    }

    public void PC(KeyCode neg, KeyCode pos)
    {
        AxisKeys.Add(AxisKey.PC(neg, pos));
    }

    /// <summary>
    /// Setup wasd, arrows, dpad, thumbstick
    /// </summary>
    /// <param name="horintalOrVertical"></param>
    /// <param name="xbox"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Axis Default(AxisKey.HorVert horintalOrVertical, int xbox = 0, string name = "")
    {
        Axis ax = new Axis(xbox, name);
        switch(horintalOrVertical)
        {
            case AxisKey.HorVert.Horizontal:
                ax.PC("D", "A");
                ax.PC(KeyCode.RightArrow, KeyCode.LeftArrow);
                ax.XboxAxis(global::XboxAxis.LeftX);
                ax.XboxDpad(AxisKey.HorVert.Horizontal);
                break;
            case AxisKey.HorVert.Vertical:
                ax.PC("W", "S");
                ax.PC(KeyCode.UpArrow, KeyCode.DownArrow);
                ax.XboxAxis(global::XboxAxis.LeftY);
                ax.XboxDpad(AxisKey.HorVert.Vertical);
                break;
            default:
                break;
        }

        return ax;
    }
}

