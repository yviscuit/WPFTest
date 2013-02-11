using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestWPF
{
    public class AnswerDataClass
    {
        private string mark;
        private string text;

        public string Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

    }

    public class DataClass
    {
        private int id;
        private int num;
        private string qs;
        private string right;
        private List<AnswerDataClass> ans;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Num
        {
            get { return num; }
            set { num = value; }
        }

        public string Qs
        {
            get { return qs; }
            set { qs = value; }
        }

        public string Right
        {
            get { return right; }
            set { right = value; }
        }

        public void Add(string _mark, string _text)
        {
            AnswerDataClass a = new AnswerDataClass();
            a.Mark = _mark;
            a.Text = _text;

            ans.Add(a);
        }

        public AnswerDataClass Get(int index)
        {
            AnswerDataClass a = new AnswerDataClass();
            a.Text = ans[index].Text;
            a.Mark = ans[index].Mark;

            return a;
        }

        public int getAnserDataClassSize()
        {
            return ans.Count;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataClass()
        {
            ans = new List<AnswerDataClass>();
        }
    }
}
