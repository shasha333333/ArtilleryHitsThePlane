using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArtilleryHitsThePlane.Forms
{
    public partial class PlaneCountForm : Form
    {
        public int SelectedPlaneCount { get; private set; }

        public PlaneCountForm()
        {
            InitializeComponent();
        }

        private void PlaneCountForm_Load(object sender, EventArgs e)
        {

        }

        private void okButton_Click_1(object sender, EventArgs e)
        {
            if (int.TryParse(planeCountTextBox.Text, out int count) && count > Utils.Constants.PlaneNumMin && count < Utils.Constants.PlaneNumMax)
            {
                SelectedPlaneCount = count;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show($"请输入{Utils.Constants.PlaneNumMin}到{Utils.Constants.PlaneNumMax}合法数字。\n默认为{Utils.Constants.PlaneNumMin}。", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
