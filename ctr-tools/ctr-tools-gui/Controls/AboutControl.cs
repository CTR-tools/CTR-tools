using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();

            labelVersion.Text = Meta.Version;
            signLabel.Text = Meta.GetSignature();
        }

        private void githubLogo_Click(object sender, EventArgs e) => Process.Start(Meta.LinkGithub);

        private void discordLogo_Click(object sender, EventArgs e) => Process.Start(Meta.LinkDiscord);
    }
}
