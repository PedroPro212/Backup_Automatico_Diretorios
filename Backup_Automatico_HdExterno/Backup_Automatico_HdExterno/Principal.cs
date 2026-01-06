using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Backup_Automatico_HdExterno
{
    public partial class Principal : Form
    {
        private BackupConfig config;
        private readonly string configPath = "BackupConfig.json";

        public Principal()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            config = LoadConfig();

            txtDestino.Text = config.Destino;
            chkExibirCMD.Checked = config.ExibirCMD;
            AtualizarLista();
        }


        public class BackupConfig
        {
            public string Destino { get; set; } = "";
            public List<string> Pastas { get; set; } = new List<string>();

            public bool ExibirCMD { get; set; } = true; // padrão: visível
        }

        private BackupConfig LoadConfig()
        {
            if (!File.Exists(configPath))
                return new BackupConfig();

            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<BackupConfig>(json) ?? new BackupConfig();
        }

        private void SaveConfig()
        {
            string json = JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(configPath, json);
        }

        private void AtualizarLista()
        {
            listBoxPastas.Items.Clear();

            foreach (string pasta in config.Pastas)
                listBoxPastas.Items.Add(pasta);
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (!config.Pastas.Contains(dialog.SelectedPath))
                    {
                        config.Pastas.Add(dialog.SelectedPath);
                        SaveConfig();
                        AtualizarLista();
                    }
                }
            }
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (listBoxPastas.SelectedItem == null)
                return;

            string pasta = listBoxPastas.SelectedItem.ToString();

            config.Pastas.Remove(pasta);
            SaveConfig();
            AtualizarLista();
        }

        private void chkExibirCMD_CheckedChanged(object sender, EventArgs e)
        {
            config.ExibirCMD = chkExibirCMD.Checked;
            SaveConfig();
        }

        void GerarBat(string destino, List<string> pastas, bool exibirCMD)
        {
            var sb = new StringBuilder();

            sb.AppendLine("@echo off");
            sb.AppendLine(":: Verifica se está rodando como administrador");
            sb.AppendLine("net session >nul 2>&1");
            sb.AppendLine("if %errorLevel% neq 0 (");
            sb.AppendLine("    powershell -Command \"Start-Process '%~f0' -Verb RunAs\"");
            sb.AppendLine("    exit");
            sb.AppendLine(")");
            sb.AppendLine();

            // Só minimiza se o usuário NÃO quiser ver o CMD
            if (!exibirCMD)
            {
                sb.AppendLine("if \"%~1\" neq \"min\" (");
                sb.AppendLine("    start \"\" /min \"%~f0\" min");
                sb.AppendLine("    exit");
                sb.AppendLine(")");
                sb.AppendLine();
            }

            sb.AppendLine("set DATA=%date:~-4%%date:~3,2%%date:~0,2%");
            sb.AppendLine();
            sb.AppendLine($"set DESTINO={destino}");
            sb.AppendLine("set LOG=C:\\BackupLogs\\backup_%DATA%.log");
            sb.AppendLine();
            sb.AppendLine("if not exist C:\\BackupLogs mkdir C:\\BackupLogs");
            sb.AppendLine();

            sb.AppendLine(":AGUARDA_HD");
            sb.AppendLine("if not exist E:\\ (");
            sb.AppendLine("    timeout /t 5 >nul");
            sb.AppendLine("    goto AGUARDA_HD");
            sb.AppendLine(")");
            sb.AppendLine();

            foreach (var origem in pastas)
            {
                string nome = new DirectoryInfo(origem).Name;

                // Progresso só aparece com CMD visível
                string flags = exibirCMD
                    ? "/E /R:2 /W:5 /TEE /LOG+:%LOG%"
                    : "/E /R:2 /W:5 /NFL /NDL /NJH /NJS /LOG+:%LOG%";

                sb.AppendLine(
                    $"robocopy \"{origem}\" \"%DESTINO%\\{nome}\" {flags}"
                );
            }

            sb.AppendLine();
            sb.AppendLine("exit");

            File.WriteAllText("backup_automatico.bat", sb.ToString(), Encoding.ASCII);
        }

        private void btnGerarBat_Click(object sender, EventArgs e)
        {
            if (config.Pastas.Count == 0)
            {
                MessageBox.Show("Nenhuma pasta selecionada.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDestino.Text))
            {
                MessageBox.Show("Informe o diretório de destino do backup.");
                return;
            }

            config.Destino = txtDestino.Text.Trim();
            SaveConfig();

            GerarBat(config.Destino, config.Pastas, config.ExibirCMD);

            MessageBox.Show("Arquivo backup_automatico.bat gerado com sucesso!");
        }

    }
}
