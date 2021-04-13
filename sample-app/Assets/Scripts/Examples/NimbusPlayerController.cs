using UnityEngine;

namespace Examples {
    public class NimbusPlayerController : MonoBehaviour {
        public FixedJoystick joystick;
        public float movementSpeed = 5.0f;
        public float jumpPower = 10f;
        public Vector2 groundColliderSize = new Vector2(0.3f, 0.5f);
        public Vector2 groundColliderOffset;
        public LayerMask whatIsGround;


        private Rigidbody2D _mRb;
        private Vector3 _mVelocity = Vector3.zero;
        private float _velocity;
        private bool _isGrounded;
        private readonly Collider2D[] _groundHit = new Collider2D[1];

        private void Awake() {
#if !UNITY_EDITOR
   Screen.orientation = ScreenOrientation.LandscapeRight;
#endif
        }

        private void Start() {
            _mRb = GetComponent<Rigidbody2D>();
        }
        
        private void Update() {
#if UNITY_EDITOR 
            _velocity = Input.GetAxis("Horizontal");        
#else
            _velocity = joystick.Horizontal;
#endif
        }
        
        private void FixedUpdate() {
            HorizontalMovement();
            GroundCheck();
        }

        private void HorizontalMovement() {
            var velocity = _mRb.velocity;
            var targetVelocity = new Vector2(_velocity  * movementSpeed, velocity.y);
            _mRb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref _mVelocity, .0001f);
        }
        
        private void GroundCheck() {
            var position = transform.position;
            var result = Physics2D.OverlapBoxNonAlloc(
                new Vector2(position.x + groundColliderOffset.x, position.y + groundColliderOffset.y),
                groundColliderSize, 0f, _groundHit,whatIsGround);
            _isGrounded = (result == 1);
        }
        

        public void Jump() {
            if (!_isGrounded) return;
            _mRb.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
        }
        
        private void OnDrawGizmosSelected() {
            var position = transform.position;
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(new Vector2(position.x + groundColliderOffset.x, position.y + groundColliderOffset.y),
                groundColliderSize);
        }
        
    }
}
