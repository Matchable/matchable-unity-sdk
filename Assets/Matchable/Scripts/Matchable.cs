using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
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
        ArrayList _actions;

        public Matchable(string customerKey, string playerId)
        {
            _customerKey = customerKey;
            _playerId = playerId;
            _actions = new ArrayList();
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
        public IEnumerator GetPlayerStats(Action<string> callback)
        {
            WWW response = new WWW(BuildPlayerUrl("mplayers"));
            yield return response;
            string jsonData = "";
            if (string.IsNullOrEmpty(response.error))
            {
                jsonData = System.Text.Encoding.UTF8.GetString(response.bytes);
                yield return jsonData;
                callback(jsonData);
            }
        }

        /// <summary>
        /// Class modelling a player action object
        /// </summary>
        [Serializable]
        class PlayerAction
        {
            public string player_id;
            public string type;
            public string parameters;
            public Int32 date;
        }

        /// <summary>
        /// Add a new action with the given type and parameters for the default player 
        /// This action is saved locally and can be sent via the SendPlayerActions() method
        /// </summary>
        /// <param name="type">Action type (ex: game_start)</param>
        /// <param name="parameters">Action parameters (JSON string)</param>
        public void AddPlayerAction(string type, string parameters)
        {
            PlayerAction action = new PlayerAction();
            action.player_id = _playerId;
            action.type = type;
            action.parameters = parameters;
            action.date = TimeStamp.UnixTimeStampUTC();
            _actions.Add(action);
        }

        /// <summary>
        /// Remove all the actions that were added for the default player using AddPlayerAction
        /// </summary>
        public void CleanPlayerActions()
        {
            _actions = new ArrayList();
        }

        /// <summary>
        /// Send all the actions that were added for the default player using AddPlayerAction
        /// </summary>
        public IEnumerator SendPlayerActions(Action<string> callback)
        {
            WWW response = new WWW(BuildCustomerUrl("mactions"));   // UTF-8 encoded json file on the server
            yield return response;
            string jsonData = "";
            if (string.IsNullOrEmpty(response.error))
            {
                jsonData = System.Text.Encoding.UTF8.GetString(response.bytes);
                yield return jsonData;
                callback(jsonData);
            }

            HTTP.Request request = new HTTP.Request("POST", BuildCustomerUrl("mactions"), _actions);
            request.Send();
            while (!request.isDone)
            {
                yield return null;
            }
            CleanPlayerActions();
            Debug.Log(request.InfoString(true));
            yield return (request.response.Text);
            callback(request.response.Text);

        }

        /// <summary>
        /// Retrieve a retention action for a given player. 
        /// This action is obtained using a strategy based on the different scores computed by Matchable. 
        /// The strategies can be specifically developped for each customer in collaboration with Matchable's data scientists.
        /// </summary>
        public IEnumerator GetPlayerAdvisor(Action<string> callback)
        {
            WWW response = new WWW(BuildPlayerUrl("advisor"));
            yield return response;
            string jsonData = "";
            if (string.IsNullOrEmpty(response.error))
            {
                jsonData = System.Text.Encoding.UTF8.GetString(response.bytes);
                yield return jsonData;
                callback(jsonData);
            }
        }
    }
}