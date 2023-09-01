using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.Interfaces
{
    // https://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b6174b62
    public interface ILogable
    {
        void Log(string message);
    }

}
