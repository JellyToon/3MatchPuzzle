using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public void InitPosition(float x, float y, float z)
    {
        this.transform.position = new Vector3(x, y, z);
    }
}
