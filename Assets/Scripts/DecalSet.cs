using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DecalSet : MonoBehaviour
{
	public int m_MaxDecals = 10;
	public SkinQuality m_DecalQuality = SkinQuality.Bone1;

	[HideInInspector]
	public List<GameObject> m_DecalList = new List<GameObject>();

	SkinnedMeshRenderer m_SkiMesh;

	int m_DecalCount = 0;
	Vector3[] m_Vertices;
	Matrix4x4 m_VP;
	float m_NormalFactor;
	float m_Depth;
	float m_Offset;
	Vector3 m_Dir;
	float m_UVRot;

	void Start()
	{
		m_SkiMesh = GetComponent<SkinnedMeshRenderer>();
	}

	public void AddDecal(Transform origin, Vector3 point, Material decalMaterial, float size = 0.2f, float rotation = 0, float normalFactor = 0, float offset = 0.1f, float depth = 1)
	{
		m_DecalCount++;

		if (m_DecalCount > m_MaxDecals)
		{
			Destroy(m_DecalList[0]);
			m_DecalList.RemoveAt(0);
			m_DecalCount--;
		}

		m_UVRot = rotation;
		m_NormalFactor = normalFactor;
		m_Depth = depth;
		m_Dir = origin.forward;

		// project from a close point from the hit point
		Matrix4x4 v = Matrix4x4.Inverse(Matrix4x4.TRS(point - m_Dir * offset, origin.rotation, new Vector3(1, 1, -1)));
		// project from origin (need a high depth value)
		// Matrix4x4 v = Matrix4x4.Inverse(Matrix4x4.TRS(origin.position, origin.rotation, new Vector3(1, 1, -1)));
		Matrix4x4 p = Matrix4x4.Ortho(-size, size, -size, size, 0.0001f, depth);

		m_VP = p * v;

		// get decalmesh
		Mesh decalMesh = Mesh.Instantiate(m_SkiMesh.sharedMesh);

		// get a snapshot of the mesh to test against
		Mesh bakeMesh = new Mesh();
		m_SkiMesh.BakeMesh(bakeMesh);

		// get vertices that are going to be tested
		m_Vertices = bakeMesh.vertices;

		// uvs
		Vector2[] uvs = decalMesh.uv;

		// process each submesh
		for (int subMesh = 0; subMesh < decalMesh.subMeshCount; subMesh++)
		{
			List<int> triangleList = new List<int>();
			int[] triangles = decalMesh.GetTriangles(subMesh);

			// check each triangle against view Frustum
			for (int i = 0; i < triangles.Length; i += 3)
			{
				if (isInsideFrustum(triangles[i], triangles[i + 1], triangles[i + 2], ref uvs))
				{
					triangleList.Add(triangles[i]);
					triangleList.Add(triangles[i + 1]);
					triangleList.Add(triangles[i + 2]);
				}
			}

			if (triangleList.Count < 3)
				continue;

			decalMesh.SetTriangles(triangleList.ToArray(), subMesh);
		}
		decalMesh.uv = uvs;

		// create go
		GameObject decalGO = new GameObject("decal");
		decalGO.transform.parent = m_SkiMesh.transform;

		// create skinned mesh
		SkinnedMeshRenderer decalSkinRend = decalGO.AddComponent<SkinnedMeshRenderer>();
		decalSkinRend.quality = m_DecalQuality;
		decalSkinRend.shadowCastingMode = ShadowCastingMode.Off;
		decalSkinRend.sharedMesh = decalMesh;
		decalSkinRend.bones = m_SkiMesh.bones;
		decalSkinRend.rootBone = m_SkiMesh.rootBone;
		decalSkinRend.sharedMaterial = decalMaterial;

		m_DecalList.Add(decalGO);

		Destroy(bakeMesh);
	}

	bool isInsideFrustum(int t1, int t2, int t3, ref Vector2[] uvs)
	{
		Vector3 v1 = m_Vertices[t1];
		Vector3 v2 = m_Vertices[t2];
		Vector3 v3 = m_Vertices[t3];

		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_VP);

		// check against the 6 planes
		for (int i = 0; i < 6; i++)
		{
			if (!planes[i].GetSide(transform.TransformPoint(v1)) && !planes[i].GetSide(transform.TransformPoint(v2)) && !planes[i].GetSide(transform.TransformPoint(v3)))
				return false;
			if (!FacingNormal(v1, v2, v3))
				return false;
		}

		uvs[t1] = m_VP * transform.localToWorldMatrix * new Vector4(v1.x, v1.y, v1.z, 1);
		uvs[t2] = m_VP * transform.localToWorldMatrix * new Vector4(v2.x, v2.y, v2.z, 1);
		uvs[t3] = m_VP * transform.localToWorldMatrix * new Vector4(v3.x, v3.y, v3.z, 1);

		uvs[t1] *= 0.5f;
		uvs[t2] *= 0.5f;
		uvs[t3] *= 0.5f;

		Quaternion rot = Quaternion.Euler(0, 0, m_UVRot);

		uvs[t1] = rot * uvs[t1];
		uvs[t2] = rot * uvs[t2];
		uvs[t3] = rot * uvs[t3];

		uvs[t1] += new Vector2(0.5f, 0.5f);
		uvs[t2] += new Vector2(0.5f, 0.5f);
		uvs[t3] += new Vector2(0.5f, 0.5f);


		return true;
	}

	bool FacingNormal(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		Plane plane = new Plane(v1, v2, v3);

		if (Vector3.Dot(-m_Dir.normalized, plane.normal) < m_NormalFactor)
			return false;

		return true;
	}
}
