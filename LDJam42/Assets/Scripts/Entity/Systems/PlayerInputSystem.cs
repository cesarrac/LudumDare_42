using UnityEngine;
using System.Collections;
using System;
using Global;
using System.Collections.Generic;

public class PlayerInputSystem : MonoBehaviour
{
    public static PlayerInputSystem instance { get; protected set; }
    Action<MoveData> OnMoveInput;
    Action<KeyCode> OnKeyInput;
    Vector2 lastV2;
    InputState inputState;
    List<DynamicKeyAction> dynamicKeyActions;

    Vector2Int mousePos, lastMousePos;
    MapManager mapManager;
    InfoUI infoUI;
    GameObject canvas;

    bool canFindTile = false;

    private void Awake()
    {
        instance = this;
        dynamicKeyActions = new List<DynamicKeyAction>();
        Global.OnTurnChange.RegisterListener(OnTurnChanged);
        mousePos = lastMousePos = Vector2Int.zero;
        Global.OnMapCleared.RegisterListener(OnNOMap);
        Global.OnMapCreated.RegisterListener(OnMapStart);
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    

    private void OnMapStart(OnMapCreated data)
    {
        canFindTile = true;
    }

    private void OnNOMap(OnMapCleared data)
    {
        canFindTile = false;
    }

    private void Start()
    {
        mapManager = MapManager.instance;
        infoUI = (InfoUI)UI_Manager.instance.GetUIComponent("InfoUI");
    }
    private void OnTurnChanged(OnTurnChange data)
    {
        if (data.newTurnState == TurnState.Player)
        {
            inputState = InputState.Default;
        }
        else
        {
            inputState = InputState.Off;
        }
    }

    private void Update()
    {
        if (inputState == InputState.Off)
            return;
        Vector2 inputV2 = Vector2.zero;
        if (Input.GetButtonDown("DiagTopLeft"))
        {
            inputV2 = Vector2.up + Vector2.left;
        }
        else if (Input.GetButtonDown("DiagTopRight"))
        {
            inputV2 = Vector2.up + Vector2.right;
        }
        else if (Input.GetButtonDown("DiagBottomRight"))
        {
            inputV2 = Vector2.down + Vector2.right;
        }
        else if (Input.GetButtonDown("DiagBottomLeft"))
        {
            inputV2 = Vector2.down + Vector2.left;
        }
        else
        {
            inputV2 = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        

        if (inputV2 != lastV2)
        {
            lastV2 = inputV2;
            if (inputV2 == Vector2.zero)
                return;
            if (OnMoveInput != null)
            {
                OnMoveInput(new MoveData(inputV2.x, inputV2.y));
            }
        }

        if (dynamicKeyActions.Count > 0)
        {
            for (int i = 0; i < dynamicKeyActions.Count; i++)
            {
                if (Input.GetKeyDown(dynamicKeyActions[i].dynamicKey))
                {
                    dynamicKeyActions[i].action();
                }
            }
        }


        if (canFindTile == false)
            return;
        mousePos = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (mousePos != lastMousePos)
        {
            lastMousePos = mousePos;
            infoUI.Clear();
            MapTile tile = mapManager.Map.GetTile(mousePos.x, mousePos.y);
            if (tile == null)
            {
                infoUI.DeActivate();
                return;
            }
            if (tile.tileType == TileType.Darkness)
            {
                infoUI.Activate();
                
                infoUI.UpdateTexts(new Message[] { new Message("Poison", Color.magenta), new Message("100%", Color.red), new Message("Lethal!", Color.red) });
                infoUI.UpdatePosition(mousePos + Vector2.left * 2, canvas);
                return;
            }
            if (tile.tileType == TileType.SemiDark)
            {
                infoUI.Activate();

                infoUI.UpdateTexts(new Message[] { new Message("Poison", Color.magenta), new Message("50%", Color.yellow), new Message("Dangerous!", Color.yellow)});
                infoUI.UpdatePosition(mousePos + Vector2.left * 2, canvas);
                return;
            }

            if (tile.entities.Count <= 0)
            {
                infoUI.DeActivate();
                return;
            }
            else
            {
                Message[] info = new Message[3];
                if (tile.entities[0].entityType == EntityType.Unit)
                {
                    FighterComponent fighter = (FighterComponent)tile.entities[0].GetEntityComponent(ComponentID.Fighter);
                    info[0] = new Message(fighter.thisEntity.Name, Color.white);
                    info[1] = new Message(fighter.GetAttackPower().ToString(), Color.red);
                    info[2] = new Message(fighter.GetDefensePower().ToString(), Color.cyan);
                }
                else if (tile.entities[0].entityType == EntityType.Item)
                {
                    ItemComponent item = (ItemComponent)tile.entities[0].GetEntityComponent(ComponentID.Item);
                    info[0] = new Message(item.itemName, Color.white);
                    if (item.itemType == ItemType.Weapon)
                    {
                        WeaponComponent wpn = (WeaponComponent)tile.entities[0].GetEntityComponent(ComponentID.Weapon);
                        info[1] = new Message(wpn.weaponAttackStats.AttackPower.ToString(), Color.red);
                        info[2] = new Message(wpn.weaponAttackStats.DefensePower.ToString(), Color.cyan);
                    }
                    else if (item.itemType == ItemType.Armor)
                    {
                        ArmorComponent armor = (ArmorComponent)tile.entities[0].GetEntityComponent(ComponentID.Armor);
                        info[1] = new Message(armor.armorAttackStats.AttackPower.ToString(), Color.red);
                        info[2] = new Message(armor.armorAttackStats.DefensePower.ToString(), Color.cyan);
                    }
                    else
                    {
                        HealthDropComponent consumable = (HealthDropComponent)tile.entities[0].GetEntityComponent(ComponentID.Consumable);
                        info[1] = new Message(consumable.HealthGained.ToString(), Color.cyan);
                    }
                }
                infoUI.Activate();
                infoUI.UpdateTexts(info);
                infoUI.UpdatePosition(mousePos + Vector2.left * 2, canvas);
            }
        }
    }

    public void RegisterOnMoveInputCB(Action<MoveData> cb)
    {
        OnMoveInput += cb;
    }
    public void UnRegisterOnMoveInputCB(Action<MoveData> cb)
    {
        OnMoveInput += cb;
    }
    public void AddDynamicKeys(Action action, string key)
    {
        
        dynamicKeyActions.Add(new DynamicKeyAction(action, key));
    }
    //public void RegisterKeyInputCB(Action<KeyCode> cb)
    //{
    //    OnKeyInput += cb;
    //}
    //public void UnRegisterKeyInputCB(Action<KeyCode> cb)
    //{
    //    OnKeyInput -= cb;
    //}
}
public enum InputState
{
    Off, Default
}

public struct MoveData
{
    private readonly float x;
    private readonly float y;

    public MoveData(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public float X
    {
        get
        {
            return x;
        }
    }

    public float Y
    {
        get
        {
            return y;
        }
    }
}

public class DynamicKeyAction
{
    public Action action;
    public string dynamicKey;

    public DynamicKeyAction(Action action, string dynamicKey)
    {
        this.action = action;
        this.dynamicKey = dynamicKey;
    }
}