using System.Windows.Forms;

namespace XLang.TokenExplorer
{
    public partial class LiveEdit : Form
    {
        public LiveEdit()
        {
            InitializeComponent();
        }

        public string Code => rtbCode.Text;
    }
}