using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    private void Start()
    {
        // Automatically adjusts camera position and distance based on grid size.
        transform.position =
            new Vector3(((float)BoardSetup.Instance.Width + 1) / 2,
                Mathf.Max(BoardSetup.Instance.Width, BoardSetup.Instance.Height),
                ((float)BoardSetup.Instance.Height + 1) / 2);
    }
}
