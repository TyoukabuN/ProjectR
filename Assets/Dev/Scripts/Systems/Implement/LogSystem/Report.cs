using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR.ClassExtension;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.Systems.Log
{
    public class Report
    {
        private Logger _logger = null;

        private Logger _curLogger
        {
            get
            {
                if (_groupIndex < 0)
                    return _logger;
                return _groupLogger[_groupIndex];
            }
        }

        private int _id = 1000000;
        //记录log Group用于Append文本的缩进（排版）
        private Logger[] _groupLogger = new Logger[256];
        //记录log Group期间的Append数
        private int[] _groupIndex2AppendCount = new int[256];
        private int _groupIndex = -1;
        //
        public Logger Logger => _logger;
        public int ID => _id;
        public bool Valid
        {
            get
            {
                if (_logger == null)
                    return false;
                return true;
            }
        }
        public int BeginLog()
        {
            if (_logger != null)
            {
                _logger.Dispose();
                _logger = null;
            }
            _groupLogger ??= new Logger[256];
            _groupIndex2AppendCount ??= new int[256];
            _groupIndex = -1;
            _logger = new Logger();
            return ++_id;
        }
        public void EndLog(bool log = true)
        {
            if (!Valid)
                return;
            if (log)
                DisplayLog();
        }
        public GroupScope BeginGroupScope(string groupName) => BeginGroupScope(groupName, null, -1);
        public GroupScope BeginGroupScope(string groupName, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid)
                return null;
            return new GroupScope(this, Hyperlink.GenLink(groupName, href, line));
        }
        public void BeginGroup(string groupName) => BeginGroup(groupName, null, -1);
        public void BeginGroup(string groupName, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid)
                return;
            _groupIndex++;
            if (_groupLogger[_groupIndex] == null)
            {
                _groupLogger[_groupIndex] = new Logger();
            }
            _groupLogger[_groupIndex].Clear();
            _groupLogger[_groupIndex].Name = Hyperlink.GenLink(groupName, href, line);
            _groupLogger[_groupIndex].LinePrefix = GetLoggerLinePrefix();
        }
        public bool EndGroup()
        {
            if (!Valid)
                return false;
            if (_groupIndex < 0)
            {
                Debug.LogError("[CombatSystem.Log.EndGroup] BeginGroup跟EndGroup没有成对调用");
                return false;
            }
            var groupLogger = _groupLogger[_groupIndex];
            int durationAppendCount = groupLogger.AppendCount;
            //
            _groupIndex--;
            //
            if (durationAppendCount > 0)
            {
                AppendLine($"[{groupLogger.Name}]");
                AppendLine(groupLogger.ToString(), true);
            }
            return durationAppendCount > 0;
        }
        public void AppendLine(string context) => AppendLine(null, context, false);
        public void AppendLine(string context, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) => AppendLine(null, context, false, href, line);
        private void AppendLine(string context, bool outOfPrefix) => AppendLine(null, context, outOfPrefix);
        public void AppendLine(string tag, object context) => AppendLine(tag, context.ToString(), false);
        public void AppendLine(string tag, object context, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) => AppendLine(tag, context.ToString(), false, href, line);
        public void AppendLine(string tag, string context) => AppendLine(tag, context, false, null, -1);
        public void AppendLine(string tag, string context, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) => AppendLine(tag, context, false, href, line);
        public void AppendLine(string tag, string context, bool outOfPrefix) => AppendLine(tag, context, outOfPrefix, null, -1);
        public void AppendLine(string tag, string context, bool outOfPrefix, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid) return;
            if (string.IsNullOrEmpty(tag))
            {
                context = Hyperlink.GenLink(context, href, line);
                _curLogger.AppendLine(context, outOfPrefix);
            }
            else
            {
                tag = Hyperlink.GenLink(tag, href, line);
                _curLogger.AppendLine($"{tag} {context}", outOfPrefix);
            }
        }
        public void DisplayLog()
        {
            if (!Valid)
                return;
            Debug.Log(_logger.ToString());
        }

        public string GetTimeStamp()
        {
            return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]") + $"(F:{Time.frameCount})";
        }

        //"GroupA"
        //"L GroupB"
        //"  L GroupC"
        public string GetLoggerLinePrefix()
        {
            if (!Valid)
                return string.Empty;
            if (_groupIndex < 0)
                return string.Empty;
            string prefix = GroupLinePrefix;
            for (int i = 0; i < _groupIndex; i++)
            {
                prefix = $"  {prefix}";
            }
            return prefix;
        }

        public class GroupScope : IDisposable
        {
            private Report report;
            public Report Report => report;
            public GroupScope(Report report) => this.report = report;
            public GroupScope(Report report, string groupName) : this(report)
            {
                report?.BeginGroup(groupName);
            }
            public void Dispose()
            {
                report?.EndGroup();
            }
        }

      

        public const string GroupLinePrefix = "L";

        #region Append func
        public void AppendValueModify(string title, int from, int to) => AppendValueModify(title, from, to, null, -1);
        public void AppendValueModify(string title, int from, int to, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid) return;
            title = Hyperlink.GenLink(title, href, line);
            AppendLine($"[{title}] [{from}]→[{to}]");
        }
        public void AppendValueModify_Mul(string title, float from, float factor) => AppendValueModify_Mul(title, from, factor, null, -1);
        public void AppendValueModify_Mul(string title, float from, float factor, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid) return;
            title = Hyperlink.GenLink(title, href, line);
            AppendLine($"[{title}] [{from}]*[{factor}]→[{from * factor}]");
        }
        public void AppendValueModify_Add(string title, float from, float factor) => AppendValueModify_Add(title, from, factor, null, -1);
        public void AppendValueModify_Add(string title, float from, float factor, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid) return;
            title = Hyperlink.GenLink(title, href, line);
            AppendLine($"[{title}] [{from}]+[{factor}]→[{from + factor}]");
        }
        public void AppendValueModify_Reduce(string title, float from, float factor) => AppendValueModify_Reduce(title, from, factor, null, -1);
        public void AppendValueModify_Reduce(string title, float from, float factor, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid) return;
            title = Hyperlink.GenLink(title, href, line);
            AppendLine($"[{title}] [{from}]-[{factor}]→[{from - factor}]");
        }
        public void AppendValueModify(string title, Enum from, Enum to) => AppendValueModify(title, from.GetEnumNiceName(), to.GetEnumNiceName(), null, -1);
        public void AppendValueModify(string title, Enum from, Enum to, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) => AppendValueModify(title, from.GetEnumNiceName(), to.GetEnumNiceName(), href, line);

        public void AppendValueModify(string title, object from, object to) => AppendValueModify(title, from.ToString(), to.ToString(), null, -1);
        public void AppendValueModify(string title, object from, object to, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) => AppendValueModify(title, from.ToString(), to.ToString(), href, line);
        public void AppendValueModify(string title, string from, string to) => AppendValueModify(title, from.ToString(), to.ToString(), null, -1);
        public void AppendValueModify(string title, string from, string to, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (!Valid) return;
            title = Hyperlink.GenLink(title, href, line);
            AppendLine($"[{title}] [{from}]→[{to}]");
        }
        #endregion
    }
    public struct Hyperlink
    {
        public string href;
        public int line;
        public Hyperlink(string href)
        {
            this.href = href;
            this.line = -1;
        }
        public string GenLink(string context)
        {
            return GenLink(context, href, line);
        }

        public static string GenLink(string context, string href, [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (string.IsNullOrEmpty(href))
                return context;
            if (line > 0)
                return $"<a href={href} line=\"{line}\">{context}</a>";

            return $"<a href={href}>{context}</a>";
        }
    }
}

