using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Global;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    UI_Component[] ui_Components;
    public static UI_Manager instance { get; protected set; }

    BarUI barUI;
    public GameObject deathPanel, winPanel;

    public GameObject abilityButtonHolder;

    ObjectPool pool;
    InfoUI infoUI;
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

    public void AddButtonAction(Action action, string abilityName)
    {
        if (pool == null)
            pool = ObjectPool.instance;
        GameObject button = pool.GetObjectForType("Ability Button", true, abilityButtonHolder.transform.position);
        button.transform.SetParent(abilityButtonHolder.transform, false);
        Button btn = button.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => action());
        btn.gameObject.GetComponentInChildren<TMP_Text>().text = abilityName;
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
        infoUI = (InfoUI)ui_Components[1];
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
    public void HandlePlayerXPLevel(int totalXP, int level)
    {
        barUI.UpdateXPValues(totalXP, level);
    }
}

