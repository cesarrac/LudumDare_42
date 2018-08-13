using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;
public abstract class UI_Component 
{
    public string ComponentName;
    public GameObject mainGO;

    protected UI_Component(string componentName)
    {
        ComponentName = componentName;
    }

    public virtual void Init()
    {
    }

    public virtual void Activate()
    {
        if (mainGO == null)
            return;
        mainGO.SetActive(true);
    }

    public virtual void DeActivate()
    {
        if (mainGO == null)
            return;
        mainGO.SetActive(false);
    }

}
public class BarUI : UI_Component
{
    Slider healthBar;
    TMP_Text valueLabel;

    public BarUI(string componentName) : base(componentName)
    {
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void DeActivate()
    {
        base.DeActivate();
    }

    public override void Init()
    {
        mainGO = GameObject.FindGameObjectWithTag(ComponentName).gameObject;
        if (mainGO == null)
        {
            Debug.LogError(ComponentName + " could not find UI gobj tagged: '" + ComponentName + "' -- is this not the correct tag? CHECK CANVAS!");
            return;
        }
        healthBar = mainGO.GetComponent<Slider>();
        valueLabel = mainGO.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
    }
    public void UpdateBar(float curValue, float maxValue)
    {
        healthBar.value = Mathf.Clamp01(curValue / maxValue);
        valueLabel.text = curValue.ToString("f0") + " / " + maxValue.ToString("f0");
    }
}


public class TextBasedUI : UI_Component
{
    public TMP_Text[] textObjs;

    protected TextBasedUI(string componentName) : base(componentName)
    {
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void DeActivate()
    {
        base.DeActivate();
    }

    public override void Init()
    {
        mainGO = GameObject.FindGameObjectWithTag(ComponentName).gameObject;
        if (mainGO == null)
        {
            Debug.LogError(ComponentName + " could not find UI gobj tagged: '" + ComponentName + "' -- is this not the correct tag? CHECK CANVAS!");
            return;
        }
        textObjs = new TMP_Text[mainGO.transform.childCount];
        //Debug.Log(ComponentName + " GO has " + textObjs.Length + " children");
        for (int i = 0; i < textObjs.Length; i++)
        {
            textObjs[i] = mainGO.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
            textObjs[i].text = string.Empty;
        }
    }
    public virtual void Clear()
    {
        for (int i = 0; i < textObjs.Length; i++)
        {
            textObjs[i].text = string.Empty;
        }
    }
    public virtual void UpdateTexts(Message[] messages)
    {
        if (messages.Length <= 0 || messages.Length > textObjs.Length)
        {
            Debug.LogError(ComponentName + " has received an array that does not match the length of its UI Text objects array!");
            return;
        }
        for (int i = 0; i < messages.Length; i++)
        {
            textObjs[i].text = messages[i].messageText;
            textObjs[i].color = messages[i].color;
        }
    }
}

public class MessageLogUI : TextBasedUI
{

    public MessageLogUI() : base("MessageLog")
    {

    }
    
    public override void UpdateTexts(Message[] messages)
    {
        base.UpdateTexts(messages);
    }
}
public class InfoUI : TextBasedUI
{
    public InfoUI() : base("InfoUI")
    {
    }
    public void UpdatePosition(Vector2 pos, GameObject canvas)
    {
        mainGO.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(pos);
    }
    public override void UpdateTexts(Message[] messages)
    {
        base.UpdateTexts(messages);
    }
}


//public class DynamicUIMenu : UI_Component
//{
//    public string[] inputKeyNames { get; protected set; }
//    Text labelTextObj;
//    ObjectPool pool;
//    UIBUtton[] uiButtons;
//    Button closeButton;

//    public DynamicUIMenu(string componentName) : base(componentName){}

//    public override void DeActivate()
//    {
//        base.DeActivate();
//        labelTextObj.transform.parent.gameObject.SetActive(false);
        
//    }
//    public override void Activate()
//    {
//        base.Activate();
//        labelTextObj.transform.parent.gameObject.SetActive(true);
//    }

//    public override void Init()
//    {
//        mainGO = GameObject.FindGameObjectWithTag(ComponentName).gameObject;
//        if (mainGO == null)
//        {
//            Debug.LogError(ComponentName + " could not find UI gobj tagged: '" + ComponentName + "' -- is this not the correct tag? CHECK CANVAS!");
//            return;
//        }

//        if (mainGO.transform.parent.childCount <= 1) return;
//        labelTextObj = mainGO.transform.parent.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>();
//        closeButton = mainGO.transform.parent.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Button>();
//        closeButton.onClick.RemoveAllListeners();
//        closeButton.onClick.AddListener(() => CloseButtonAction());
//        DeActivate();
//    }
//    void CloseButtonAction()
//    {
//        DeActivate();
//        //Global.InputChangeEvent newInput = new Global.InputChangeEvent();
//        //newInput.newState = InputState.DEFAULT;
//        //newInput.FireEvent();
//    }

//    public void SetLabel(string label)
//    {
//        labelTextObj.text = label;
//    }

//    public void LoadMenu(string label, Message[] bttnTxtContents, Action<int>[] onClickActions, string[] inputKeys, Action closeBtnAction) // array of values to get button data from, action for buttons to listen for
//    {
//        if (mainGO.activeSelf == false)
//            return;
//        if (pool == null)
//            pool = ObjectPool.instance;

//        labelTextObj.text = label;
//        inputKeyNames = inputKeys;

//        if (closeBtnAction != null)
//        {
//            closeButton.onClick.RemoveAllListeners();
//            closeButton.onClick.AddListener(() => closeBtnAction());
//        }

//        uiButtons = new UIBUtton[bttnTxtContents.Length];
//        for (int i = 0; i < uiButtons.Length; i++)
//        {
//            // Instantitate the button gobj
//            GameObject btnGO = pool.GetObjectForType("UI Slot", true, Camera.main.WorldToScreenPoint(mainGO.transform.position));
//            if (btnGO == null)
//            {
//                Debug.LogError("The pool ran out of UIButton prefabs!");
//                return;
//            }
//            btnGO.transform.SetParent(mainGO.transform);
//            Button btn = btnGO.GetComponent<Button>();
//            btn.targetGraphic = btnGO.GetComponent<Image>();
//            int index = i;
//            Action<int> btnAction;
//            if (index >= onClickActions.Length)
//                btnAction = onClickActions[0];
//            else
//                btnAction = onClickActions[index];

//            btn.onClick.RemoveAllListeners();

//            if (btnAction != null)
//            {
//                btn.onClick.AddListener(() => btnAction(index));
//            }
            
//            Text btnText = btnGO.transform.GetChild(1).gameObject.GetComponent<Text>();
//            btnText.text = bttnTxtContents[i].messageText;
//            btnText.color = bttnTxtContents[i].color;
//            Text keyText = btnGO.transform.GetChild(0).gameObject.GetComponent<Text>();
//            keyText.text = inputKeyNames[i] + ") ";
//            uiButtons[i] = new UIBUtton(btn, btnText, keyText);
//        }
//    }

//    public void OnKeyPressed(string key)
//    {
//        for (int i = 0; i < inputKeyNames.Length; i++)
//        {
//            if (inputKeyNames[i] == key)
//            {
//                // Press the button that matches this index
//                // firing its event
//                if (i >= uiButtons.Length)
//                    return;
//                uiButtons[i].Fire();
//            }
//        }
//    }

//    public void Clear()
//    {
//        if (uiButtons == null)
//            return;
//        if (uiButtons.Length <= 0)
//            return;
//        for (int i = 0; i < uiButtons.Length; i++)
//        {
//            uiButtons[i].ButtonObj.onClick.RemoveAllListeners();
//            uiButtons[i].TextObj.text = string.Empty;
//            uiButtons[i].TextObj.color = Color.white;
//            uiButtons[i].InputKeyText.text = string.Empty;
//            GameObject GO = uiButtons[i].ButtonObj.gameObject;
//            GO.transform.SetParent(null);
//            pool.PoolObject(GO);
//        }
//        uiButtons = null;
//    }
//}

//public struct UIBUtton
//{
//    private readonly Button buttonObj;
//    private readonly Text textObj;
//    private readonly Text inputKeyText;

//    public UIBUtton(Button buttonObj, Text textObj, Text inputKeyText)
//    {
//        this.buttonObj = buttonObj;
//        this.textObj = textObj;
//        this.inputKeyText = inputKeyText;
//    }

//    public Button ButtonObj
//    {
//        get
//        {
//            return buttonObj;
//        }
//    }

//    public Text TextObj
//    {
//        get
//        {
//            return textObj;
//        }
//    }

//    public Text InputKeyText
//    {
//        get
//        {
//            return inputKeyText;
//        }
//    }

//    public void Fire()
//    {
//        buttonObj.onClick.Invoke();
//    }
//}