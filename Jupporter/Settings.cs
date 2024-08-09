using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jupporter
{
    internal interface iniproperties
    {
        string fileIcon { get; set; }
        string procName { get; set; }
        string targetPath { get; set; }
        int refreshCycle { get; set; }
        string runOption { get; set; }
        bool autoRestart { get; set; }
        string autoRestartTime { get; set; }
    }

    public class iniProperties : iniproperties
    {
        private string _fileIcon;
        private string _procName;
        private string _targetPath;
        private int _refreshCycle;
        private string _runOption;
        private bool _autoRestart;
        private string _autoRestartTime;
        public string fileIcon { get { return _fileIcon; } set { _fileIcon = value; } }
        public string procName { get { return _procName; } set { _procName = value; } }
        public string targetPath { get { return _targetPath; } set { _targetPath = value; } }
        public int refreshCycle { get { return _refreshCycle; } set { _refreshCycle = value; } }
        public string runOption { get { return _runOption; } set { _runOption = value; } }
        public bool autoRestart { get { return _autoRestart; } set { _autoRestart = value; } }
        public string autoRestartTime { get { return _autoRestartTime; } set { _autoRestartTime = value; } }
    }

    public class Setting
    {
        public static void createSetting()
        {
            IniFile setting = new IniFile();

            setting["Jupporter"]["fileIcon"] = "icon.png";
            setting["Jupporter"]["procName"] = "cmd.exe";
            setting["Jupporter"]["targetPath"] = "cmd";
            setting["Jupporter"]["refreshCycle"] = "1000";
            setting["Jupporter"]["runOption"] = "";
            setting["Jupporter"]["autoRestart"] = "n";
            setting["Jupporter"]["autoRestartTime"] = "2,0,0";

            setting.Save("./Jupporter.ini");
        }
    }
}
