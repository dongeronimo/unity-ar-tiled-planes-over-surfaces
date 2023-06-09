using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField]
    GameObject spawnablePrefab;
    Camera arCam;
    GameObject spawnedObject;
    bool isSimulation;
    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        isSimulation = GameObject.Find("SimulationCamera") != null;
        Debug.Log($"isSimulation = {isSimulation}");
    }

    // Update is called once per frame
    void Update()
    {
        if (isSimulation)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                //O raio saindo da câmera
                Ray ray = arCam.ScreenPointToRay(Input.mousePosition);
                if (m_RaycastManager.Raycast(Input.mousePosition, m_Hits))
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        SpawnPrefab(m_Hits[0].pose.position);
                    }
                }
            }
        }
        else
        {
            if (Input.touchCount == 0) return;
            RaycastHit hit;
            //O raio saindo da câmera
            Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);
            //Faz um raio pra tocar em trackables como planos.
            if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
            {
                //Se comecei o touch
                if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        //O touch tocou no objeto?
                        if (hit.collider.gameObject.tag == "Spawnable")
                            spawnedObject = hit.collider.gameObject;
                        else
                            SpawnPrefab(m_Hits[0].pose.position);
                    }
                }
                //Se estou arrastando o touch
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
                {
                    spawnedObject.transform.position = m_Hits[0].pose.position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    spawnedObject = null;
                }
            }
        }
    }

    private void SpawnPrefab(Vector3 position)
    {
        spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);
    }
}
