using UnityEngine;
using UnityEngine.SceneManagement;

namespace Example.Scripts.NotAdRelated {
	public class NimbusPlayerController : MonoBehaviour {
		[SerializeField] private FixedJoystick _joystick;
		[SerializeField] private float _movementSpeed = 5.0f;
		[SerializeField] private float _jumpPower = 10f;
		[SerializeField] private Vector2 _groundColliderSize = new Vector2(0.3f, 0.5f);
		[SerializeField] private Vector2 _groundColliderOffset;
		[SerializeField] private LayerMask _whatIsGround;
		
		private readonly Collider2D[] _groundHit = new Collider2D[1];
		private bool _isGrounded;
		
		private Rigidbody2D _mRb;
		private Vector3 _mVelocity = Vector3.zero;
		private float _velocity;

		private void Awake() {
			Screen.orientation = ScreenOrientation.LandscapeRight;
		}

		private void Start() {
			_mRb = GetComponent<Rigidbody2D>();
		}

		private void Update() {
#if UNITY_EDITOR
			_velocity = Input.GetAxis("Horizontal");
			if (Input.GetButtonDown("Jump")) Jump();
#else
            _velocity = _joystick.Horizontal;
#endif
		}

		private void FixedUpdate() {
			HorizontalMovement();
			GroundCheck();
		}
		
		private void HorizontalMovement() {
			var velocity = _mRb.velocity;
			var targetVelocity = new Vector2(_velocity * _movementSpeed, velocity.y);
			_mRb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref _mVelocity, .0001f);
		}

		private void GroundCheck() {
			var position = transform.position;
			var result = Physics2D.OverlapBoxNonAlloc(
				new Vector2(position.x + _groundColliderOffset.x, position.y + _groundColliderOffset.y),
				_groundColliderSize, 0f, _groundHit, _whatIsGround);
			_isGrounded = result == 1;
		}
		
		public void Jump() {
			if (!_isGrounded) return;
			_mRb.AddForce(new Vector2(0f, _jumpPower), ForceMode2D.Impulse);
		}
		
		private void OnDrawGizmosSelected() {
			var position = transform.position;
			Gizmos.color = Color.magenta;
			Gizmos.DrawCube(new Vector2(position.x + _groundColliderOffset.x, position.y + _groundColliderOffset.y),
				_groundColliderSize);
		}
		
		public void GoBack()
		{
			SceneManager.LoadScene("Example/Scenes/NimbusDemoScene");
		}

	}
}