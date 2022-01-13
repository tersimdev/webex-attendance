using System;
using System.Collections.Generic;

namespace WebExAttendance_Form
{
    public class PersonName
    {
        //because a person can have more than one word in name, i.e. Tan Xiao Ming
        private string[] names = null;
        public string fullName { private set; get; } = "";

        public bool isLongName { private set; get; } = false;
        public bool hasShortNamePart { private set; get; } = false;

        private Dictionary<string, bool> nameMatched = null;

        private string shortNameCombined = "";
        private const int longFullNameLen = 15;
        private const int longPartNameLen = 6;
        private const int shortPartNameLen = 2;
        private const int stringDistMax = 3;
        private const int stringMatchedMin = 2;
        private const int partNameMatchedDiff = 1;

        public PersonName(string fullName)
        {
            this.fullName = fullName;
            names = fullName.ToLower().Split(' ');
            nameMatched = new Dictionary<string, bool>(names.Length);

            //check if long name, because if long name then only need partial match
            // short chinese names dont count! e.g. tan xiao ming has 13 chars, need all to match for uniqueness!
            // two-letter chinese names also have some issues, because the dtectioon combines them, so flag them here
            //if (names.Length >= 4)
            //{
            //    if (fullName.Length >= longFullNameLen)
            //        isLongName = true;
            //}
            if (names.Length >= 3)
            {
                if (names.Length >= 4 && fullName.Length >= longFullNameLen)
                        isLongName = true;
                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i].Length >= longPartNameLen)
                        isLongName = true;

                    if (names[i].Length <= shortPartNameLen) //has a two letter word in name
                    {
                        if (i - 1 >= 0)
                            shortNameCombined = names[i-1] + names[i];
                        else if (i + 1 < names.Length)
                            shortNameCombined = names[i] + names[i + 1];

                        if (shortNameCombined != "")
                        {
                            nameMatched.Add(shortNameCombined, false);
                            hasShortNamePart = true;
                        }
                    }
                }
            }
            else
                isLongName = false;

            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = names[i].Trim();
                nameMatched.Add(names[i], false);
            }
        }

        #region State
        public bool IsAllNameMatched()
        {
            int numMatched = 0;
            foreach (var b in nameMatched.Values)
                if (b) ++numMatched;

            if (isLongName | hasShortNamePart)
            {
                return numMatched >= names.Length - partNameMatchedDiff;
            }
            else
                return numMatched >= names.Length;            
        }
        public void ResetNameMatched()
        {
            foreach (var s in names)
                nameMatched[s] = false;
        }

        public void SetNameMatched(string name)
        {
            if (nameMatched.ContainsKey(name))
                nameMatched[name] = true;
        }
        #endregion

        //asumes word is in lowercase
        public static bool IsNameInWord(PersonName personName, string word, out string name)
        {
            name = "";
            //its a letter not a word >:(
            if (word.Length <= 1)
                return false;

            List<string> nameList = new List<string>(personName.names);
            if (personName.hasShortNamePart)
                nameList.Add(personName.shortNameCombined);

            //metric method - check for two values:
            foreach (string s in nameList)
            {
                int matchedLen = GetStringMetric(s, word);
                bool similar = matchedLen >= Math.Min(s.Length, stringMatchedMin) && 
                    Math.Max(0, s.Length - matchedLen) + Math.Max(0, word.Length - matchedLen) <= stringDistMax;
                Console.WriteLine("{2}: {0}, {1}", s, word, similar);
                if (similar)
                {
                    name = s;
                    return similar;
                }
            }

            return false; //just in case
        }

        private static int GetStringMetric(string orig, string comp)
        {
            //e.g. ter vs 123ter => 3 matched
            //e.g. ter vs tre => 3 0 matched
            //e.g. terecne vs trence => 5 matched

            int matchedLength = 0;
            int[] matchedIdxComp = new int[orig.Length];

            //find indexes of each letter in orig
            for (int i = 0; i < orig.Length; ++i)
            {
                for (int j = 0; j < comp.Length; ++j)
                {
                    if (orig[i] == comp[j])
                    {
                        matchedIdxComp[i] = j;
                        break;
                    }
                }
            }

            for (int k = 0; k < matchedIdxComp.Length; ++k)
            {
                int len = 0;
                int origIdx = 0;
                int compIdx = Math.Max(0, matchedIdxComp[k] - k);
                for (int j = 0; ; ++j)
                {
                    //bounds check
                    if (compIdx + j >= comp.Length || origIdx + j >= orig.Length)
                        break;

                    if (orig[origIdx + j] == comp[compIdx + j])
                    {
                        ++len;
                        if (len > matchedLength)
                            matchedLength = len;
                    }
                    else
                        break;
                }
            }

            return matchedLength;
        }
    }
}
