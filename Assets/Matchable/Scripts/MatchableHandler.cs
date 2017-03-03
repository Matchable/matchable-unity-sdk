using System;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MatchableSDK
{
    /// <summary>
    /// Matchable Handler game object, used to save actions between scenes and auto run StartSession actions at start
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class MatchableHandler : MonoBehaviour
    {
        /// <summary>
        /// The singleton class instance
        /// </summary>
        public static MatchableHandler instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
			Hashtable parameters = new Hashtable();
			parameters.Add("version", MatchableSettings.GetGameVersion());

			StartCoroutine(Matchable.SendAction("start_session", parameters, (response) =>
            {
                if (MatchableSettings.IsLoggingEnabled())
                {
                    Debug.Log("Start session response:" + response.ToJsonString());
                }
            }));
        }
    }
}
