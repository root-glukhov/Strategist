using System.CommandLine;
using System.Diagnostics;

namespace Strategist.Core.Commands;

internal class ReportCommand : Command
{
    public ReportCommand() 
        : base("report", "Visual chart report.")
    {
        this.SetHandler(Handle);
    }

    private void Handle()
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
