using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Sayna/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite itemIcon;
    public bool throwable;
}
