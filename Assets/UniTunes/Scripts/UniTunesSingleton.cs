using UnityEngine;

public abstract class UniTunesSingleton<T> : MonoBehaviour where T : UniTunesSingleton<T>
{
    private static T m_Instance = null;
    public static T Instance
    {
        get
        {
			if((object)m_Instance != null) {
        		return m_Instance;
        	}
            // Instance requiered for the first time, we look for it
            else {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                // Object not found, we create a temporary one
                if( m_Instance == null )
                {
                    m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    // Problem during the creation, this should not happen
                    if( m_Instance == null )
                    {
						Debug.LogError("UniTunesSingleton: Problem during the creation of " + typeof(T).ToString());
                    }
                }
            }

            return m_Instance;
        }
    }

    /// <summary>
    /// Awake function. Override when necessary and call base.Awake() first.
    /// </summary>
    protected virtual void Awake()
    {
        if( m_Instance == null )
        {
            m_Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Clear the reference when the application quits. Override when necessary and call base.OnApplicationQuit() last.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        m_Instance = null;
    }

	protected static void ForceNullInstance()
	{
		m_Instance = null;
	}
}

