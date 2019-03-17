using UnityEngine;

public class CameraSkinnedDecal : MonoBehaviour
{
	public float m_CameraSensivity = 10.0f;
	public float m_CameraMoveSpeed = 0.1f;
	public Material decalMaterial;
	[Range(0.05f, 2f)]public float m_Size = 0.2f;
	[Range(-1, 1)] public float m_NormalFactor = 0;
	[Range(0.001f, 10)] public float m_Offset = 0.1f;

	float m_Pitch;
	float m_Yaw;
	Camera m_Camera;

	private void Start()
	{
		m_Camera = GetComponent<Camera>();
		QualitySettings.vSyncCount = 0;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
			Shoot();

		if (Input.GetKey(KeyCode.Mouse1))
		{
			HandleMovementInput();
		}
	}

	void HandleMovementInput()
	{
		m_Yaw += Input.GetAxisRaw("Mouse X") * m_CameraSensivity;
		m_Pitch += -Input.GetAxisRaw("Mouse Y") * m_CameraSensivity;
		m_Pitch = Mathf.Clamp(m_Pitch, -90, 90);

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveDir.Normalize();

		moveDir *= m_CameraMoveSpeed;
		transform.position += transform.forward * moveDir.z + transform.right * moveDir.x;
		transform.rotation = Quaternion.Euler(m_Pitch, m_Yaw, 0);
	}

	void Shoot()
	{
		RaycastHit hitInfo;
		var ray = m_Camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hitInfo))
		{
			// Debug.Log(hitInfo.transform.name);
			SkinnedMeshRenderer ski = hitInfo.transform.GetComponent<SkinnedMeshRenderer>();
			if (ski == null)
				return;
			// for (int i = 0; i < ski.Length; i++)
			// {
				DecalSet decalset = ski.GetComponent<DecalSet>();
				decalset.AddDecal(transform, hitInfo.point, decalMaterial, m_Size, Random.Range(0, 360), m_NormalFactor, m_Offset);
			// }
		}

	}
}
