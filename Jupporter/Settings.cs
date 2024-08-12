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
        string targetPath { get; set; }
        string runOption { get; set; }
        int refreshCycle { get; set; }
        bool autoRestart { get; set; }
        string autoRestartTime { get; set; }
    }

    public class iniProperties : iniproperties
    {
        private string _fileIcon;
        private string _targetPath;
        private string _runOption;
        private int _refreshCycle;
        private bool _autoRestart;
        private string _autoRestartTime;
        public string fileIcon { get { return _fileIcon; } set { _fileIcon = value; } }
        public string targetPath { get { return _targetPath; } set { _targetPath = value; } }
        public string runOption { get { return _runOption; } set { _runOption = value; } }
        public int refreshCycle { get { return _refreshCycle; } set { _refreshCycle = value; } }
        public bool autoRestart { get { return _autoRestart; } set { _autoRestart = value; } }
        public string autoRestartTime { get { return _autoRestartTime; } set { _autoRestartTime = value; } }
    }

    public class Setting
    {
        public static void createSetting()
        {
            IniFile setting = new IniFile();

            setting["Jupporter"]["fileIcon"] = "icon.png";
            setting["Jupporter"]["targetPath"] = "cmd";
            setting["Jupporter"]["runOption"] = "";
            setting["Jupporter"]["refreshCycle"] = "1000";
            setting["Jupporter"]["autoRestart"] = "n";
            setting["Jupporter"]["autoRestartTime"] = "2,0,0";

            setting.Save("./Jupporter.ini");
        }
    }
}
