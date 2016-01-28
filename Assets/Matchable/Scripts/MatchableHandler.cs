using System;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

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
            StartCoroutine(MatchableAction.StartSession((response) =>
            {
                if (MatchableSettings.IsLoggingEnabled())
                {
                    Debug.Log("Start session response:" + response.ToJsonString());
                }
            }));
        }
    }
}
