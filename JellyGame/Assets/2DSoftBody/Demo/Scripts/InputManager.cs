using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
	public List<GameObject> ObjectsToMove;
	public List<SoftObject> ObjectsToClone;
	public AudioSource CreateSource;
	public AudioClip[] CollideSounds;
	public int MaxObjectsCount;

	private Camera thisCamera;
	private Transform firstTransform;
	private Transform capturedObject;
	private Vector3 startTapPosition;
	private int currentObjectToInstantiateId;
	private int CurrentObjectToInstantiate
	{
		get
		{
			currentObjectToInstantiateId = (currentObjectToInstantiateId + 1) % ObjectsToClone.Count;
			return currentObjectToInstantiateId;
		}
	}

	void Awake()
	{
		Application.targetFrameRate = 60;
		firstTransform = ObjectsToMove[0].transform;
		thisCamera = Camera.allCameras[0];
		foreach (var objectToMove in ObjectsToMove)
		{
			var softObject = objectToMove.GetComponent<SoftObject>();
			if (softObject != null)
			{
				AddSoundsForCollisions(softObject);
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
#if UNITY_5_3_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
#else
			Application.LoadLevel(0);
#endif
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			var position = startTapPosition = thisCamera.ScreenToWorldPoint(Input.mousePosition);
			var haveExtraHit = false;
			var hits = Physics2D.RaycastAll(position, Vector2.zero);
			foreach (var hit in hits)
			{
				if (hit.transform != null && ObjectsToMove.Contains(hit.collider.gameObject))
				{
					capturedObject = hit.transform;
					var colliders = capturedObject.GetComponentsInChildren<Collider2D>();
					foreach (var coll in colliders)
					{
						coll.isTrigger = true;
					}
					break;
				}
				if (hit.transform != null)
				{
					haveExtraHit = true;
				}
			}

			if (capturedObject == null && ObjectsToMove.Count < MaxObjectsCount && !haveExtraHit)
			{
				if (Physics2D.CircleCastAll(position, 0.5f, Vector2.zero).Length == 0)
				{
					var newObject = Instantiate(ObjectsToClone[CurrentObjectToInstantiate]) as SoftObject;
					CreateSource.Play();
					if (newObject != null)
					{
						newObject.transform.position = new Vector3(position.x, position.y, firstTransform.position.z);
						newObject.transform.parent = firstTransform.parent;
						ObjectsToMove.Add(newObject.gameObject);
						AddSoundsForCollisions(newObject);
					}
				}
			}
		}
		if (Input.GetMouseButton(0))
		{
			var position = thisCamera.ScreenToWorldPoint(Input.mousePosition);
			if (capturedObject != null)
			{
				capturedObject.transform.position = new Vector3(position.x, position.y, capturedObject.transform.position.z);
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			var position = thisCamera.ScreenToWorldPoint(Input.mousePosition);
			var haveExtraHit = false;
			var caprtureObjectBounds = Vector2.zero;
			var caprtureObjectRadius = 0.05f;
			if (capturedObject != null)
			{
				caprtureObjectBounds = capturedObject.GetComponent<MeshFilter>().sharedMesh.bounds.size / 2f;
				caprtureObjectRadius = caprtureObjectBounds.x > caprtureObjectBounds.y ? caprtureObjectBounds.x : caprtureObjectBounds.y;
			}

			for (int i = 0; i < ObjectsToMove.Count; i++)
			{
				var hits = Physics2D.CircleCastAll(position, caprtureObjectRadius, Vector2.zero);
				foreach (var hit in hits)
				{
					if (hit.transform == null || capturedObject == null || hit.transform.IsChildOf(capturedObject)) continue;
					haveExtraHit = true;
					break;
				}

				if (haveExtraHit)
				{
					break;
				}
			}

			if (capturedObject != null)
			{
				if (haveExtraHit)
				{
					var newPosition = new Vector3(startTapPosition.x, Random.Range(8, 12), capturedObject.transform.position.z);
					if (Physics2D.CircleCastAll(newPosition, caprtureObjectRadius, Vector2.zero).Length > 0)
					{
						newPosition = new Vector3(newPosition.x, newPosition.y + 2f, newPosition.z);
					}
					capturedObject.transform.position = newPosition;
				}

				var rbodys = capturedObject.GetComponentsInChildren<Rigidbody2D>();
				foreach (var rigid in rbodys)
				{
					rigid.velocity = Vector3.zero;
					rigid.angularVelocity = 0f;
				}

				var colliders = capturedObject.GetComponentsInChildren<Collider2D>();
				foreach (var coll in colliders)
				{
					coll.isTrigger = false;
				}
				capturedObject.GetComponent<Collider2D>().isTrigger = true;
			}
			capturedObject = null;
		}
	}

	private void AddSoundsForCollisions(SoftObject softObject)
	{
		softObject.OnInitializeCompleted = () =>
		{
			foreach (var joint in softObject.Joints)
			{
				var jointSoundHelper = joint.GameObject.AddComponent<SoftObjectSoundHelper>();
				jointSoundHelper.InputManager = this;
				jointSoundHelper.Rigidbody2D = joint.Rigidbody2D;
			}
		};
	}
}