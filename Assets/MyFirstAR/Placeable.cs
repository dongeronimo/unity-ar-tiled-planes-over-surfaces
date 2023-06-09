using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public Vector3 SizeInM = new Vector3(1,1,1);
    public int Id { get; private set; }
    private static int IdCount = 0;
    private void Awake()
    {
        IdCount++;
        Id = IdCount;
    }
}
