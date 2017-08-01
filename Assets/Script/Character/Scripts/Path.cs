using UnityEngine;

public class Path : MonoBehaviour
{
    public bool DebugPath;

    [HideInInspector]
    public Transform[] pathTransformPosition;

    private void Start()
    {
        pathTransformPosition = gameObject.GetComponentsInChildren<Transform>();
    }

    private void OnDrawGizmos()
    {
        if (DebugPath)
        {
            pathTransformPosition = gameObject.GetComponentsInChildren<Transform>();
            Gizmos.color = Color.black;
            for (int i = 0; i < pathTransformPosition.Length; i++)
            {
                if (i >= pathTransformPosition.Length - 1)
                {
                    Gizmos.DrawLine(pathTransformPosition[i].position, pathTransformPosition[0].position);
                    break;
                }

                Gizmos.DrawLine(pathTransformPosition[i].position, pathTransformPosition[i + 1].position);
            }
        }
    }
}
