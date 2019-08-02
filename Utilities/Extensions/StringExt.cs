using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RandR.Utilities.Extensions
{
    public static class StringExt
    {
        /// <summary>
        /// Trims string after checking for null so no exception is thrown
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeTrim(this String theString) => String.IsNullOrWhiteSpace(theString) ? "" : theString.Trim();
        public static string SafeTrim(this string theString, params char[] trimChars) => String.IsNullOrWhiteSpace(theString) ? "" : theString.Trim(trimChars);
        public static string SareTrimEnd(this string theString, params char[] trimChars) => String.IsNullOrWhiteSpace(theString) ? "" : theString.TrimEnd(trimChars);
        public static string SareTrimStart(this string theString, params char[] trimChars) => String.IsNullOrWhiteSpace(theString) ? "" : theString.TrimStart(trimChars);
        /// <summary>
        /// Trims string to max length after checking for null so no exception is thrown
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static string SafeTrimMax(this String theString, int MaxLength)
        {
            string temp = theString.SafeTrim();
            return temp.Substring(0, Math.Min(MaxLength, temp.Length));
        }

        public static string SafeTrimMax(this String theString, int MaxLength, params char[] trimChars)
        {
            string temp = theString.SafeTrim(trimChars);
            return temp.Substring(0, Math.Min(MaxLength, temp.Length));
        }

        public static string SareTrimEndMax(this string theString, int MaxLength, params char[] trimChars)
        {
            string temp = theString.SareTrimEnd(trimChars);
            return temp.Substring(0, Math.Min(MaxLength, temp.Length));
        }

        public static string SareTrimStartMax(this string theString, int MaxLength, params char[] trimChars)
        {
            string temp = theString.SareTrimStart(trimChars);
            return temp.Substring(0, Math.Min(MaxLength, temp.Length));
        }
            /// <summary>
            /// Returns true if string is NOT null or empty / whitespace
            /// </summary>
            /// <param name="theString"></param>
            /// <returns></returns>
            public static bool IsValid(this String theString) => (false == String.IsNullOrWhiteSpace(theString));

        /// <summary>
        /// Returns true if string is either Null or Whitespace        /// 
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static bool IsEmpty(this String theString) => (String.IsNullOrWhiteSpace(theString));

        /// <summary>
        /// Removes duplicate space characters from a string
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static string RemoveExtraSpaces(this String theString)
        {
            char[] delimiter = { ' ' };

            string[] split = theString.Split(delimiter, 100);

            string newString = split.Where(s => s.Length > 0).Aggregate("", (current, s) => current + (s.Trim() + " "));

            return newString.Trim();

        }

        /// <summary>
        /// Return true if a string does not contain the argument string
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static bool DoesNotContain(this string theString, string searchString) => (false == theString.Contains(searchString));

        public static bool ContainsNumbers(this string theString) => Regex.IsMatch(theString, @"\d");

    }
}
