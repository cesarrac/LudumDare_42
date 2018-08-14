using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Global;

public class UI_Manager : MonoBehaviour
{
    UI_Component[] ui_Components;
    public static UI_Manager instance { get; protected set; }

    BarUI barUI;
    public GameObject deathPanel, winPanel;

    public Button UseAbilityButton;

    private void Awake()
    {
        instance = this;
        SetComponents();
        Init();
        Global.OnMapCreated.RegisterListener(OnMapStart);
        Global.PlayerDeath.RegisterListener(OnPlayerDeath);
        Global.GameWin.RegisterListener(OnGameWin);
    }

    private void OnGameWin(GameWin data)
    {
        winPanel.SetActive(true);
    }

    private void OnMapStart(OnMapCreated data)
    {
        if (deathPanel == null)
            deathPanel = GameObject.FindGameObjectWithTag("DeathPanel");

        deathPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void AddButtonAction(Action action)
    {
        UseAbilityButton.onClick.RemoveAllListeners();
        UseAbilityButton.onClick.AddListener(() => action());
    }

    private void OnPlayerDeath(PlayerDeath data)
    {
        deathPanel.SetActive(true);
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

