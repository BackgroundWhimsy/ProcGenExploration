using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 cameraOffset = new Vector3(8, 17, -3);

    public void MoveCamera(Vector3 tilePos)
    {
        transform.position = tilePos + cameraOffset;
    }
}
