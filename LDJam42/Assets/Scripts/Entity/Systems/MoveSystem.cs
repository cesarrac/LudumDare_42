using UnityEngine;
using System.Collections;

public class MoveSystem : MonoBehaviour
{
    public static MoveSystem instance { get; protected set; }

    private void Awake()
    {
        instance = this;
    }

    public void ChangePosition(MoveData positionData, Transform transform)
    {
        transform.position = new Vector2(positionData.X, positionData.Y);
    }
}
