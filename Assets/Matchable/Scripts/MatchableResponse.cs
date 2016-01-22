using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MatchableSDK
{
    /// <summary>
    /// Class for a response from the Matchable API.
    /// </summary>
    public class MatchableResponse
    {
        private object _data;
        private string _text;

        public MatchableResponse(WWW request)
        {
            _text = System.Text.Encoding.UTF8.GetString(request.bytes);
            _data = MJSON.Deserialize(_text);
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
        /// Retrieve the specific key in the _data Hashtable
        /// </summary>
        /// <returns>The Hashtable containing attributes from the JSON response</returns>
        public object GetKey(string key)
        {
            return GetData()[key];
        }
    }
}
