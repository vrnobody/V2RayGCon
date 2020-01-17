using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.Models.Interfaces
{
    // https://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b6174b62
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
        Image Icon { get; }

        void Run(IServices.IApiService api);
        void Show();
        void Cleanup();

        ToolStripMenuItem GetMenu();
    }

}
