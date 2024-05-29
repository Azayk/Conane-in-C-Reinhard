using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reinhard
{
    public partial class Menu : Form
    {
        private bool elementsAdded = false; // Логическая переменная для отслеживания состояния
        private bool elementsAdded2 = false;
        private Label sideLabel;
        private Button sideButton1;
        private Button sideButton2;

        public Menu()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Скрываем текущую форму
            this.Hide();

            // Создаем новую форму Form1
            Form1 form1 = new Form1();

            // Показываем новую форму
            form1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (elementsAdded2)
            {
                // Удаляем текст из button3_Click
                this.Controls.Remove(sideLabel);
                sideLabel.Dispose();
                elementsAdded2 = false; // Устанавливаем состояние как удаленные элементы
            }

            if (!elementsAdded)
            {
                // Создаем текст и кнопки для button2_Click
                sideLabel = new Label();
                sideLabel.Text = "Choose sides";
                sideLabel.Location = new Point(200, 50);
                sideLabel.AutoSize = true;
                sideLabel.Font = new Font("Old English Text MT", 20); // Устанавливаем шрифт 14
                this.Controls.Add(sideLabel);

                // Путь к первому изображению
                string imagePath1 = @"E:\Program Code\Reinhard\Sprite\w.png";

                // Создаем первую кнопку
                sideButton1 = new Button();
                sideButton1.Size = new Size(90, 90);
                sideButton1.Location = new Point(171, 112);
                sideButton1.BackgroundImage = Image.FromFile(imagePath1);
                sideButton1.BackgroundImageLayout = ImageLayout.Stretch;
                sideButton1.BackColor = Color.White; // Устанавливаем задний фон белым
                sideButton1.Click += SideButton1_Click; // Добавляем обработчик клика
                this.Controls.Add(sideButton1);

                // Путь ко второму изображению
                string imagePath2 = @"E:\Program Code\Reinhard\Sprite\b.png";

                // Создаем вторую кнопку
                sideButton2 = new Button();
                sideButton2.Size = new Size(90, 90);
                sideButton2.Location = new Point(293, 112);
                sideButton2.BackgroundImage = Image.FromFile(imagePath2);
                sideButton2.BackgroundImageLayout = ImageLayout.Stretch;
                sideButton2.BackColor = Color.White; // Устанавливаем задний фон белым
                sideButton2.Click += SideButton2_Click; // Добавляем обработчик клика
                this.Controls.Add(sideButton2);

                elementsAdded = true; // Устанавливаем состояние как добавленные элементы
            }
            else
            {
                // Удаляем текст и кнопки из button2_Click
                this.Controls.Remove(sideLabel);
                this.Controls.Remove(sideButton1);
                this.Controls.Remove(sideButton2);

                sideLabel.Dispose();
                sideButton1.Dispose();
                sideButton2.Dispose();

                elementsAdded = false; // Устанавливаем состояние как удаленные элементы
            }
        }

        private void SideButton1_Click(object sender, EventArgs e)
        {
            OpenForm2WithSides(1, 2);
        }

        private void SideButton2_Click(object sender, EventArgs e)
        {
            OpenForm2WithSides(2, 1);
        }

        private void OpenForm2WithSides(int playerside, int noPlayerside)
        {
            // Скрываем текущую форму
            this.Hide();

            // Создаем новую форму Form2 с передачей параметров
            Form2 form2 = new Form2(playerside, noPlayerside);

            // Показываем новую форму
            form2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (elementsAdded)
            {
                // Удаляем текст и кнопки из button2_Click
                this.Controls.Remove(sideLabel);
                this.Controls.Remove(sideButton1);
                this.Controls.Remove(sideButton2);

                sideLabel.Dispose();
                sideButton1.Dispose();
                sideButton2.Dispose();

                elementsAdded = false; // Устанавливаем состояние как удаленные элементы
            }

            if (!elementsAdded2)
            {
                // Добавляем текст и кнопки для button3_Click
                sideLabel = new Label();
                sideLabel.Text = "ПРАВИЛА ИГРЫ:\n" +
                    "Ходить можно, только захватывая противника.\n" +
                    "Захват противника происходит перепрыгиванием\nчерез него\n" +
                    "Можно уничтожить несколько фигур противника,\nно только в одном направлении\n" +
                    "В начале игры нужно убрать по 1 своей фигуре.\n" +
                    "Сначала белые должны убрать 1 фигуру либо с центра, \nлибо с края \n" +
                    " Черные убирают 1 свою фигуру рядом с убранной \nбелыми, но если белые убрали в центре, \nто убрать можно только с цента";
                sideLabel.Location = new Point(150, 50);
                sideLabel.AutoSize = true;
                sideLabel.Font = new Font("Old English Text MT", 8); // Устанавливаем шрифт 14
                this.Controls.Add(sideLabel);
                elementsAdded2 = true;
            }
            else
            {
                // Удаляем текст из button3_Click
                this.Controls.Remove(sideLabel);
                sideLabel.Dispose();
                elementsAdded2 = false; // Устанавливаем состояние как удаленные элементы
            }
        }
    }
}
