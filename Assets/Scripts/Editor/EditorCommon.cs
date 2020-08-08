
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Reo
{
    public static class EditorCommon
    {
        public static bool CanCreate()
        {
            return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
        }

        //無効文字
        static readonly string[] INVALID_CHARS =
        {
            " ",  "!",  "\"",   "#",    "$",
            "%",  "&", "\'", "(", ")",
            "_", "=", "^", "~",  "\\",
             "|", "[",  "]",  "{", "}",
            "@", "`", ":", "*", ";",
             "+", "/", "?", ".", "<",
              ">", ",",
        };

        //無効文字を除く文字列を取得
        public static string GetStrWithoutInvalidChars(string str, List<string> exceptionList = null)
        {
            List<string> removeChars = INVALID_CHARS.ToList();
            if(exceptionList != null)
            {
                exceptionList.ForEach(element=>{ removeChars.Remove(element); });
            }
            removeChars.ForEach(item => str = str.Replace(item, string.Empty));
            return str;
        }
    }
}

