using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPBaseCursor : MonoBehaviour
{
    public static SPBaseCursor Instance;
    public static Vector3 WorldPos {get{return Instance.mousePos;}}
    public static Vector3 GridPos {get{return Instance.gridPos;}}
    public static System.Action<SPBase> OnHoverObject;
    public static System.Action<Vector3> OnGridPosition;
    public static System.Action<Vector3> OnUpdateCursor;

    [Header("Cursor")]
    public bool grid;
    public Transform graphics;
    [SerializeField] Vector3 rawMousePos, mousePos, lastPos;
    [SerializeField] Vector3 gridPos, lastGridPos;
    [SerializeField] SPBase hover, lastHover;


    void Awake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
    }

    void Update()
    {
        UpdateMouse();
    }

    void UpdateMouse()
    {
        lastPos = rawMousePos;
        rawMousePos = SPInput.MouseWorldPos;

        if (grid)
        {

            lastGridPos = gridPos;

            gridPos = new Vector3(Mathf.Round(rawMousePos.x),Mathf.Round(rawMousePos.y),Mathf.Round(rawMousePos.z));
            mousePos = gridPos;

            if(gridPos != lastGridPos) {
                if(OnGridPosition != null)
                    OnGridPosition.Invoke(gridPos);
            }

        }
        else
        {   
            mousePos = Vector3.MoveTowards(graphics.position, rawMousePos, 50f * Time.deltaTime);
        }

        graphics.position = mousePos;   

        if(lastPos != rawMousePos) {
            OnUpdateCursor?.Invoke(rawMousePos);
        }

        if(mousePos != lastPos) {
            UpdateHover();
        }
    }

    void UpdateHover() {

        lastHover = hover;
        // hover = GetEntityFromRadius(mousePos + Vector3.up * .1f,.25f);

        if(lastHover != hover) {
            OnHoverObject?.Invoke(hover);
        }


    }


    Collider[] hits;
    public SPBase GetObjectFromRadius(Vector3 position, float radius) {
        if (hits == null) { hits = new Collider[10]; }

        int amount = Physics.OverlapSphereNonAlloc(position, radius, hits, LayerMask.NameToLayer("Nothing"), QueryTriggerInteraction.Collide);
        int selectedItem = -1;
        float minDistance = 999f;
        SPBase bestItem = null;
        List<SPBase> objects = new List<SPBase>();

        for (int i = 0; i < amount; i++)
        {
            SPBase checkItem = hits[i].GetComponentInParent<SPBase>();

            if (!checkItem)
                continue;

            objects.Add(checkItem);

            float distance = Vector3.Distance(position, hits[i].ClosestPoint(position));
            if (distance < minDistance)
            {
                minDistance = distance;
                selectedItem = i;
                bestItem = checkItem;
            }
        }

        return bestItem;
    }

    public SPBase [] GetObjectsFromRadius(Vector3 position, float radius)
    {

        if (hits == null) { hits = new Collider[10]; }

        int amount = Physics.OverlapSphereNonAlloc(position, radius, hits, LayerMask.NameToLayer("Nothing"), QueryTriggerInteraction.Collide);
        int selectedItem = -1;
        float minDistance = 999f;
        SPBase bestItem = null;
        List<SPBase> entities = new List<SPBase>();

        for (int i = 0; i < amount; i++)
        {
            SPBase checkItem = hits[i].GetComponentInParent<SPBase>();

            if (!checkItem)
                continue;

            entities.Add(checkItem);

            float distance = Vector3.Distance(position, checkItem.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                selectedItem = i;
                bestItem = checkItem;
            }
        }

        // return bestItem;

        return entities.ToArray();

    }
}
