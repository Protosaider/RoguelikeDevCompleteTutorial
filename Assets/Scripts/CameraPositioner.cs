using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositioner : MonoBehaviour
{
	[SerializeField]
	private Transform _targetTransform;

	private Transform _cameraTransform;
	private Vector3 _targetPosition;

    // Start is called before the first frame update
    void Start()
	{
		_cameraTransform = GetComponent<Transform>();
	}

    // Update is called once per frame
    void Update()
	{
		var position = _cameraTransform.position;
		_targetPosition = _targetTransform.position;
		position.x = _targetPosition.x;
		position.z = _targetPosition.z;
		_cameraTransform.position = position;
	}
}
