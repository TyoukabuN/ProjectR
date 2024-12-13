using System.Text;
using System;

namespace PJR.Systems
{
    public class Logger : IDisposable
    {
        public string Name = string.Empty;
        public int AppendCount = 0;
        public string LinePrefix = null;

        private StringBuilder s_sb;
        //
        public StringBuilder sb => s_sb;

        public Logger()
        {
            AppendCount = 0;
            s_sb = new StringBuilder();
        }
        public Logger(string name) : this()
        {
            Name = name;
        }
        public Logger(string name, string linePrefix) : this(name)
        {
            LinePrefix = linePrefix;
        }
        public bool Valid
        {
            get
            {
                return s_sb != null;
            }
        }
        public void AppendLine(string context, bool outOfPrefix = false)
        {
            if (s_sb == null)
                return;
            if (string.IsNullOrEmpty(LinePrefix) || outOfPrefix)
                s_sb.AppendLine(context);
            else
                s_sb.AppendLine($"{LinePrefix} {context}");
            ++AppendCount;
        }
        public override string ToString()
        {
            if (s_sb == null)
                return string.Empty;
            return s_sb.ToString();
        }

        public void Dispose()
        {
            if (s_sb != null)
            {
                s_sb.Clear();
                s_sb = null;
            }
        }
        public void Clear()
        {
            if (s_sb != null)
                s_sb.Clear();
            Name = string.Empty;
            AppendCount = 0;
            LinePrefix = null;
        }
    }
}
