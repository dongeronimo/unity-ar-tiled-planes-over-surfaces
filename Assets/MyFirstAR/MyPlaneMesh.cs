using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;
using Unity.Collections;

[RequireComponent(typeof(ARPlane))]
public class MyPlaneMesh : MonoBehaviour
{

    [SerializeField]
    Placeable testeFloorPrefab;
    public Transform ObjectRoot;
    ARPlane m_ArPlane;
    [SerializeField]
    float uniformScale = 0.01f;
    Color m_Color;
    // Start is called before the first frame update
    void Start()
    {
        m_Color = Random.ColorHSV();
        m_ArPlane = GetComponent<ARPlane>();
        m_ArPlane.boundaryChanged += OnBoundaryChanged;
        Debug.Log("Filling sphere cache");
        for(var i=0; i<30; i++)
        {
            Transform obj = Instantiate(original: spherePrefab, parent: ObjectRoot);
            obj.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
            obj.GetComponent<MeshRenderer>().material.color = m_Color;
            obj.gameObject.SetActive(false);
            borderSpheres.Add(obj.gameObject);
        }
        Debug.Log("Sphere cache filled");
        Debug.Log("Filling tiles cache");
        for(var i=0; i<3000; i++)
        {
            Transform obj = Instantiate(original: tilePrefab, parent: ObjectRoot);
            obj.transform.localScale = new Vector3 (uniformScale, uniformScale, uniformScale);
            obj.GetComponent<MeshRenderer>().material.color= m_Color;
            obj.gameObject.SetActive(false);
            tiles.Add(obj.gameObject);
        }
        Debug.Log("Tile cache filled");
    }
    void OnEnable()
    {
        try
        {
            m_ArPlane.boundaryChanged += OnBoundaryChanged;
            OnBoundaryChanged(default(ARPlaneBoundaryChangedEventArgs));
        }
        catch { }
    }
    public Transform spherePrefab;
    private List<GameObject> borderSpheres = new List<GameObject>();
    public Transform tilePrefab;
    private List<GameObject> tiles = new List<GameObject>();
    void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        //Evaluates whether the plane is horizontal. If it isn't, return;
        //bool planeIsHorizontal = Mathf.Abs(Vector3.Dot(m_ArPlane.normal, Vector3.up) - 1f) < 0.0001f;
        //if (!planeIsHorizontal)
        //    return;
        //draw the boundary spheres
        NativeArray<Vector2> boundary = m_ArPlane.boundary;
        borderSpheres.ForEach(s => s.SetActive(false));
        var queue = new Queue<GameObject>(borderSpheres);
        boundary.Select(p_BoundaryPoint => {
                return new Vector3(p_BoundaryPoint.x, 0, p_BoundaryPoint.y);
            })
            .ToList()
            .ForEach(pos => {
                GameObject obj = queue.Dequeue();
                obj.SetActive(true);
                obj.transform.localPosition = pos;
            });
        //Draw the tiles
        tiles.ForEach(s=>s.SetActive(false));
        int counter = 0;
        Vector2 p_TopRight = new Vector2(m_ArPlane.extents.x, m_ArPlane.extents.y);
        Vector2 p_BottomLeft = new Vector2(-m_ArPlane.extents.x, -m_ArPlane.extents.y);
        Vector2 p_Cursor = p_TopRight;
        while(p_Cursor.y > p_BottomLeft.y)
        {
            p_Cursor.x = p_TopRight.x;
            while(p_Cursor.x > p_BottomLeft.x)
            {
                p_Cursor.x = p_Cursor.x - 1 * uniformScale; //For now i assume that the tile is 1x1
                if (IsPointInPolygon(p_Cursor, boundary)){
                    GameObject obj = tiles[counter];
                    obj.SetActive(true);
                    obj.transform.localPosition = new Vector3(p_Cursor.x, 0, p_Cursor.y);
                    counter++;
                }
            }
            p_Cursor.y = p_Cursor.y - 1 * uniformScale; //For now i assume that the tile is 1x1
        }
    }


    public static bool IsPointInPolygon(Vector2 point, NativeArray<Vector2> polygon)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;
        float pointX = point.x, pointY = point.y;
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength - 1];

        float x = pointX;
        float y = pointY;

        for (i = 0; i < polygonLength; i++)
        {
            startX = endPoint.x;
            startY = endPoint.y;
            endPoint = polygon[i];
            endX = endPoint.x;
            endY = endPoint.y;

            if ((startY < y && endY >= y || endY < y && startY >= y) && (startX <= x || endX <= x))
            {
                if (startX + (y - startY) / (endY - startY) * (endX - startX) < x)
                {
                    inside = !inside;
                }
            }
        }

        return inside;
    }
}
