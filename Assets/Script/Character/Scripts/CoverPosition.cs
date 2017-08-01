using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider))]
public class CoverPosition : MonoBehaviour
{
    public Collider m_Collider;
    [Tooltip("For placing the transform in the floor")]
    public LayerMask m_LayerMask;
    //[HideInInspector]
    public bool m_isOccupied = false;

    private void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_Collider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("CoverPosition");
        m_isOccupied = false;

        RaycastHit hitInfo;

        if(Physics.Raycast(transform.position, Vector3.down, out hitInfo, 5f, m_LayerMask))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            Debug.Log("Cover Position is Detroyed" + this.gameObject.name);
            Debug.DrawRay(transform.position, Vector3.up * 100f, Color.blue);
            DestroyImmediate(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
