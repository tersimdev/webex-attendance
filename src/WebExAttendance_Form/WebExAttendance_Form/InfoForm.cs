using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebExAttendance_Form
{
    public partial class InfoForm : Form
    {
        #region Helper Classes
        public class PersonName
        {
            public PersonName(string fullName)
            {
                this.fullName = fullName;
                names = fullName.ToLower().Split(' ');
            }
            //because a person can have more than one word in name, i.e. Tan Xiao Ming
            private string[] names = null;
            public string fullName { private set; get; } = "";

            //x letter same means same name, for those long names that got cut
            private const int substringThreshold = 6;
            private const int stringDistMax = 2;


            //asumes word is in lowercase
            public bool IsNameInWord(string word)
            {
                //its a letter not a word >:(
                if (word.Length <= 1)
                    return false;

                //check if the word contains any part of their name    
                //e.g. 2123123jonsdfsdf(word) in jon(name)
                foreach (string s in names)
                {
                    if (word.Contains(s))
                        return true;
                }

                //check if a substring of the word is a substring of their (long) name
                //e.g.12313jonathan(word) vs jonathanroger(name)
                foreach (string s in names)
                {
                    char secondLtr = s[1];
                    //look for same letter
                    for (int i = 0; i < word.Length; ++i)
                    {
                        if (word[i] != secondLtr)
                            continue;
                        int sameLtrCount = 1;
                        for (int j = 0; j < s.Length; ++j)
                        {
                            if (i + j >= word.Length)
                                break;
                            if (word[i + j] == s[j])
                                ++sameLtrCount;
                        }
                        if (sameLtrCount >= substringThreshold)
                            return true;
                    }
                }


                //check if word is similar to name i.e. mispelt with noise infront
                //e.g. 123jomath(word) vs jonath(name)
                foreach (string s in names)
                {
                    //char firstLtr = s[0];
                    for (int i = 0; i < word.Length; ++i)
                    {
                        int distance = LevenshteinDistance(word, s);

                        if (word.Length >= substringThreshold)
                            if (distance <= stringDistMax)
                                return true;
                    }
                }

                return false;
            }


            //not written by me! thanks google
            private static int LevenshteinDistance(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                // Step 1
                if (n == 0)
                {
                    return m;
                }

                if (m == 0)
                {
                    return n;
                }

                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7
                return d[n, m];
            }
        }
        #endregion



        //state
        private List<string> recognisedWords = null;
        private List<string> filters = null;
        public List<PersonName> nameList { private set; get; } = null;
        public Dictionary<PersonName, bool> attendanceDict { private set; get; } = null;


        public InfoForm()
        {
            InitializeComponent();
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {

        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!this.Visible)
                ResetAttendance();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        #region Public
        public void SetNameList(string filename)
        {
            var rawNameList = LoadFileLines(filename);

            if (attendanceDict == null)
            {
                attendanceDict = new Dictionary<PersonName, bool>(rawNameList.Count);
                foreach (string name in rawNameList)
                {
                    attendanceDict.Add(new PersonName(name), false); // default not here
                }
                nameList = new List<PersonName>();
                nameList.AddRange(attendanceDict.Keys);
            }
        }

        public void SetFilter(string filename)
        {
            filters = LoadFileLines(filename);
        }

        public void ProcessWords(List<string> recognisedWords)
        {
            this.recognisedWords = recognisedWords;

            foreach (string word in recognisedWords)
            {
                string filteredWord = word.ToLower();
                //filter first
                int filterIdx = -9999;
                int filterLength = 0;
                foreach (string filter in filters)
                {
                    int idx = filteredWord.IndexOf(filter);
                    if (idx >= 0)
                    {
                        if (filter.Length > filterLength)
                        {
                            filterIdx = idx;
                            filterLength = filter.Length;
                        }
                    }
                }
                //remove filter from word
                if (filterLength >= 1)
                    filteredWord = filteredWord.Remove(filterIdx, filterLength);

                foreach (PersonName name in nameList)
                {
                    if (name.IsNameInWord(filteredWord))
                    {
                        attendanceDict[name] = true;
                    }
                }
            }

            ShowAttendance();
        }

        public void ResetAttendance()
        {
            foreach (var name in nameList)
                attendanceDict[name] = false;
        }

        #endregion            


        private void ShowAttendance()
        {
            string res = "Present: \n";
            int present = 0, absent = 0;
            foreach (var kv in attendanceDict)
            {
                if (kv.Value)
                {
                    ++present;
                    res += present.ToString() + ". " + kv.Key.fullName + "\n";
                }
            }
            res += "Absent: \n";
            foreach (var kv in attendanceDict)
            {
                if (!kv.Value)
                {
                    ++absent;
                    res += absent.ToString() + ". " + kv.Key.fullName + "\n";
                }
            }
            //todo scrollable ui
            debugLabel.Text = res;
        }


        private List<string> LoadFileLines(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = lines[i].Trim().ToLower();
            }
            return lines.ToList();
        }        
    }
}
