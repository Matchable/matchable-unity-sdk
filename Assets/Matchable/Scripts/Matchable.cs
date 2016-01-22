using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
#if UNITY_4_6 || UNITY_5
using UnityEngine.EventSystems;
#endif

namespace MatchableSDK
{
    /// <summary>
    ///  Provide methods to call Matchable API.
    ///  For more information on integrating and using the Matchable SDK
    ///  please visit our help site documentation at https://wiki.matchable.io/doku.php?id=info:api:v0.9
    /// </summary>
    public class Matchable
    {
        const string scheme = "https";
        const string url = "api.matchable.io";
        const string version = "v0.9";

        string _customerKey;
        string _playerId;
        
        public Matchable(string customerKey, string playerId)
        {
            _customerKey = customerKey;
            _playerId = playerId;
        }

        /// <summary>
        /// Build matchable.io API url for this specific endpoint using the provided customer_key
        /// ex: http://api.matchable.io/v0.9/advisor/<CUSTOMER_KEY>/
        /// </summary>
        /// <param name="endpoint">API endpoint name</param>
        /// <returns>The complete URL string for the given endpoint</returns>
        protected string BuildCustomerUrl(string endpoint)
        {
            return String.Format("{0}://{1}/{2}/{3}/{4}/", scheme, url, version, endpoint, _customerKey);
        }

        /// <summary>
        /// Build matchable.io API url for this specific endpoint using the provided customer_key and player_id
        /// ex: http://api.matchable.io/v0.9/advisor/<CUSTOMER_KEY>/<PLAYER_ID/
        /// </summary>
        /// <param name="endpoint">API endpoint name</param>
        /// <returns>The complete URL string for the given endpoint</returns>
        protected string BuildPlayerUrl(string endpoint)
        {
            return String.Format(BuildCustomerUrl(endpoint) + "{0}/", _playerId);
        }

        /// <summary>
        /// Retrieve all the statistics available for the default player
        /// ex:http://api.matchable.io/v0.9/mplayers/<CUSTOMER_KEY>/<PLAYER_ID>/
        /// </summary>
        public IEnumerator GetStats(Action<object> callback)
        {
            WWW response = new WWW(BuildPlayerUrl("mplayers"));
            yield return response;
            string jsonData = "";
            if (string.IsNullOrEmpty(response.error))
            {
                jsonData = System.Text.Encoding.UTF8.GetString(response.bytes);
                yield return jsonData;
                callback(MJSON.Deserialize(jsonData));
            }
        }

        /// <summary>
        /// Add a new action with the given type and parameters for the default player 
        /// This action is saved locally and can be sent via the SendPlayerActions() method
        /// </summary>
        /// <param name="type">Action type (ex: game_start)</param>
        /// <param name="parameters">Action parameters (JSON string)</param>
        protected Hashtable CreateAction(string type, object parameters)
        {
            Hashtable action = new Hashtable();
            action.Add("player_id", _playerId);
            action.Add("type", type);
            action.Add("parameters", parameters);
            action.Add("date", TimeStamp.UnixTimeStampUTC());
            return action;
        }

        /// <summary>
        /// Send a player action with the given type and parameters.
        /// Then executes the given callback when the response is received.
        /// </summary>
        public IEnumerator SendAction(string type, object parameters, Action<object> callback)
        {
            if (type == null) {
                Debug.LogError("SendAction(): type is mandatory");
                yield return null;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");

            Hashtable action = CreateAction(type, parameters);
            
            // Simple hack to wrap the action inside a JSON array
            string data = "[" + MJSON.Serialize(action) + "]";
            Debug.Log("Sent actions:" + data);
            byte[] postData = System.Text.Encoding.ASCII.GetBytes(data.ToCharArray());

            WWW response = new WWW(BuildCustomerUrl("mactions"), postData, headers);
            yield return response;
            string jsonData = "";
            if (string.IsNullOrEmpty(response.error))
            {
                jsonData = System.Text.Encoding.UTF8.GetString(response.bytes);
                yield return jsonData;
                callback(MJSON.Deserialize(jsonData));
            }
        }

        /// <summary>
        /// Retrieve a retention action for a given player. 
        /// This action is obtained using a strategy based on the different scores computed by Matchable. 
        /// The strategies can be specifically developped for each customer in collaboration with Matchable's data scientists.
        /// </summary>
        public IEnumerator GetAdvisor(Action<object> callback)
        {
            WWW response = new WWW(BuildPlayerUrl("advisor"));
            yield return response;
            string jsonData = "";
            if (string.IsNullOrEmpty(response.error))
            {
                jsonData = System.Text.Encoding.UTF8.GetString(response.bytes);
                yield return jsonData;
                callback(MJSON.Deserialize(jsonData));
            }
        }
    }
}