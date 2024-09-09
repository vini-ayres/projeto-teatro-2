using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoBilheteria
{
    public partial class Form1 : Form
    {
        private Label lblTexto;
        private ComboBox comboBoxMenu;

        private Label lblFileira;
        private Label lblColuna;
        private TextBox txtFileira;
        private TextBox txtColuna;
        private Button btnReservar;

        private const int fileiras = 15;
        private const int qtdPoltronas = 40;
        private double valorFileira_1a5 = 50.00;
        private double valorFileira_6a10 = 30.00;
        private double valorFileira_11a15 = 15.00;

        private bool[,] poltronas = new bool[fileiras, qtdPoltronas];
        private Button[,] botoesPoltronas = new Button[fileiras, qtdPoltronas];

        public Form1()
        {
            InitializeComponent();
            InitializeMyComponents();
            CarregarOcupacao();
        }

        private void InitializeMyComponents()
        {
            lblTexto = new Label
            {
                Text = "Selecione uma opção:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(120, 20)
            };

            comboBoxMenu = new ComboBox
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(150, 20)
            };
            comboBoxMenu.Items.AddRange(new object[]
            {
                "0 - Finalizar",
                "1 - Reservar poltrona",
                "2 - Mapa de Ocupação",
                "3 - Faturamento"
            });
            comboBoxMenu.SelectedIndexChanged += ComboBoxMenu_SelectedIndexChanged;

            this.Controls.Add(lblTexto);
            this.Controls.Add(comboBoxMenu);
        }

        private void ComboBoxMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxMenu.SelectedItem.ToString())
            {
                case "0 - Finalizar":
                    SalvarOcupacao();
                    MessageBox.Show("Aplicação encerrada.");
                    this.Close();
                    break;

                case "1 - Reservar poltrona":
                    ReservarPoltrona();
                    break;

                case "2 - Mapa de Ocupação":
                    ExibirMapaOcupacao();
                    break;

                case "3 - Faturamento":
                    ExibirFaturamento();
                    break;
            }
        }

        private void ReservarPoltrona()
        {
            lblFileira = new Label
            {
                Text = "Fileira (1-15):",
                Location = new System.Drawing.Point(10, 80),
                Size = new System.Drawing.Size(100, 20)
            };
            txtFileira = new TextBox
            {
                Location = new System.Drawing.Point(120, 80),
                Size = new System.Drawing.Size(50, 20)
            };
            lblColuna = new Label
            {
                Text = "Coluna (1-40):",
                Location = new System.Drawing.Point(10, 110),
                Size = new System.Drawing.Size(100, 20)
            };
            txtColuna = new TextBox
            {
                Location = new System.Drawing.Point(120, 110),
                Size = new System.Drawing.Size(50, 20)
            };
            btnReservar = new Button
            {
                Text = "Reservar",
                Location = new System.Drawing.Point(10, 140),
                Size = new System.Drawing.Size(75, 30)
            };
            btnReservar.Click += BtnReservar_Click;

            this.Controls.Add(lblFileira);
            this.Controls.Add(txtFileira);
            this.Controls.Add(lblColuna);
            this.Controls.Add(txtColuna);
            this.Controls.Add(btnReservar);
        }

        private void BtnReservar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtFileira.Text, out int fileira) && int.TryParse(txtColuna.Text, out int coluna))
            {
                int indiceFileira = fileira - 1;
                int indiceColuna = coluna - 1;

                if (indiceFileira >= 0 && indiceFileira < fileiras && indiceColuna >= 0 && indiceColuna < qtdPoltronas)
                {
                    if (!poltronas[indiceFileira, indiceColuna])
                    {
                        poltronas[indiceFileira, indiceColuna] = true;
                        MessageBox.Show("Poltrona reservada com sucesso!");
                    }
                    else
                    {
                        MessageBox.Show("Poltrona já ocupada.");
                    }
                }
                else
                {
                    MessageBox.Show("Coordenadas inválidas. Insira fileira de 1 a 15 e coluna de 1 a 40.");
                }
            }
            else
            {
                MessageBox.Show("Entrada inválida. Insira números válidos.");
            }
        }

        private void ExibirMapaOcupacao()
        {
            int buttonWidth = 25;
            int buttonHeight = 25;

            for (int i = 0; i < fileiras; i++)
            {
                for (int j = 0; j < qtdPoltronas; j++)
                {
                    if (botoesPoltronas[i, j] == null)
                    {
                        botoesPoltronas[i, j] = new Button
                        {
                            Location = new Point(10 + (j * buttonWidth), 200 + (i * buttonHeight)),
                            Size = new Size(buttonWidth, buttonHeight),
                            BackColor = poltronas[i, j] ? Color.Red : Color.Green,
                            Text = (j + 1).ToString(),
                            Enabled = false
                        };
                        this.Controls.Add(botoesPoltronas[i, j]);
                    }
                    else
                    {
                        botoesPoltronas[i, j].BackColor = poltronas[i, j] ? Color.Red : Color.Green;
                    }
                }
            }
        }

        private void ExibirFaturamento()
        {
            int totalOcupados = 0;
            double totalFaturamento = 0;

            for (int i = 0; i < fileiras; i++)
            {
                for (int j = 0; j < qtdPoltronas; j++)
                {
                    if (poltronas[i, j])
                    {
                        totalOcupados++;
                        if (i < 5) totalFaturamento += valorFileira_1a5;
                        else if (i < 10) totalFaturamento += valorFileira_6a10;
                        else totalFaturamento += valorFileira_11a15;
                    }
                }
            }

            MessageBox.Show($"Qtde de lugares ocupados: {totalOcupados}\nValor da bilheteira: R$ {totalFaturamento:F2}", "Faturamento");
        }

        private void SalvarOcupacao()
        {
            using (StreamWriter sw = new StreamWriter("ocupacao.txt"))
            {
                for (int i = 0; i < fileiras; i++)
                {
                    for (int j = 0; j < qtdPoltronas; j++)
                    {
                        sw.Write(poltronas[i, j] ? "1" : "0");
                    }
                    sw.WriteLine();
                }
            }
        }

        private void CarregarOcupacao()
        {
            if (File.Exists("ocupacao.txt"))
            {
                using (StreamReader sr = new StreamReader("ocupacao.txt"))
                {
                    for (int i = 0; i < fileiras; i++)
                    {
                        string linha = sr.ReadLine();
                        for (int j = 0; j < qtdPoltronas; j++)
                        {
                            poltronas[i, j] = linha[j] == '1';
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
