namespace MatchableSDK
{
    using System;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Matchable Handler game object
    /// Used to save actions between scenes and auto run StartSession actions at start
    /// Needs to be imported (drag&drop in the first scene of your game)
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class MatchableHandler : MonoBehaviour
    {
        /// <summary>
        /// The singleton class instance
        /// </summary>
        public static MatchableHandler instance;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
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

        /// <summary>
        /// Starts this instance.
        /// Sends the StartSession action each time the game runs
        /// </summary>
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
