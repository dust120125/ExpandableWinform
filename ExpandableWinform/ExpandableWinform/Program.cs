using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace ExpandableWinform
{

    class Main
    {
        public void run()
        {
            Application.Run(new Form1());
        }
    }
    
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            System.Threading.Mutex appMutex = new System.Threading.Mutex(
                true, System.Diagnostics.Process.GetCurrentProcess().ProcessName, out createNew);

            if (createNew)
            {
                loadedAssembly = new Dictionary<string, Assembly>();
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());
                new Main().run();
            }

            appMutex.Dispose();
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            loadedAssembly.Add(args.LoadedAssembly.FullName, args.LoadedAssembly);
        }

        private static Assembly EXPANDABLE;
        public static Dictionary<string, Assembly> loadedAssembly { get; private set; }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assName = args.Name.Substring(0, args.Name.IndexOf(','));
            if (assName == "Expandable")
            {
                if (EXPANDABLE == null)
                {
                    //读取资源
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MWinTool.Expandable.dll"))
                    {
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);
                        EXPANDABLE = Assembly.Load(bytes);//加载资源文件中的dll,代替加载失败的程序集
                    }
                }
                return EXPANDABLE;
            }
            else
            {
                loadedAssembly.TryGetValue(args.Name, out Assembly assembly);
                return assembly;
            }
        }
    }
}
