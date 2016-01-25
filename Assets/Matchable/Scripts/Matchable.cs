using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
#if UNITY_4_6 || UNITY_5
using UnityEngine.EventSystems;
#endif

/// <summary>
/// MatchableSDK class
/// </summary>
namespace MatchableSDK
{
    /// <summary>
    /// Provide methods to call Matchable API.
    /// For more information on integrating and using the Matchable SDK
    /// please visit our help site documentation at https://wiki.matchable.io/doku.php?id=info:api:v0.9
    /// </summary>
    public sealed class Matchable
    {
        /// <summary>
        /// The Matchable API scheme
        /// </summary>
        private const string scheme = "https";

        /// <summary>
        /// The Matchable API URL
        /// </summary>
        private const string url = "api.matchable.io";
        
        /// <summary>
        /// The Matchable API version
        /// </summary>
        private const string version = "v0.9";

        /// <summary>
        /// Build matchable.io API url for the given endpoint using the provided customer_key
        /// </summary>
        /// <param name="endpoint">API endpoint name</param>
        /// <returns>
        /// The complete URL string for the given endpoint
        /// </returns>
        private static string BuildCustomerEndpoint(string endpoint)
        {
            return String.Format("{0}://{1}/{2}/{3}/{4}/", scheme, url, version, endpoint, MatchableSettings.GetCustomerKey());
        }

        /// <summary>
        /// Build matchable.io API url for the given endpoint using the provided customer_key and player_id
        /// </summary>
        /// <param name="endpoint">API endpoint name</param>
        /// <returns>
        /// The complete URL string for the given endpoint
        /// </returns>
        private static string BuildPlayerEndpoint(string endpoint)
        {
            return String.Format(BuildCustomerEndpoint(endpoint) + "{0}/", MatchableSettings.GetPlayerId());
        }

        /// <summary>
        /// Retrieve all the statistics available for the default player
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public static IEnumerator GetStats(Action<MatchableResponse> callback)
        {
            WWW request = new WWW(BuildPlayerEndpoint("mplayers"));
            yield return request;
            MatchableResponse response = new MatchableResponse(request);
            yield return response;
            callback(response);
        }

        /// <summary>
        /// Add a new action with the given type and parameters for the default player 
        /// This action is saved locally and can be sent via the SendPlayerActions() method
        /// </summary>
        /// <param name="type">Action type (ex: game_start)</param>
        /// <param name="parameters">Action parameters (JSON string)</param>
        private static Hashtable CreateAction(string type, object parameters)
        {
            Hashtable action = new Hashtable();
            action.Add("player_id", MatchableSettings.GetPlayerId());
            action.Add("type", type);
            action.Add("parameters", parameters);
            action.Add("date", TimeStamp.UnixTimeStampUTC());
            return action;
        }

        /// <summary>
        /// Send a player action with the given type and parameters.
        /// Then executes the given callback when the response is received.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public static IEnumerator SendAction(string type, object parameters, Action<MatchableResponse> callback)
        {
            if (type == null)
            {
                Debug.LogError("SendAction(): parameter 'type' is required");
                yield return null;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");

            Hashtable action = CreateAction(type, parameters);

            // Simple hack to wrap the action inside a JSON array
            string data = "[" + MJSON.Serialize(action) + "]";
            if (MatchableSettings.isLogging())
            {
                Debug.Log("Sent action:" + data);
            }
            byte[] postData = AsciiEncoding.StringToAscii(data);

            WWW request = new WWW(BuildCustomerEndpoint("mactions"), postData, headers);
            yield return request;
            MatchableResponse response = new MatchableResponse(request);
            yield return response;
            callback(response);
        }

        /// <summary>
        /// Retrieve a retention action for a given player. 
        /// This action is obtained using a strategy based on the different scores computed by Matchable. 
        /// The strategies can be specifically developped for each customer in collaboration with Matchable's data scientists.
        /// </summary>
        public static IEnumerator GetAdvisor(Action<MatchableResponse> callback)
        {
            WWW request = new WWW(BuildPlayerEndpoint("advisor"));
            yield return request;
            MatchableResponse response = new MatchableResponse(request);
            yield return response;
            callback(response);
        }
    }
}