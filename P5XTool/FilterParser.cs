using System;
using System.Collections.Generic;
using System.Text;

namespace P5XTool
{
    using FilterToken = (string? Key, object? Value);

    public static class FilterParser
    {
        public static List<FilterToken> Parse(string input)
        {
            var tokens = new List<FilterToken>();
            int i = 0;

            while (i < input.Length)
            {
                SkipWhitespace(input, ref i);

                if (i >= input.Length)
                    break;

                var token = ReadKV(input, ref i);
                if (token.HasValue)
                    tokens.Add(token.Value);
            }

            return tokens;
        }

        public static FilterToken? ReadKV(string input, ref int i)
        {
            var obj = ReadKV2(input, ref i);
            if (obj is FilterToken token)
                return token;
            else if (obj is string key)
                return (null, key);
            else
                return null;
        }

        public static object? ReadKV2(string input, ref int i)
        {
            string key = ReadValue(input, ref i);

            if (i < input.Length && input[i] == ':')
            {
                i++;

                return new FilterToken
                {
                    Key = key,
                    Value = ReadKV2(input, ref i)
                    //Value = ReadValue(input, ref i)
                };
            }
            else
            {
                return key;
            }
        }

        static void SkipWhitespace(string s, ref int i)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i]))
                i++;
        }

        static string ReadUntil(string s, ref int i, params char[] stops)
        {
            int start = i;

            while (i < s.Length && Array.IndexOf(stops, s[i]) == -1)
                i++;

            return s.Substring(start, i - start);
        }

        static string ReadValue(string s, ref int i)
        {
            if (i < s.Length && s[i] == '"')
            {
                i++;
                int start = i;

                while (i < s.Length && s[i] != '"')
                    i++;

                string value = s.Substring(start, i - start);

                if (i < s.Length)
                    i++;

                return value;
            }

            return ReadUntil(s, ref i, ':', ' ', '\t');
        }
    }
}
