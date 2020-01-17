using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.Interfaces
{
    // https://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b6174b62
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
        Image Icon { get; }

        void Run(Interfaces.Services.IApiService api);
        void Show();
        void Cleanup();

        ToolStripMenuItem GetMenu();
    }

}
