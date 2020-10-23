using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XLang.TokenExplorer
{
    public partial class LiveEdit : Form
    {

        public string Code => rtbCode.Text;

        public LiveEdit()
        {
            InitializeComponent();
        }
    }
}
