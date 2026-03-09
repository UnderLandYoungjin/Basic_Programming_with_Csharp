<img width="806" height="483" alt="image" src="https://github.com/user-attachments/assets/7d7b6689-6d3f-453c-9916-f061405265ee" />


```csharp
namespace WinFormsApp9
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text.Trim();

            if (int.TryParse(input, out int intVal))
            {
                if (intVal > 0)
                {
                    label1.Text = "양의정수";
                }
                else if (intVal == 0)
                {
                    label1.Text = "0입니다.";
                }
                else
                {
                    label1.Text = "음의정수";
                }
            }

            else if (double.TryParse(input, out double dblVal))
            {
                if (dblVal > 0)
                {
                    label1.Text = "양의실수";
                }
                else if (dblVal == 0)
                {
                    label1.Text = "0입니다.";
                }
                else
                {
                    label1.Text = "음의실수";
                }
            }
        }
    }
}
```
