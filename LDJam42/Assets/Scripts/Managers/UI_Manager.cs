using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UI_Manager : MonoBehaviour
{
    UI_Component[] ui_Components;
    public static UI_Manager instance { get; protected set; }

    BarUI barUI;

    private void Awake()
    {
        instance = this;
        SetComponents();
        Init();
    }


    private void SetComponents()
    {
        ui_Components = new UI_Component[]
        {
            new MessageLogUI(),
            new InfoUI(),
            new BarUI("HealthBar")
           // new DynamicUIMenu("InventoryUI"),
            //new DynamicUIMenu("MiniMenuUI")
        };
        barUI = (BarUI)ui_Components[2];
    }

    private void Init()
    {
        for (int i = 0; i < ui_Components.Length; i++)
        {
            ui_Components[i].Init();
        }
    }

    public UI_Component GetUIComponent(string name)
    {
        for (int i = 0; i < ui_Components.Length; i++)
        {
            if (ui_Components[i].ComponentName == name)
                return ui_Components[i];
        }
        return null;
    }

    public void HandlePlayerHealthUI(float current, float max)
    {
        barUI.UpdateBar(current, max);
    }
    public void HandleEnemyHealthUI(float current, float max)
    {

    }
}

