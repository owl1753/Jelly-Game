using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class SoftObject : MonoBehaviour
{
	public enum FillType
	{
		Rectangle,
		Ellipse
	}

	public FillType Filling;
	public float JointRadius = 0.25f;
	public float Distance = 0.05f;
	public float Frequency = 10f;
	public float DampingRation;
	public bool AutoMass;
	public float Mass = 1f;
	public float AngularDrag = 0.05f;
	public float LinearDrag;
	public PhysicsMaterial2D JointsPhysicMaterial;
	public LayerMask JointsLayer;
	public List<mJoint> Joints = new List<mJoint>();
	public Action OnInitializeCompleted;

	private Transform thisTransform;
	private Rigidbody2D thisRigidbody;
	private MeshFilter meshFilter;
	private Mesh sharedMesh;
	private List<Vector3> jointsStartLocalPositions = new List<Vector3>();
	private float[] vertsAffected;
	private Vector3[] startVertices;
	private bool canUpdate;
	private bool isCached;

	private const float MinimalMass = 0.0001f;
	private const float PIFourThirds = 2.35619449019839f;

	[Serializable]
	public class mJoint
	{
		public CircleCollider2D Collider;
		public SpringJoint2D Joint;
		public GameObject GameObject;
		public Rigidbody2D Rigidbody2D;
		public Transform Transform;
		public float[] Weights;
	}

	private void Awake()
	{
		if (enabled)
		{
			Initialize();
		}
	}

	private void CacheObjects()
	{
		thisTransform = transform;
		thisRigidbody = GetComponent<Rigidbody2D>();
		meshFilter = GetComponent<MeshFilter>();
		sharedMesh = meshFilter.sharedMesh;
		startVertices = sharedMesh.vertices;
		isCached = true;
	}
		
	private void Initialize()
	{
		if (!isCached)
		{
			CacheObjects();
		}

		canUpdate = false;
		DestroyJoints();

		jointsStartLocalPositions.Clear();
		var size = sharedMesh.bounds.size;
		var meshQuadSize = GetMeshQuadSize();
		var jointsHorizontalCount = (int)Mathf.Round(size.x / meshQuadSize.x) + 1;
		var jointsVerticalCount = (int)Mathf.Round(size.y / meshQuadSize.y) + 1;
		var scaledSize = size - new Vector3(JointRadius * 2f, JointRadius * 2f);
		var offset = scaledSize / 2f;
		var jointCount = jointsHorizontalCount * jointsVerticalCount;
		var massOfJoint = Mass / jointCount;
		var mass = (massOfJoint > MinimalMass) ? massOfJoint : MinimalMass;

		if (Filling == FillType.Rectangle)
		{
			FillGrid(jointsHorizontalCount, jointsVerticalCount, mass, scaledSize, offset);
		}
		else
		{
			FillCircle(jointsHorizontalCount, jointsVerticalCount, mass, scaledSize);
		}

		IgnoreCollisions();
		canUpdate = true;

		if (OnInitializeCompleted != null)
		{
			OnInitializeCompleted();
		}
	}

	private void FillGrid(int jointsHorizontalCount, int jointsVerticalCount, float mass, Vector2 scaledSize, Vector2 offset)
	{
		for (int y = 0, ti = 0; y < jointsVerticalCount; y++)
		{	
			for (int x = 0; x < jointsHorizontalCount; x++, ti++)
			{
				var jointPosition = new Vector2(
					(scaledSize.x * x / (float)(jointsHorizontalCount - 1)),
					(scaledSize.y * y / (float)(jointsVerticalCount - 1))
					) - offset;
				CreateJoint(ti, mass, jointPosition, null);
			}
		}
	}

	private void FillCircle(int jointsHorizontalCount, int jointsVerticalCount, float mass, Vector2 scaledSize)
	{
		var vertsCount = jointsHorizontalCount * jointsVerticalCount;
		var length = Mathf.Max(jointsHorizontalCount, jointsVerticalCount);
		var jointsCount = (int)Mathf.Round(length * PIFourThirds);
		var size = sharedMesh.bounds.size;
		var offset = new Vector2(size.x / 2f - JointRadius, size.y / 2f - JointRadius);

		for (int i = 0; i < jointsCount; i++)
		{
			var angle = (i + 1) / (float)jointsCount * Mathf.PI * 2f;
			var jointPosition = new Vector3(Mathf.Cos(angle) * offset.x, Mathf.Sin(angle) * offset.y);
			var weights = new float[vertsCount];
			for (int j = 0; j < weights.Length; j++)
			{
				var distance = Vector2.Distance(startVertices[j], jointPosition) - JointRadius;
				if (distance < 0f)
				{
					distance = 1f;
				}
				else if (distance > JointRadius)
				{
					distance = 0f;
				}
				weights[j] = distance;
			}
			CreateJoint(i, mass, jointPosition, weights);
		}
		CalcVertsWeight();
	}

	private void CreateJoint(int id, float mass, Vector2 jointPosition, float[] weights)
	{
		var gJoint = new GameObject("Joint" + (id + 1));
		gJoint.transform.parent = thisTransform;
		gJoint.layer = JointsLayer;

		var joint = gJoint.AddComponent<SpringJoint2D>();
		var jointRigidBody = joint.GetComponent<Rigidbody2D>();
#if UNITY_5_3_OR_NEWER
		if (AutoMass)
		{
			jointRigidBody.useAutoMass = true;
		}
		else
		{
			jointRigidBody.mass = mass;
		}
#else
		jointRigidBody.mass = mass;
#endif
		jointRigidBody.drag = LinearDrag;
		jointRigidBody.angularDrag = AngularDrag;
#if UNITY_5_3_OR_NEWER || UNITY_5_2 || UNITY_5_1
		jointRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
#else
		jointRigidBody.fixedAngle = true;
#endif
#if UNITY_5_3_OR_NEWER
		joint.autoConfigureConnectedAnchor = false;
		joint.autoConfigureDistance = false;
#endif
		joint.connectedBody = thisRigidbody;
		joint.distance = Distance;
		joint.frequency = Frequency;
		joint.dampingRatio = DampingRation;

		var circleCollider = gJoint.AddComponent<CircleCollider2D>();
		circleCollider.radius = JointRadius;
		circleCollider.sharedMaterial = JointsPhysicMaterial;

		gJoint.transform.localPosition = jointPosition;
		joint.connectedAnchor = jointPosition;
		jointsStartLocalPositions.Add(jointPosition);

		var mJoint = new mJoint
		{
			Collider = circleCollider,
			Joint = joint,
			GameObject = gJoint,
			Rigidbody2D = jointRigidBody,
			Transform = gJoint.transform,
			Weights = weights
		};
		Joints.Add(mJoint);
	}

	private void CalcVertsWeight()
	{
		vertsAffected = new float[sharedMesh.vertices.Length];
		for (int i = 0; i < Joints.Count; i++)
		{
			for (int j = 0; j < Joints[i].Weights.Length; j++)
			{
				if (Joints[i].Weights[j] > 0f)
				{
					vertsAffected[j] += Joints[i].Weights[j];
				}
			}
		}
	}

	private Vector2 GetMeshQuadSize()
	{
		var vertices = meshFilter.sharedMesh.vertices;
		var triangles = meshFilter.sharedMesh.triangles;
		var cell = new Vector2(vertices[triangles[1]].x - vertices[triangles[0]].x, vertices[triangles[1]].y - vertices[triangles[0]].y);
		return cell;
	}

	private void IgnoreCollisions()
	{
		for (int i = 0; i < Joints.Count - 1; i++)
		{
			for (int j = i + 1; j < Joints.Count; j++)
			{
				if (Joints[i] != null && Joints[j] != null)
				{
					Physics2D.IgnoreCollision(Joints[i].Collider, Joints[j].Collider);
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (canUpdate)
		{
			var vertices = sharedMesh.vertices;
			if (Filling == FillType.Rectangle)
			{
				for (int i = 0; i < vertices.Length; i++)
				{
					var offset = Joints[i].Transform.localPosition - jointsStartLocalPositions[i];
					vertices[i] = startVertices[i] + offset;
				}
			}
			else
			{
				var newVerts = new Vector3[vertices.Length];
				for (int i = 0; i < Joints.Count; i++)
				{
					var offset = Joints[i].Transform.localPosition - jointsStartLocalPositions[i];
					for (int j = 0; j < Joints[i].Weights.Length; j++)
					{
						newVerts[j] += offset * Joints[i].Weights[j];
					}
				}
				for (int i = 0; i < newVerts.Length; i++)
				{
					if (vertsAffected[i] > 0f)
					{
						newVerts[i] /= vertsAffected[i];
					}
					newVerts[i] += startVertices[i];
				}
				vertices = newVerts;
			}
			sharedMesh.vertices = vertices;
		}
	}

	private void DestroyJoints()
	{
		foreach (var joint in Joints)
		{
			if (joint != null)
			{
				DestroyImmediate(joint.GameObject);
			}
		}
		Joints.Clear();
	}

	public void ForceUpdate()
	{
		Initialize();
	}

	public void UpdateParams()
	{
		foreach (var joint in Joints)
		{
			if (joint != null)
			{
#if UNITY_5_3_OR_NEWER
				if (AutoMass)
				{
					joint.Rigidbody2D.useAutoMass = true;
				}
				else
				{
					joint.Rigidbody2D.mass = Mass;
				}
#else
				joint.Rigidbody2D.mass = Mass;
#endif
				joint.Rigidbody2D.angularDrag = AngularDrag;
				joint.Rigidbody2D.drag = LinearDrag;
#if UNITY_5_3_OR_NEWER || UNITY_5_2 || UNITY_5_1
				joint.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
#else
				joint.Rigidbody2D.fixedAngle = true;
#endif
				joint.Joint.connectedBody = thisRigidbody;
				joint.Joint.distance = Distance;
				joint.Joint.frequency = Frequency;
				joint.Joint.dampingRatio = DampingRation;

				var circleCollider = (CircleCollider2D)joint.Collider;
				circleCollider.radius = JointRadius;
			}
		}
	}
}