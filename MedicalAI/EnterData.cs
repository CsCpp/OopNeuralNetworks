using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedicalAI
{
    public partial class EnterData : Form
    {
        private List<TextBox> inputs = new List<TextBox>();
        public EnterData()
        {
            InitializeComponent();
            var propInfo = typeof(Pacient).GetProperties();
            for (int i = 0; i < propInfo.Length; i++)
            {
                var textBox = CreateTextBox(i, propInfo[i]);
                Controls.Add(textBox);
                inputs.Add(textBox);
            }
        }
        public bool? ShowForm()
        {
            var form = new EnterData();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var pacient = new Pacient();
                foreach (var textBox in form.inputs)
                {
                    pacient.GetType().InvokeMember(textBox.Tag.ToString(),
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                        Type.DefaultBinder, pacient, new object[] { textBox.Text });
                }
                var result = Program.Controller.DataNetwork.Predict()?.Output;

                return result == 1.0;
            }
            return null;
        }
        private void EnterData_Load(object sender, EventArgs e)
        {

        }
        private TextBox CreateTextBox(int number, PropertyInfo property)
        {
            var y = (number + 1) * 30;
            var textBox = new TextBox
            {
                Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right),
                Location = new System.Drawing.Point(13, y),
                Name = "textBox" + number,
                Size = new System.Drawing.Size(307, 20),
                TabIndex = number,
                Text = property.Name,
                Tag = property.Name,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic, GraphicsUnit.Point, 204),
                ForeColor = Color.Gray
            };
            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;

            return textBox;
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "")
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic, GraphicsUnit.Point, 204);
                textBox.ForeColor = Color.Gray;
            }
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = "";
                textBox.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
                textBox.ForeColor = Color.Black;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
