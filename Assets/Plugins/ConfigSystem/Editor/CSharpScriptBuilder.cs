#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEditor;

public class CSharpScriptBuilder
{
    public StringBuilder sb = new StringBuilder();
    public int tabCount = 0;

    public string className;
    public string fileName;
    public string filePath;

    public CSharpScriptBuilder(string filePath)
    {
        this.filePath = filePath;
    }

    public ScopeHandle BeginScope()
    {
        return new ScopeHandle(this, true, false);
    }
    public void EndScope()
    {
        AppendLine("}");
        tabCount--;
    }
    private string TabStr()
    {
        var stringBuilder = new StringBuilder();
        string tabStr = "\t";
        for (int i = 0; i < tabCount; i++)
        {
            stringBuilder.Append(tabStr);
        }

        return stringBuilder.ToString();
    }

    #region Appends
    public void AppendEmptyLine()
    {
        sb.AppendLine(TabStr());
    }
    public void AppendLine(string str)
    {
        sb.AppendLine($"{TabStr()}{str}");
    }

    public void AppendUsing(string nameSpace)
    {
        AppendLine($"using {nameSpace};");
    }

    public ScopeHandle BeginNameSpace(string nameSpace)
    {
        AppendLine($"namespace {nameSpace}");
        return new ScopeHandle(this, true, false);
    }

    public ScopeHandle BeginClass(string className, string modifier = "public", string baseClass = "")
    {
        if (!string.IsNullOrEmpty(baseClass))
        {
            AppendLine($"{modifier} class {className} : {baseClass}");
        }
        else
        {
            AppendLine($"{modifier} class {className}");
        }
        return new ScopeHandle(this, true, false);
    }

    public ScopeHandle BeginStruct(string structName, string modifier = "public")
    {
        AppendLine($"{modifier} struct {structName}");
        return new ScopeHandle(this, true, false);
    }

    #endregion

    public void Gen()
    {
        File.WriteAllText(filePath, sb.ToString());
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        sb.Clear();
    }

    public ScopeHandle Tab(bool addScope = false, bool endPara = false)
    {
        return new ScopeHandle(this, addScope, endPara);
    }

    public class ScopeHandle : System.IDisposable
    {
        private CSharpScriptBuilder builder;
        private bool addScope = false;
        private bool endPara = false;

        public ScopeHandle(CSharpScriptBuilder builder, bool addScope = false, bool endPara = false)
        {
            this.builder = builder;
            this.addScope = addScope;
            this.endPara = endPara;
            if (addScope) builder.AppendLine("{");
            builder.tabCount++;
        }

        public void Dispose()
        {
            builder.tabCount--;
            if (addScope) builder.AppendLine("}");
            if (endPara) builder.AppendLine("");
        }
    }
}
#endif //UNITY_EDITOR