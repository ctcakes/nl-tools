using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.Title = "Neverlose Tools";
        Console.OutputEncoding = Encoding.UTF8;
        DrawHeader();

        Console.WriteLine("请选择游戏：");
        Console.WriteLine(" [1] CS2");
        Console.WriteLine(" [2] CSGO Legacy");
        Console.Write("\n> ");

        string choice = Console.ReadLine();
        bool isCsgo = choice == "2";

        Console.Write("\n请输入启动项(留空不使用启动项)： ");
        string launchOptions = Console.ReadLine() ?? "";

        string temp = Path.GetTempPath();
        string nlLog = Path.Combine(temp, "nl.log");

        清空日志(nlLog);

        Console.WriteLine("\n正在监听，请打开注入器注入。");

        等待日志关键字(nlLog, "PreinitSuspended");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nNL注入器初始化成功，准备启动游戏");
        Console.ResetColor();

        结束Steam();
        启动游戏(isCsgo, launchOptions);

        if (isCsgo)
        {
            string csgoLog = Path.Combine(temp, "nl_csgo");

            Console.WriteLine("\n等待NL注入...");
            清空日志(csgoLog);
            等待并提示(csgoLog, "server 0", "NL服务器连接成功！");
            等待并提示(csgoLog, "user...", "NL用户信息获取成功！");
            等待并提示(csgoLog, "...1", "开始注入流程！");
            等待并提示(csgoLog, "Initializing", "✔ 注入成功，模块正在初始化...");
            等待日志关键字(csgoLog, "Finished.");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n✔ 所有注入流程已完成");
            Console.ResetColor();
        }

        Console.WriteLine("\n程序结束，按任意键退出...");
        Console.ReadKey();
    }

    static void 清空日志(string path)
    {
        try
        {
            File.WriteAllText(path, string.Empty);
            //Console.WriteLine($"已清空日志：{Path.GetFileName(path)}");
        }
        catch
        {
            Console.WriteLine($"无法清空日志：{Path.GetFileName(path)}");
        }
    }

    static void 等待日志关键字(string path, string keyword)
    {
        long lastLen = 0;

        while (true)
        {
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                if (fs.Length > lastLen)
                {
                    fs.Seek(lastLen, SeekOrigin.Begin);
                    using var sr = new StreamReader(fs);
                    string content = sr.ReadToEnd();
                    lastLen = fs.Length;

                    if (content.Contains(keyword))
                        return;
                }
            }
            Thread.Sleep(500);
        }
    }

    static void 等待并提示(string path, string keyword, string message)
    {
        long lastLen = 0;
        bool 已提示 = false;

        while (true)
        {
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                if (fs.Length > lastLen)
                {
                    fs.Seek(lastLen, SeekOrigin.Begin);
                    using var sr = new StreamReader(fs);
                    string content = sr.ReadToEnd();
                    lastLen = fs.Length;

                    if (!已提示 && content.Contains(keyword))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(message);
                        Console.ResetColor();
                        已提示 = true;
                        return;
                    }
                }
            }
            Thread.Sleep(500);
        }
    }

    static void 结束Steam()
    {
        foreach (var p in Process.GetProcessesByName("steam"))
        {
            try
            {
                Console.WriteLine("正在结束 Steam.exe...");
                p.Kill();
                p.WaitForExit();
            }
            catch { }
        }
    }

    static void 启动游戏(bool isCsgo, string launchOptions)
    {
        string url = "steam://run/730";

        if (isCsgo)
            url += "//-beta csgo_legacy";

        if (!string.IsNullOrWhiteSpace(launchOptions))
            url += " " + launchOptions;

        Console.WriteLine($"启动游戏：{url}");

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    static void DrawHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================");
        Console.WriteLine("        Neverlose Tools");
        Console.WriteLine("====================================\n");
        Console.ResetColor();
    }
}
