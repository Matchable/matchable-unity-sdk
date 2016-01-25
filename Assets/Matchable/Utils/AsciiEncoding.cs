﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchableSDK
{
    /// <summary>
    /// Custom ASCII encoder
    /// </summary>
    public class AsciiEncoding
    {
        public static byte[] StringToAscii(string s)
        {
            byte[] retval = new byte[s.Length];
            for (int ix = 0; ix < s.Length; ++ix)
            {
                char ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte)ch;
                else retval[ix] = (byte)'?';
            }
            return retval;
        }
    }
}
