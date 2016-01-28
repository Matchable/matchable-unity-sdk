using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchableSDK
{
    class MatchableAction
    {
        /// <summary>
        /// Add a new action with the given type and parameters for the default player 
        /// This action is saved locally and can be sent via the SendPlayerActions() method
        /// </summary>
        /// <param name="type">Action type (ex: game_start)</param>
        /// <param name="parameters">Action parameters (JSON string)</param>
        public static Hashtable Create(string type, object parameters)
        {
            Hashtable action = new Hashtable();
            action.Add("player_id", MatchableSettings.GetPlayerId());
            action.Add("type", type);
            action.Add("parameters", parameters);
            action.Add("date", TimeStamp.UnixTimeStampUTC());
            return action;
        }
    }
}
