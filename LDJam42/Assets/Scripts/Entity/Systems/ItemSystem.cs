using UnityEngine;
using System.Collections;

public class ItemSystem : MonoBehaviour
{
    public static ItemSystem instance { get; protected set; }
    private void Awake()
    {
        instance = this;
    }
    public void OnItemActiveChanged(Entity itemEntity, GameObject go, bool isActive, MoveData positionData)
    {
        if (isActive == false)
        {
            EntityActionManager.instance.EntityOnTileChanged(itemEntity, positionData);
            if (go == null)
                return;
            go.SetActive(false);
        }
        else
        {
            if (go == null)
            {
                // Spawn a new one
                go = EntityManager.instance.SpawnEntityGO(itemEntity, positionData);
                // Init Item component with new go
                ItemComponent item = (ItemComponent)itemEntity.GetEntityComponent(ComponentID.Item);
                item.Init(itemEntity, go);
            }
            go.SetActive(true);
            go.transform.position = new Vector2(positionData.X, positionData.Y);
            EntityActionManager.instance.EntityOnTileChanged(itemEntity, positionData);
        }
    }
   
}
