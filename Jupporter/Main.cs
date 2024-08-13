using Jupporter.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Jupporter
{
    public partial class Main : Form
    {
        #region Global Variable
        private System.Windows.Threading.DispatcherTimer workProcessTimer = new System.Windows.Threading.DispatcherTimer();
        public static iniProperties IniProperties = new iniProperties();
        private static System.Windows.Forms.Timer tScheduler;
        private Point mousePoint;
        private int targetPID = 0;
        private bool isRunning = true;
        private bool autoStart = false;
        #endregion

        #region Structure definition
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO shinfo, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")] 
        private static extern int RegisterHotKey(int hwnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")] 
        private static extern int UnregisterHotKey(int hwnd, int id);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        const uint SHGFI_ICON = 0x000000100;
        const uint SHGFI_SMALLICON = 0x000000000;
        const uint SHGFI_LARGEICON = 0x000000000;
        #endregion

        public Main(string[] args)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            #region 환경 설정 파싱
            IniFile pairs = new IniFile();
            while (true)
            {
                try
                {
                    if (new FileInfo(Application.StartupPath + "/Jupporter.ini").Exists)
                    {
                        pairs.Load(Application.StartupPath + "/Jupporter.ini");
                        IniProperties.targetPath = pairs["Jupporter"]["targetPath"].ToString();
                        IniProperties.refreshCycle = int.Parse(pairs["Jupporter"]["refreshCycle"].ToString());
                        IniProperties.runOption = pairs["Jupporter"]["runOption"].ToString();
                        IniProperties.autoRestart = pairs["Jupporter"]["autoRestart"].ToString().Equals("Y");
                        IniProperties.autoRestartTime = pairs["Jupporter"]["autoRestartTime"].ToString();
                        break;
                    }
                    else
                    {
                        Setting.createSetting();
                        MessageBox.Show("설정 파일을 생성하였습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("설정 파일 초기화 중 오류가 발생하였습니다. " + e.Message);
                    if (MessageBox.Show("설정 파일이 손상되었습니다. 초기화하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        Setting.createSetting();
                        MessageBox.Show("설정 파일을 초기화하였습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        program_Exit(true);
                    }
                }
            }
            #endregion
            #region 로깅 서비스 등록
            workProcessTimer.Tick += new EventHandler(OnProcessTimedEvent);
            workProcessTimer.Interval = new TimeSpan(0, 0, 0, 0, 32);
            workProcessTimer.Start();
            #endregion
            #region 스케줄러 관련
            tScheduler = new System.Windows.Forms.Timer();
            tScheduler.Interval = CalculateTimerInterval();
            tScheduler.Tick += new EventHandler(tScheduler_Tick);
            tScheduler.Start();
            DevLog.Write((IniProperties.refreshCycle / 1000) + "초 마다 감지", LOG_LEVEL.INFO);
            DevLog.Write("타겟 프로세스 자동 재시작: " + (IniProperties.autoRestart ? "Y" : "n"), LOG_LEVEL.INFO);
            if (IniProperties.autoRestart)
                DevLog.Write("다음 스케줄러 작동시간: " + (DateTime.Now + TimeSpan.FromMilliseconds(tScheduler.Interval)));
            #endregion
            #region args
            if (args.Length != 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].ToLower().Contains("/autostart"))
                    {
                        autoStart = true;
                    }
                }
            }
            #endregion
        }

        #region Form Function
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey((int)this.Handle, 0);
            e.Cancel = true;
            program_Exit(false);
        }
        private void Main_Load(object sender, EventArgs e)
        {
            RegisterHotKey((int)this.Handle, 0, 0x0, (int)Keys.Escape);
            new Thread(delegate ()
            {
                while (!this.IsDisposed)
                {
                    if (this.isRunning)
                    {
                        try
                        {
                            Process[] targetProc = Process.GetProcesses()
                                          .Where(p => string.Equals(p.ProcessName, Path.GetFileName(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase) || string.Equals(p.ProcessName, Path.GetFileNameWithoutExtension(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase))
                                          .ToArray();
                            if (targetProc.Length != 0)
                            {
                                targetPID = targetProc[0].Id;
                                label3.Text = string.Format("PID: {0}", targetProc[0].Id);
                            }
                            else
                            {
                                targetPID = 0;
                                label3.Text = string.Format("No Detected");
                            }
                            if (targetPID == 0)
                            {
                                if (IniProperties.targetPath.Contains("./"))
                                {
                                    Process.Start(Application.StartupPath + @"\" + IniProperties.targetPath.Replace("./", ""), IniProperties.runOption);
                                }
                                else
                                {
                                    Process.Start(IniProperties.targetPath, IniProperties.runOption);
                                }

                                DevLog.Write("타겟 프로세스가 종료되어 재실행하였습니다.");
                            }
                        }
                        catch (Exception e1)
                        {
                            DevLog.Write(e1.Message, LOG_LEVEL.ERROR);
                        }
                    } else
                    {
                        targetPID = 0;
                        label3.Text = string.Format("No Detected");
                    }
                    Thread.Sleep(IniProperties.refreshCycle);
                }
            }).Start();

            SHFILEINFO shinfo;
            int result = SHGetFileInfo(IniProperties.targetPath, 0, out shinfo, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), SHGFI_ICON | SHGFI_LARGEICON);

            if (result != 0 && shinfo.hIcon != IntPtr.Zero)
            {
                using (Icon icon = Icon.FromHandle(shinfo.hIcon))
                {
                    using (Bitmap bitmap = icon.ToBitmap())
                    {
                        pictureBox1.Image = (Image)bitmap.Clone();
                    }
                }

                DestroyIcon(shinfo.hIcon);
            }

            DestroyIcon(shinfo.hIcon);
            label2.Text = Path.GetFileNameWithoutExtension(IniProperties.targetPath);

        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if (this.autoStart)
            {
                try
                {
                    Process[] processes = Process.GetProcesses()
                                  .Where(p => string.Equals(p.ProcessName, Path.GetFileName(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase) || string.Equals(p.ProcessName, Path.GetFileNameWithoutExtension(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase))
                                  .ToArray();

                    foreach (Process process in processes)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    DevLog.Write($"에러 발생: {ex.Message}");
                }
                this.Visible = false;
            }
        }

        #endregion
        #region Function
        private void changeIsRunning(bool isRunning)
        {
            if (this.isRunning)
            {
                try
                {
                    Process[] processes = Process.GetProcesses()
                                  .Where(p => string.Equals(p.ProcessName, Path.GetFileName(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase) || string.Equals(p.ProcessName, Path.GetFileNameWithoutExtension(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase))
                                  .ToArray();

                    foreach (Process process in processes)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    DevLog.Write($"에러 발생: {ex.Message}");
                }

                DevLog.Write("사용자에 의해 타겟프로그램을 종료하였습니다.", LOG_LEVEL.INFO);

                panel3.BackgroundImage = Resources.off;
                this.isRunning = false;
                this.Show();
                this.Activate();
            }
            else
            {
                panel3.BackgroundImage = Resources.on;
                this.isRunning = true;
                this.Hide();
            }
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            if (this.isRunning)
            {
                changeIsRunning(false);
            }
            else
            {
                changeIsRunning(true);
            }
        }

        private void 프로그램종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            program_Exit(false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + @"\Jupporter.ini");
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {

            if (this.Visible && e.Button == MouseButtons.Left)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X),
                    this.Top - (mousePoint.Y - e.Y));
            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == (int)0x312)
            {
                if (m.WParam == (IntPtr)0x0)
                {
                    if (isRunning)
                    {
                        changeIsRunning(false);
                    } else
                    {
                        changeIsRunning(true);
                    }
                }
            }
        }

        private int CalculateTimerInterval()
        {
            string[] timeStr = IniProperties.autoRestartTime.Split(',');
            int[] time = new int[3];
            for (int i = 0; i < 2; i++)
            {
                time[i] = int.Parse(timeStr[i]);
            }
            DateTime timeTaken = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time[0], time[1], time[2]).AddDays(1);

            TimeSpan curTime = timeTaken - DateTime.Now;

            Debug.WriteLine(timeTaken.ToString("yyyy-MM-dd HH:mm:ss"));
            Debug.WriteLine(curTime);
            return (int)curTime.TotalMilliseconds;
        }

        private void tScheduler_Tick(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            try
            {
                tScheduler.Interval = CalculateTimerInterval();
            }
            catch (Exception) { tScheduler.Dispose(); DevLog.Write("스케줄러 작동중 오류가 발생하여 비활성화 하였습니다."); }

            if (IniProperties.autoRestart)
            {
                try
                {
                    Process[] processes = Process.GetProcesses()
                                  .Where(p => string.Equals(p.ProcessName, Path.GetFileName(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase) || string.Equals(p.ProcessName, Path.GetFileNameWithoutExtension(IniProperties.targetPath), StringComparison.OrdinalIgnoreCase))
                                  .ToArray();

                    foreach (Process process in processes)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    DevLog.Write($"에러 발생: {ex.Message}");
                }
                DevLog.Write("스케줄러에 의해 타겟프로그램을 종료하였습니다.", LOG_LEVEL.INFO);
            }
        }
        private void OnProcessTimedEvent(object sender, EventArgs e)
        {
            int logWorkCount = 0;

            while (true)
            {
                string msg;

                if (DevLog.GetLog(out msg))
                {
                    textBox1.AppendText(string.Format("{0}\r\n", msg));
                }
                else
                {
                    break;
                }

                if (logWorkCount > 7)
                {
                    break;
                }
            }
        }
        private void program_Exit(bool forceExit)
        {
            if (!forceExit)
            {
                if (MessageBox.Show(this, "Jupporter를 종료하시겠습니까?", "경고", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    try
                    {
                        Application.ExitThread();
                        Environment.Exit(0);
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                try
                {
                    Application.ExitThread();
                    Environment.Exit(0);
                }
                catch (Exception) { }
            }
        }
        #endregion
    }
}
