using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatchableSDK {

#if UNITY_EDITOR	
	[InitializeOnLoad]
#endif
	public class MatchableSettings : ScriptableObject
	{
		const string mSettingsAssetName = "MatchableSettings";
	    const string mSettingsPath = "Matchable/Resources";
	    const string mSettingsAssetExtension = ".asset";

	    const string mCustomerKeyLabel = "MATCHABLE_CUSTOMER_KEY";
	    const string mCustomerKeyDefault = "<CUSTOMER_KEY>";

        const string mPlayerIdLabel = "MATCHABLE_PLAYER_ID";
        const string mPlayerIdDefault = "<DEFAULT_DEVICE_ID>";

        const string exampleCredentialsWarning = "MATCHABLE: You are using the default Matchable Customer Key! You need to fill in your customer key to reach the API. If you need help, email us: support@matchable.io"; 

	    private static bool credentialsWarning = false;

	    private static MatchableSettings instance;

	    static MatchableSettings Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
	                instance = Resources.Load(mSettingsAssetName) as MatchableSettings;
	                if (instance == null)
	                {
	                    // If not found, autocreate the asset object.
	                    instance = CreateInstance<MatchableSettings>();
	#if UNITY_EDITOR
	                    string properPath = Path.Combine(Application.dataPath, mSettingsPath);
	                    if (!Directory.Exists(properPath))
	                    {
	                        AssetDatabase.CreateFolder("Assets/Matchable", "Resources");
	                    }

	                    string fullPath = Path.Combine(Path.Combine("Assets", mSettingsPath),
	                                                   mSettingsAssetName + mSettingsAssetExtension
	                                                  );
	                    AssetDatabase.CreateAsset(instance, fullPath);
	#endif
	                }
	            }
	            return instance;
	        }
	    }

	#if UNITY_EDITOR
	    [MenuItem("Matchable/Edit Settings")]
	    public static void Edit()
	    {
	        Selection.activeObject = Instance;
	    }

	    [MenuItem("Matchable/SDK Documentation")]
	    public static void OpenDocumentation()
	    {
	        string url = "https://wiki.matchable.io/doku.php?id=info:api:v0.9";
	        Application.OpenURL(url);
	    }
	#endif

	    #region App Settings
		[SerializeField]
		public string mCustomerKey = mCustomerKeyDefault;
		[SerializeField]
		public string mPlayerId = mPlayerIdDefault;
		[SerializeField]
		public bool isLoggingEnabled = false;
		

		public void SetCustomerKey(string key)
	    {
	        if (!Instance.mCustomerKey.Equals(key))
	        {
	            Instance.mCustomerKey = key;
	            DirtyEditor();
	        }
	    }

		public static string GetCustomerKey()
		{
			if(Instance.mCustomerKey.Equals(mCustomerKeyDefault))
			{
				CredentialsWarning();
				return mCustomerKeyDefault;
			}

			return Instance.mCustomerKey;
		}

        public void SetPlayerId(string id)
        {
            if (!Instance.mPlayerId.Equals(id))
            {
                Instance.mPlayerId = id;
                DirtyEditor();
            }
        }

        public static string GetPlayerId()
        {
            if (Instance.mPlayerId.Equals(mPlayerIdDefault))
            {
                CredentialsWarning();
                //Use system unique identifier if no identifier is set
                return SystemInfo.deviceUniqueIdentifier;
            }
            //Else use specified player id
            return Instance.mPlayerId;
        }

        public static void enableLogging(bool enabled)
		{
			Instance.isLoggingEnabled = enabled;
			DirtyEditor();
		}

		public static bool isLogging()
		{
			return Instance.isLoggingEnabled;
		}

	    private static void DirtyEditor()
	    {
	#if UNITY_EDITOR
	        EditorUtility.SetDirty(Instance);
	#endif
	    }

	    private static void CredentialsWarning()
	    {
	    	if(credentialsWarning == false) 
	    	{
				credentialsWarning = true;

				Debug.LogWarning(exampleCredentialsWarning);
			}
	    }

		public static void resetSettings()
		{	
			if(!Instance.mCustomerKey.Equals(mCustomerKeyDefault))
			{
				Instance.SetCustomerKey(mCustomerKeyDefault);
			}
			
			if(!Instance.mPlayerId.Equals(mPlayerIdDefault))
			{
				Instance.SetPlayerId(mPlayerIdDefault);
			}
		}

	    #endregion
	}
}
