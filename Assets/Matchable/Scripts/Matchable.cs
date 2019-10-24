namespace MatchableSDK
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Utils;
    using UnityEngine.Networking;

#if UNITY_4_6 || UNITY_5
    using UnityEngine.EventSystems;
#endif

    /// <summary>
    /// Provide methods to call Matchable API.
    /// For more information on integrating and using the Matchable SDK
    /// please visit our help site documentation at http://developers.matchable.io/
    /// </summary>
    public sealed class Matchable
    {

        /// <summary>
        /// Send a player action with the given type and parameters.
        /// Then executes the given callback when the response is received.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public static IEnumerator SendAction(string type, Hashtable parameters, Action<MatchableResponse> callback)
        {
            if (MatchableSettings.IsPluginEnabled())
            {
                if (type == null)
                {
                    Debug.LogError("SendAction(): parameter 'type' is required");
                    yield return null;
                }

                Hashtable action = MatchableAction.Create(type, parameters);

                // Simple hack to wrap the action inside a JSON array
                string data = "[" + MJSON.Serialize(action) + "]";
                if (MatchableSettings.IsLoggingEnabled())
                {
                    Debug.Log("Sent action:" + data);
                }
                byte[] postData = AsciiEncoding.StringToAscii(data);

                UnityWebRequest request = UnityWebRequest.Post(MatchableSettings.GetActionsEndpoint(), data);
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "api_key " + MatchableSettings.GetAppKey());

                yield return request.SendWebRequest();
                MatchableResponse response = new MatchableResponse(request);
                yield return response;
                callback(response);
            }
        }

        /// <summary>
        /// Retrieve recommendations for a given player. 
        /// The recommendations are obtained using a strategy based on the different scores computed by Matchable. 
        /// The strategies can be specifically developped for each customer in collaboration with Matchable's data scientists.
        /// </summary>
        /// /// <code>
        /// StartCoroutine(Matchable.GetAdvisor((response) =>
        /// {
        ///     Debug.Log(response.ToJsonString());
        /// }));
        /// </code>
        public static IEnumerator GetRecommendations(Action<MatchableResponse> callback)
        {
            if (MatchableSettings.IsPluginEnabled())
            {
                UnityWebRequest request = UnityWebRequest.Get(MatchableSettings.GetRecommendationsEndpoint());
                request.SetRequestHeader("Authorization", "api_key " + MatchableSettings.GetAppKey());
                yield return request.SendWebRequest();
                MatchableResponse response = new MatchableResponse(request);
                yield return response;
                callback(response);
            }
        }

        /// <summary>
        /// Initializes the SDK plugin. (optional)
        /// Enabled by default in Matchable > EditSettings
        /// </summary>
        public static void Init()
        {
            MatchableSettings.SetPluginEnabled(true);
        }

        /// <summary>
        /// Disable the SDK plugin.
        /// Prevents all the method calls (useful for debuging other plugins)
        /// </summary>
        public static void Disable()
        {
            MatchableSettings.SetPluginEnabled(false);
        }
    }

    /// <summary>
    /// Actions sent to the Matchable API
    /// </summary>
    class MatchableAction
    {
        /// <summary>
        /// Add a new action with the given type and parameters for the default player 
        /// This action is saved locally and can be sent via the SendPlayerActions() method
        /// </summary>
        /// <param name="type">Action type (ex: game_start)</param>
        /// <param name="parameters">Action parameters (JSON string)</param>
        public static Hashtable Create(string type, Hashtable parameters)
        {
            Hashtable action = new Hashtable();
            //adding device info
            action.Add("device_model", SystemInfo.deviceModel);
            action.Add("device_type", SystemInfo.deviceType);
            action.Add("operating_system", SystemInfo.operatingSystem);
            action.Add("version", MatchableSettings.GetGameVersion());
            action.Add("type", type);
            action.Add("parameters", parameters);
            action.Add("date", TimeStamp.UnixTimeStampUTC());
            return action;
        }
    }

    /// <summary>
    /// Response from the Matchable API.
    /// </summary>
    public class MatchableResponse
    {
        private object _data;
        private string _text;


        /// <summary>
        /// Initializes a new instance of the <see cref="MatchableResponse"/> class.
        /// </summary>
        /// <param name="request">The original matchable request.</param>
        public MatchableResponse(UnityWebRequest request)
        {
            _text = request.downloadHandler.text;
            try
            {
                _data = MJSON.Deserialize(_text);
            }
            catch (OverflowException e)
            {
                Debug.Log("Trying to deserialize empty json object");
                Debug.Log(e);
                _data = null;
            }
        }

        /// <summary>
        /// Get the JSON response as a string
        /// </summary>
        /// <returns>The string of the JSON response</returns>
        public string ToJsonString()
        {
            return _text;
        }

        /// <summary>
        /// Serialise the JSON as a Hashtable
        /// </summary>
        /// <returns>The Hashtable containing attributes from the JSON response</returns>
        public Hashtable GetData()
        {
            return (Hashtable)_data;
        }

        /// <summary>
        /// Retrieve the value for the given key in the _data Hashtable
        /// </summary>
        /// <returns>The value matching the key</returns>
        public object GetValue(string key)
        {
            return GetData() != null ? GetData()[key] : null;
        }
    }
}