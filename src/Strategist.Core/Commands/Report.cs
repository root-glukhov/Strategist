using System.Diagnostics;

namespace Strategist.Core.Commands;

internal class Report
{
    public Report()
    {
        try
        {
            Process.Start("Strategist.Plugins.Report.exe");
            Process.Start("cmd", "/C start http://localhost:5000/");
        }
        catch
        {
            Console.WriteLine("Для вызова команды 'report' установите пакет Strategist.Plugins.Report");
        }
    }
}
