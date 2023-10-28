using UnityEngine;

namespace Player.movement
{
    public class PlayerMovement : MonoBehaviour
    {
        CharacterController charControl;
        [SerializeField] float speed = 3f;
        public float gravity = -9.81f;
        public float jumpHight = 2;

        Vector3 velocity;
        public bool isGrounded;

        private void Awake()
        {
            charControl = GetComponent<CharacterController>();

        }
        private void FixedUpdate()
        {
            isGrounded = charControl.isGrounded;
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;
            charControl.Move(move * Time.deltaTime * speed);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            charControl.Move(velocity * Time.deltaTime);
        }

    }
}