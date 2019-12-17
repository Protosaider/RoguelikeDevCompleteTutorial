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
		var cameraPosition = _cameraTransform.position;
		var position = cameraPosition;

		_targetPosition = _targetTransform.position;
		position.x = _targetPosition.x;
		position.z = _targetPosition.z;

		cameraPosition = Vector3.Lerp(cameraPosition, position, Time.deltaTime);

		_cameraTransform.position = cameraPosition;
	}
}
