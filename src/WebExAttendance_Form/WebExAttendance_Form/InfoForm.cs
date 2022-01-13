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
        //state
        private List<string> filters = null;
        public List<PersonName> nameList { private set; get; } = null;
        public Dictionary<PersonName, bool> attendanceDict { private set; get; } = null;


        public InfoForm()
        {
            InitializeComponent();

            SetNameList("NAMELIST.txt");
            SetFilter("FILTERS.txt");
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {

        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!this.Visible)
                ResetAttendance();
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

                    if (PersonName.IsNameInWord(name, filteredWord, out string matchedNameStr))
                        name.SetNameMatched(matchedNameStr);

                    if (name.IsAllNameMatched())                    
                        attendanceDict[name] = true;                    
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
