using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BaseRenderer : MonoBehaviour
{
	[SerializeField]
	private Sprite _sprite;

	private SpriteRenderer _spriteRenderer;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = _sprite;
		transform.localRotation = Quaternion.Euler(90, 0, 0);
		transform.localScale = new Vector3(3, 3, 1);
	}

	public void SetSprite(Sprite sprite, Int32 sortingOrder, Int32 sortingLayerId)
	{
		if (sprite == _spriteRenderer.sprite && _spriteRenderer.sortingOrder == sortingOrder && _spriteRenderer.sortingLayerID == sortingLayerId)
			return;

		_sprite = sprite;
		_spriteRenderer.sprite = _sprite;
		_spriteRenderer.sortingOrder = sortingOrder;
		_spriteRenderer.sortingLayerID = sortingLayerId;

	}
}
