using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance;
    
    public List<ButtonKey> redKeys = new ();
    public List<ButtonKey> greenKeys = new ();
    public List<ButtonKey> blueKeys = new ();
    public List<ButtonKey> yellowKeys = new ();

    private Dictionary<KeyCode, ButtonKey> _mapping = new ();

    [Serializable]
    public class ButtonKey
    {
        public enum AttackType
        {
            Melee,
            Ranged
        }
        
        public KeyCode KeyCode;
        public AttackType Type;
        public PlayerController.ButtonColor Color;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        foreach (var buttonKey in redKeys) { _mapping[buttonKey.KeyCode] = buttonKey; }
        foreach (var buttonKey in greenKeys) { _mapping[buttonKey.KeyCode] = buttonKey; }
        foreach (var buttonKey in blueKeys) { _mapping[buttonKey.KeyCode] = buttonKey; }
        foreach (var buttonKey in yellowKeys) { _mapping[buttonKey.KeyCode] = buttonKey; }
    }

    public ButtonKey GetKey(String keyString)
    {
        KeyCode parsed;
        if (!KeyCode.TryParse(keyString, out parsed))
        {
            return null;
        }
        return _mapping[parsed];
    }
}
