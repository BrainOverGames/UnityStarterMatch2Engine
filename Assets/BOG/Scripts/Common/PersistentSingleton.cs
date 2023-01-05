using UnityEngine;

namespace BOG
{
	/// <summary>
	///  Only one instance of class is required at one point of time & it can be preserved during the runtime of game
	/// </summary>
	/// <typeparam name="T">type of class to be persisted</typeparam>
	public class PersistentSingleton<T> : MonoBehaviour	where T : Component
	{
		public static T Current => _instance;
		
		protected static T _instance;
		protected bool _enabled;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T> ();
					if (_instance == null)
					{
						GameObject obj = new GameObject ();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T> ();
					}
				}
				return _instance;
			}
		}

	    protected virtual void Awake ()
		{
			if (!Application.isPlaying)
			{
				return;
            }

            if (_instance == null)
			{
				_instance = this as T;
				DontDestroyOnLoad (transform.gameObject);
				_enabled = true;
			}
			else
			{
				if(this != _instance)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}
}
