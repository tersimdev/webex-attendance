using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace WebExAttendance_Form
{
    public partial class MainForm : Form
    {
        private Frame frame = null;
        private Inference inference = null;

        private const int maxScrollTimes = 20;
        private const int scrollSpeed = 7;


        private List<string> filters = null;
        public List<PersonName> nameList { private set; get; } = null;
        public Dictionary<PersonName, bool> attendanceDict { private set; get; } = null;

        #region Init
        public MainForm()
        {
            InitializeComponent();
            inference = new Inference();

            SetNameList("NAMELIST.txt");
            SetFilter("FILTERS.txt");

            this.TopMost = true;
        }
         
        private void MainForm_Load(object sender, EventArgs e)
        {
            ResetState();
        }
        #endregion

        #region StateHelper
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
        private List<string> LoadFileLines(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = lines[i].Trim().ToLower();
            }
            return lines.ToList();
        }
        private void ResetState()
        {
            if (frame == null)
            {
                frame = new Frame();
                frame.Show();
            }

            frame.FormClosing += FrameFormClosing;
        }

        private void FrameFormClosing(object sender, FormClosingEventArgs args)
        {
            args.Cancel = true;
        }
        #endregion

        #region CV Process
        private Image TakeScreenshot(Rectangle rect)
        {
            //take screenshot of desktop at specified rect of form     
            Image image = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(image);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, frame.Size, CopyPixelOperation.SourceCopy);
            return image;
        }
        //compare with base64, because its easy to code
        private bool ImageIsSame(Image img1, Image img2)
        {
            byte[] img1Bytes;
            byte[] img2Bytes;
            ImageConverter convertor = new ImageConverter();

            img1Bytes = (byte[])convertor.ConvertTo(img1, typeof(byte[]));
            img2Bytes = (byte[])convertor.ConvertTo(img2, typeof(byte[]));

            var img1_64 = Convert.ToBase64String(img1Bytes);
            var img2_64 = Convert.ToBase64String(img2Bytes);

            return string.Equals(img1_64, img2_64);
        }
        private void ProcessWords(List<string> recognisedWords)
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
        private void ResetAttendance()
        {
            foreach (var name in nameList)
            {
                name.ResetNameMatched();
                attendanceDict[name] = false;
            }
        }
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
            res += "\n Possibly Absent: \n";
            foreach (var kv in attendanceDict)
            {
                if (!kv.Value)
                {
                    ++absent;
                    res += absent.ToString() + ". " + kv.Key.fullName + "\n";
                }
            }
            //todo scrollable ui
            labelInfo.Text = res;
        }
        #endregion


        #region ButtonCallbacks

        private void btnDetect_Click(object sender, EventArgs e)
        {
            labelInfo.Text = "Loading...";
            ResetAttendance();

            frame.Hide();
            frame.Click();

            List<Image> images = new List<Image>(5);
            List<string> totalResults = new List<string>(100);
            for (int i = 0; i < maxScrollTimes; ++i)
            {
                var nextImage = TakeScreenshot(frame.DesktopBounds);
                if (images.Count >= 1 && ImageIsSame(images[images.Count - 1], nextImage))
                    break;
                images.Add(nextImage);     
                
                if (maxScrollTimes != 1)
                    frame.Scroll(scrollSpeed);
            }

            foreach (var img in images)
            {
                var task = Task<List<string>>.Factory.StartNew(() =>
                {
                    return inference.DoAnalysis(img);
                });
                totalResults.AddRange(task.Result);
            }

            frame.Show();
            labelInfo.Text = "";
            ProcessWords(totalResults);
        }
        #endregion
    }
}
