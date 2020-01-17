using System.Windows.Forms;

namespace VgcApis.Models.BaseClasses
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
                if (instance == null || instance.IsDisposed)
                {
                    instance = new TForm();
                }
                else
                {
                    instance.Activate();
                }
            }
            return instance;
        }

        public void ShowForm()
        {
            var form = GetForm();
            form.Show();
        }
    }
}
