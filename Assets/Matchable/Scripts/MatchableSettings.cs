namespace MatchableSDK
{
    using UnityEngine;
    using System.IO;
    using System.Collections;
    using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    /// <summary>
    /// Matchable applications settings
    /// </summary>
    /// <seealso cref="UnityEngine.ScriptableObject" />
    public class MatchableSettings : ScriptableObject
	{
		const string mSettingsAssetName = "MatchableSettings";
	    const string mSettingsPath = "Matchable/Resources";
	    const string mSettingsAssetExtension = ".asset";

	    const string customerKeyLabel = "MATCHABLE_CUSTOMER_KEY";
	    const string customerKeyDefault = "<CUSTOMER_KEY>";

        const string playerIdLabel = "MATCHABLE_PLAYER_ID";
        const string playerIdDefault = "<DEFAULT_DEVICE_ID>";

        const string exampleCredentialsWarning = "MATCHABLE: You are using the default Matchable Customer Key! Go to Matchable > Edit Settings to fill in your customer key. If you need help, email us: support@matchable.io"; 

	    private static bool credentialsWarning = false;

	    private static MatchableSettings instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance singleton.
        /// </value>
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
        /// <summary>
        /// Edits this instance settings.
        /// </summary>
        [MenuItem("Matchable/Edit Settings")]
	    public static void Edit()
	    {
	        Selection.activeObject = Instance;
	    }

        /// <summary>
        /// Opens the documentation.
        /// </summary>
        [MenuItem("Matchable/SDK Documentation")]
	    public static void OpenDocumentation()
	    {
	        string url = "https://wiki.matchable.io/doku.php?id=info:api:v0.9";
	        Application.OpenURL(url);
	    }
#endif

        #region App Settings

        /// <summary>
        /// The API URL
        /// </summary>
        public static string apiUrl = "https://api.matchable.io";

        /// <summary>
        /// The API version
        /// </summary>
        public static string apiVersion = "v0.9";

        /// <summary>
        /// The SDK version
        /// </summary>
        public static string sdkVersion = "1.1"; 
        
        /// <summary>
        /// Define if the SDK plugin is enabled
        /// True by default
        /// Can be checked out in the settings 
        /// (then you'll need to call Matchable.Init() directly in the code to set to true)
        /// </summary>
        [SerializeField]
        public bool isPluginEnabled = true;

        /// <summary>
        /// The customer key
        /// </summary>
        [SerializeField]
		public string customerKey = customerKeyDefault;
        
        /// <summary>
        /// The player identifier
        /// </summary>
        [SerializeField]
		public string playerId = playerIdDefault;

        /// <summary>
        /// The game version
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
            public string gameVersion = PlayerSettings.bundleVersion;
        #else
            public string gameVersion;
        #endif

        /// <summary>
        /// Define if logging is enabled
        /// </summary>
        [SerializeField]
		public bool isLoggingEnabled = true;

        public ArrayList cachedActions;

        /// <summary>
        /// Sets the customer key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SetCustomerKey(string key)
	    {
	        if (!Instance.customerKey.Equals(key))
	        {
	            Instance.customerKey = key;
	            DirtyEditor();
	        }
	    }

        /// <summary>
        /// Gets the customer key.
        /// </summary>
        /// <returns></returns>
        public static string GetCustomerKey()
		{
			if(Instance.customerKey.Equals(customerKeyDefault))
			{
				CredentialsWarning();
				return customerKeyDefault;
			}

			return Instance.customerKey;
		}

        /// <summary>
        /// Sets the player identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void SetPlayerId(string id)
        {
            if (!Instance.playerId.Equals(id))
            {
                Instance.playerId = id;
                DirtyEditor();
            }
        }

        /// <summary>
        /// Gets the player identifier
        /// </summary>
        /// <returns>Returns the game player id if set, if not returns the device unique identifier </returns>
        public static string GetPlayerId()
        {
            if (Instance.playerId.Equals(playerIdDefault) || Instance.playerId == "")
            {
                //Use system unique identifier if no identifier is set
                return SystemInfo.deviceUniqueIdentifier;
            }
            //Else use specified player id
            return Instance.playerId;
        }

        /// <summary>
        /// Gets the game version.
        /// </summary>
        /// <returns></returns>
        public static string GetGameVersion()
        {
            return Instance.gameVersion;
        }

        /// <summary>
        /// Build matchable.io API url for the given endpoint using the provided customer_key
        /// </summary>
        /// <param name="endpoint">API endpoint name</param>
        /// <returns>
        /// The complete URL string for the given endpoint
        /// </returns>
        private static string GetCustomerEndpoint(string endpoint)
        {
            return String.Format("{0}/{1}/{2}/{3}/", apiUrl, apiVersion, endpoint, GetCustomerKey());
        }
        
        /// <summary>
        /// Build matchable.io API url for the given endpoint using the provided customer_key and player_id
        /// </summary>
        /// <param name="endpoint">API endpoint name</param>
        /// <returns>
        /// The complete URL string for the given endpoint
        /// </returns>
        private static string GetPlayerEndpoint(string endpoint)
        {
            return String.Format(GetCustomerEndpoint(endpoint) + "{0}/", MatchableSettings.GetPlayerId());
        }

        /// <summary>
        /// Gets the player endpoint.
        /// </summary>
        /// <returns></returns>
        public static string GetPlayerEndpoint()
        {
            return GetPlayerEndpoint("mplayers");
        }

        /// <summary>
        /// Gets the actions endpoint.
        /// </summary>
        /// <returns></returns>
        public static string GetActionsEndpoint()
        {
            return GetCustomerEndpoint("mactions");
        }

        /// <summary>
        /// Gets the advisor endpoint.
        /// </summary>
        /// <returns></returns>
        public static string GetAdvisorEndpoint()
        {
            return GetPlayerEndpoint("advisor");
        }

        /// <summary>
        /// Sets the game version.
        /// </summary>
        /// <param name="version">The version.</param>
        public void SetGameVersion(string version)
        {
            if (!Instance.gameVersion.Equals(version))
            {
                Instance.gameVersion = version;
                DirtyEditor();
            }
        }

        public static void SetLoggingEnabled(bool enabled)
		{
			Instance.isLoggingEnabled = enabled;
			DirtyEditor();
		}

		public static bool IsLoggingEnabled()
		{
			return Instance.isLoggingEnabled;
		}
        
        public static void SetPluginEnabled(bool enabled)
        {
            Instance.isPluginEnabled = enabled;
            DirtyEditor();
        }

        public static bool IsPluginEnabled()
        {
            return Instance.isPluginEnabled;
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
			if(!Instance.customerKey.Equals(customerKeyDefault))
			{
				Instance.SetCustomerKey(customerKeyDefault);
			}
			
			if(!Instance.playerId.Equals(playerIdDefault))
			{
				Instance.SetPlayerId(playerIdDefault);
			}
		}

	    #endregion
	}
}
