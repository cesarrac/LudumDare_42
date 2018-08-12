using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Prototype", menuName = "Entity-Prototypes/New Prototype")]
public class EntityPrototype : ScriptableObject
{
    public ComponentBlueprint[] components;
    public string Name;
    public EntityType entityType;

    public EntityPrototype(string name, EntityType entityType, ComponentBlueprint[] components)
    {
        this.components = components;
        Name = name;
        this.entityType = entityType;
    }
}
[System.Serializable]
public struct ComponentBlueprint
{
    public string className;
    public ComponentParam[] compParams;

    public ComponentBlueprint(string className, ComponentParam[] compParams)
    {
        this.className = className;
        this.compParams = compParams;
    }
}
[System.Serializable]
public struct ComponentParam
{
    public FieldType fieldType;
    public string value;

    public ComponentParam(FieldType fieldType, string value)
    {
        this.fieldType = fieldType;
        this.value = value;
    }
}
[System.Serializable]
public enum FieldType
{
    STRING, INT, FLOAT
}
