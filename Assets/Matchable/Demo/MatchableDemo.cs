﻿
namespace MatchableSDK.Demo
{
    using UnityEngine;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    /// This is the import needed to use the Matchable SDK
    using MatchableSDK;

    /// <summary>
    /// A simple demo app on how to integrate with the Matchable SDK
    /// </summary>
    public class MatchableDemo : MonoBehaviour
    {
        string _test = "";
        string _log = "";

        /// <summary>
        /// Demo of call the the GetRecommendations SDK method.
        /// Displays the response JSON string.
        /// </summary>
        /// <code>        
        ///    StartCoroutine(Matchable.GetRecommendations((response) =>
        ///    {
        ///         _log = response.ToJsonString();
        ///    }));
        /// </code>
        public IEnumerator DemoGetRecommendations()
        {
            _test = "Matchable.GetRecommendations()";
            _log = "Waiting for response...";
            // Call GetAdvisor asynchronously as a Coroutine
            yield return StartCoroutine(Matchable.GetRecommendations((response) =>
            {
                // Handle the API response the way you want
                _log = response.ToJsonString();
                // get the response as hashtable
                Hashtable hash = response.GetData();
                foreach (DictionaryEntry entry in hash)
                {
                    string str = String.Format("Key: {0}, Type: {1}", entry.Key.ToString(), entry.Value.GetType().ToString());
                    Debug.Log(str);
                }
                // Ex: get the recoms value
                // List<string> recoms = (List<string>) response.GetValue("recoms");
                // _log += "\n\nRecom: " + recoms[0];
            }));
        }

        /// <summary>
        /// Demo of call the the SendAction SDK method.
        /// Displays the response JSON string.
        /// </summary>
        /// <code>        
        ///     Hashtable parameters = new Hashtable();
        ///     parameters.Add("game_type", "tactical");
        ///     parameters.Add("xp", 0);
        ///     parameters.Add("player_lvl", 1);
        ///     parameters.Add("status", 1);
        ///     StartCoroutine(MatchableAction.StartGame(parameters, (response) =>
        ///     {
        ///         _log = response.ToJsonString();
        ///     }));
        /// </code>
        public IEnumerator DemoSendAction()
        {
            _test = "Matchable.SendAction()";
            _log = "Waiting for response...";
            Hashtable parameters = new Hashtable();

            parameters.Add("game_type", "tactical");
            parameters.Add("xp", 0);
            parameters.Add("player_lvl", 1);
            parameters.Add("status", 1);

            Dictionary<string, int> bonuses = new Dictionary<string, int>();
            bonuses.Add("bonus1", 2);
            bonuses.Add("bonus2", 3);
            parameters.Add("bonuses", bonuses);

            Hashtable resources = new Hashtable();
            resources.Add("resource1", 0);
            resources.Add("resource2", 0);
            parameters.Add("resources", resources);

            List<string> recoms = new List<string>();
            recoms.Add("recom1");
            recoms.Add("recom2");
            parameters.Add("recoms", recoms);

            // Call any MatchableAction asynchronously as a Coroutine
            yield return StartCoroutine(Matchable.SendAction("sample_action_type", parameters, (response) =>
            {
                // Handle the API response the way you want
                _log = response.ToJsonString();
            }));
        }

        /// <summary>
        /// Generate the GUI.
        /// </summary>
        void OnGUI()
        {
            // Initialize the SDK (optional)
            Matchable.Init();
            // Set the player id
            // MatchableSettings.SetPlayerId("demo_player_id");

            GUIStyle style;
            style = new GUIStyle(GUI.skin.box);
            style.wordWrap = true;
            style.padding = new RectOffset(5, 5, 5, 5);
            style.normal.textColor = Color.white;
            GUI.skin.box = style;

            int buttonSize = Screen.width / 2 - 10;
            int buttonHeight = 50;

            Rect rect = new Rect(10, 10, buttonSize, buttonHeight);
            if (GUI.Button(rect, "GetRecommendations()"))
            {
                StartCoroutine(DemoGetRecommendations());
            }

            rect = new Rect(buttonSize + 10, 10, buttonSize, buttonHeight);
            if (GUI.Button(rect, "SendAction()"))
            {
                StartCoroutine(DemoSendAction());
            }

            rect = new Rect(10, buttonHeight + 20, Screen.width - 20, Screen.height);
            string text = String.Format("{0}\n\n{1}", _test, _log);
            GUI.Box(rect, text);
        }
    }
}
