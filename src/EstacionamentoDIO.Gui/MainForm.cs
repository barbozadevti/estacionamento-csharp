using EstacionamentoDIO.Core;

namespace EstacionamentoDIO.Gui;

public class MainForm : Form
{
    private readonly Estacionamento estacionamento;

    private readonly TextBox txtPlaca = new();
    private readonly Button btnAdicionar = new();
    private readonly Button btnRemover = new();
    private readonly Label lblVagas = new();
    private readonly Label lblStatus = new();
    private readonly DataGridView dgvVeiculos = new();
    private readonly System.Windows.Forms.Timer timerAtualizacao = new();

    public MainForm()
    {
        var pastaDados = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EstacionamentoDIO");
        Directory.CreateDirectory(pastaDados);
        var arquivoDados = Path.Combine(pastaDados, "estacionamento.json");

        estacionamento = new Estacionamento(
            precoInicial: 5.00m,
            precoPorHora: 2.00m,
            vagasTotais: 10,
            arquivoDados: arquivoDados);

        Text = "Sistema de Estacionamento - DIO";
        Width = 760;
        Height = 520;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(640, 420);
        Font = new Font("Segoe UI", 9.5f);

        ConstruirLayout();
        AtualizarTela();

        timerAtualizacao.Interval = 1000;
        timerAtualizacao.Tick += (_, _) => AtualizarTela();
        timerAtualizacao.Start();
    }

    private void ConstruirLayout()
    {
        var painelTopo = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(12) };

        var lblPlaca = new Label { Text = "Placa do veículo:", AutoSize = true, Location = new Point(12, 18) };

        txtPlaca.Location = new Point(150, 14);
        txtPlaca.Width = 160;
        txtPlaca.CharacterCasing = CharacterCasing.Upper;
        txtPlaca.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                Adicionar();
                e.SuppressKeyPress = true;
            }
        };

        btnAdicionar.Text = "Adicionar veículo (entrada)";
        btnAdicionar.Location = new Point(330, 12);
        btnAdicionar.Width = 190;
        btnAdicionar.Height = 30;
        btnAdicionar.Click += (_, _) => Adicionar();

        btnRemover.Text = "Remover veículo (saída)";
        btnRemover.Location = new Point(330, 48);
        btnRemover.Width = 190;
        btnRemover.Height = 30;
        btnRemover.Click += (_, _) => Remover();

        lblVagas.AutoSize = true;
        lblVagas.Location = new Point(12, 55);
        lblVagas.Font = new Font(Font, FontStyle.Bold);

        painelTopo.Controls.Add(lblPlaca);
        painelTopo.Controls.Add(txtPlaca);
        painelTopo.Controls.Add(btnAdicionar);
        painelTopo.Controls.Add(btnRemover);
        painelTopo.Controls.Add(lblVagas);

        dgvVeiculos.Dock = DockStyle.Fill;
        dgvVeiculos.ReadOnly = true;
        dgvVeiculos.AllowUserToAddRows = false;
        dgvVeiculos.AllowUserToDeleteRows = false;
        dgvVeiculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvVeiculos.MultiSelect = false;
        dgvVeiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvVeiculos.Columns.Add("Placa", "Placa");
        dgvVeiculos.Columns.Add("Entrada", "Hora de entrada");
        dgvVeiculos.Columns.Add("Tempo", "Tempo estacionado");

        var painelStatus = new Panel { Dock = DockStyle.Bottom, Height = 40, Padding = new Padding(12, 8, 12, 8) };
        lblStatus.Dock = DockStyle.Fill;
        painelStatus.Controls.Add(lblStatus);

        Controls.Add(dgvVeiculos);
        Controls.Add(painelStatus);
        Controls.Add(painelTopo);
    }

    private void Adicionar()
    {
        var sucesso = estacionamento.AdicionarVeiculo(txtPlaca.Text, out var mensagem);
        MostrarStatus(mensagem, sucesso);
        if (sucesso)
        {
            txtPlaca.Clear();
        }
        txtPlaca.Focus();
        AtualizarTela();
    }

    private void Remover()
    {
        var placa = ObterPlacaSelecionadaOuDigitada();
        if (string.IsNullOrWhiteSpace(placa))
        {
            MostrarStatus("Selecione um veículo na lista ou digite a placa no campo acima.", sucesso: false);
            return;
        }

        var sucesso = estacionamento.RemoverVeiculo(placa, out _, out var mensagem);
        MostrarStatus(mensagem, sucesso);
        if (sucesso)
        {
            txtPlaca.Clear();
        }
        AtualizarTela();
    }

    private string ObterPlacaSelecionadaOuDigitada()
    {
        if (!string.IsNullOrWhiteSpace(txtPlaca.Text))
        {
            return txtPlaca.Text;
        }

        if (dgvVeiculos.SelectedRows.Count > 0)
        {
            return dgvVeiculos.SelectedRows[0].Cells["Placa"].Value?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }

    private void MostrarStatus(string mensagem, bool sucesso)
    {
        lblStatus.Text = mensagem;
        lblStatus.ForeColor = sucesso ? Color.DarkGreen : Color.DarkRed;
    }

    private void AtualizarTela()
    {
        lblVagas.Text = $"Vagas disponíveis: {estacionamento.VagasDisponiveis}/{estacionamento.VagasTotais}";

        var placaSelecionada = dgvVeiculos.SelectedRows.Count > 0
            ? dgvVeiculos.SelectedRows[0].Cells["Placa"].Value?.ToString()
            : null;

        dgvVeiculos.Rows.Clear();
        foreach (var veiculo in estacionamento.ListarVeiculos())
        {
            var tempo = DateTime.Now - veiculo.HoraEntrada;
            var indiceLinha = dgvVeiculos.Rows.Add(
                veiculo.Placa,
                veiculo.HoraEntrada.ToString("dd/MM/yyyy HH:mm:ss"),
                tempo.ToString(@"hh\:mm\:ss"));

            if (veiculo.Placa == placaSelecionada)
            {
                dgvVeiculos.Rows[indiceLinha].Selected = true;
            }
        }
    }
}
