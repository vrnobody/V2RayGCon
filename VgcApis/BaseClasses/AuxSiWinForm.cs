using System.Windows.Forms;

namespace VgcApis.BaseClasses
{
    public class AuxSiWinForm<TForm>
        where TForm : Form, new()
    {
        /*
         * 坑:
         * 如果用类继承会无法使用Form设计器
         * 接口只能用于实例方法,不能用于表态方法
         */


        TForm instance;
        readonly object instanceLocker = new object();

        public AuxSiWinForm() { }

        public TForm GetForm()
        {
            lock (instanceLocker)
            {
                Misc.UI.Invoke(() =>
                {
                    if (instance == null || instance.IsDisposed)
                    {
                        instance = new TForm();
                        instance.FormClosed += (s, a) => instance = null;
                    }
                    else
                    {
                        instance.Activate();
                    }
                });
            }
            return instance;
        }

        public TForm ShowForm()
        {
            var form = GetForm();
            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }
            Misc.UI.Invoke(() => form.Show());
            return form;
        }
    }
}
