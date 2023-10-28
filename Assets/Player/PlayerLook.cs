using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.movement
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] float mouseSens = 100f;
        [SerializeField] GameObject uiInventory = null;
        Transform playerBody;
        float xRotation = 0f;


        void Start()
        {

            playerBody = GetComponentInParent<Rigidbody>().transform;
        }

        private void Update()
        {
            /*if (uiInventory.active == false)
            {*/
            Cursor.lockState = CursorLockMode.Locked;
            float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime * 100;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime * 100;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
            /*}
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }*/


        }
    }
}