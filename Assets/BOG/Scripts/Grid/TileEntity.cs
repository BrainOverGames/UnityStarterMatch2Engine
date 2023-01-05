using UnityEngine;
using UnityEngine.Events;

namespace BOG
{
	/// <summary>
	/// The base class used for the tile entities in the game.
	/// </summary>
	public class TileEntity : MonoBehaviour
	{
		public UnityEvent onSpawn;
		public UnityEvent onExplode;

		internal virtual void OnEnable()
		{
			var spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				var newColor = spriteRenderer.color;
				newColor.a = 1.0f;
				spriteRenderer.color = newColor;
			}
			transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			transform.localRotation = Quaternion.identity;
		}

		internal virtual void OnDisable()
		{
			var spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				var newColor = spriteRenderer.color;
				newColor.a = 1.0f;
				spriteRenderer.color = newColor;
			}
			transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			transform.localRotation = Quaternion.identity;
		}

		internal virtual void Spawn()
		{
			onSpawn.Invoke();
		}

		internal virtual void Explode()
		{
			onExplode.Invoke();
		}
	}
}
