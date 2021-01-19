using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Transform pivotCamera;
    [SerializeField][Range(0.01f,2)] private float mouseSensibility;
    [SerializeField] private float playerSpeed;

    [SerializeField] private bool playerRotation;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerRotation)
        {
            transform.localRotation = Quaternion.Euler(0, -Input.mousePosition.x * mouseSensibility, 0);
            pivotCamera.localRotation = Quaternion.Euler(Input.mousePosition.y * mouseSensibility
                , 0, 0);
        }
        else
        {
            pivotCamera.localRotation = Quaternion.Euler(Input.mousePosition.y * mouseSensibility
                , -Input.mousePosition.x * mouseSensibility, 0);
        }

        Camera.main.transform.position += (Vector3) Input.mouseScrollDelta * 0.1f;
    }
}
