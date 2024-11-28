using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace RsaEncryptionApp
{
    public partial class EncryptionForm : Form
    {
        private string? _privateRsaKey;
        private string? _publicRsaKey;

        private TextBox? _inputField;
        private TextBox? _resultField;
        private TextBox? _publicKeyField;
        private TextBox? _privateKeyField;

        public EncryptionForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            Text = "RSA Cryptography Tool";
            ClientSize = new System.Drawing.Size(400, 700);
            BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            // Поля для текстового вводу/виводу
            var inputLabel = new Label
            {
                Text = "Input Text:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            _inputField = new TextBox
            {
                Location = new System.Drawing.Point(20, 50),
                Width = 350,
                Height = 80,
                Multiline = true,
                Font = new System.Drawing.Font("Consolas", 10)
            };

            var outputLabel = new Label
            {
                Text = "Result:",
                Location = new System.Drawing.Point(20, 140),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            _resultField = new TextBox
            {
                Location = new System.Drawing.Point(20, 170),
                Width = 350,
                Height = 80,
                Multiline = true,
                Font = new System.Drawing.Font("Consolas", 10),
                BackColor = System.Drawing.Color.WhiteSmoke
            };

            // Поля для ключів
            var publicKeyLabel = new Label
            {
                Text = "Public Key:",
                Location = new System.Drawing.Point(20, 260),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            _publicKeyField = new TextBox
            {
                Location = new System.Drawing.Point(20, 290),
                Width = 350,
                Height = 100,
                Multiline = true,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 10),
                BackColor = System.Drawing.Color.WhiteSmoke
            };

            var privateKeyLabel = new Label
            {
                Text = "Private Key:",
                Location = new System.Drawing.Point(20, 400),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            _privateKeyField = new TextBox
            {
                Location = new System.Drawing.Point(20, 430),
                Width = 350,
                Height = 100,
                Multiline = true,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 10),
                BackColor = System.Drawing.Color.WhiteSmoke
            };

            // Кнопки
            var generateKeyButton = new Button
            {
                Text = "Generate Keys",
                Location = new System.Drawing.Point(20, 550),
                Width = 350,
                Height = 40,
                BackColor = System.Drawing.Color.FromArgb(70, 130, 180),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            generateKeyButton.Click += (_, _) => GenerateKeyPair();

            var encryptButton = new Button
            {
                Text = "Encrypt",
                Location = new System.Drawing.Point(20, 600),
                Width = 350,
                Height = 40,
                BackColor = System.Drawing.Color.FromArgb(46, 139, 87),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            encryptButton.Click += (_, _) => PerformAction(true);

            var decryptButton = new Button
            {
                Text = "Decrypt",
                Location = new System.Drawing.Point(20, 650),
                Width = 350,
                Height = 40,
                BackColor = System.Drawing.Color.FromArgb(220, 20, 60),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            decryptButton.Click += (_, _) => PerformAction(false);

            // Додавання елементів
            Controls.Add(inputLabel);
            Controls.Add(_inputField);
            Controls.Add(outputLabel);
            Controls.Add(_resultField);
            Controls.Add(publicKeyLabel);
            Controls.Add(_publicKeyField);
            Controls.Add(privateKeyLabel);
            Controls.Add(_privateKeyField);
            Controls.Add(generateKeyButton);
            Controls.Add(encryptButton);
            Controls.Add(decryptButton);
        }

        // Генерація ключів
        private void GenerateKeyPair()
        {
            using var rsaProvider = new RSACryptoServiceProvider(2048);
            _publicRsaKey = rsaProvider.ToXmlString(false); // Відкритий ключ
            _privateRsaKey = rsaProvider.ToXmlString(true); // Приватний ключ
            _publicKeyField!.Text = _publicRsaKey;
            _privateKeyField!.Text = _privateRsaKey;
            MessageBox.Show("RSA Keys generated!");
        }

        // Шифрування
        private string PerformEncryption(string plainData, string publicKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            var plainBytes = Encoding.UTF8.GetBytes(plainData);
            var cipherBytes = rsa.Encrypt(plainBytes, false);
            return Convert.ToBase64String(cipherBytes);
        }

        // Дешифрування
        private string PerformDecryption(string cipherText, string privateKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            var encryptedBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = rsa.Decrypt(encryptedBytes, false);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        // Виконання дії
        private void PerformAction(bool encrypt)
        {
            if (string.IsNullOrWhiteSpace(_publicRsaKey) || string.IsNullOrWhiteSpace(_privateRsaKey))
            {
                MessageBox.Show("Generate RSA keys first!");
                return;
            }

            var inputData = _inputField!.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputData))
            {
                MessageBox.Show("Please provide input text.");
                return;
            }

            try
            {
                var outputData = encrypt
                    ? PerformEncryption(inputData, _publicRsaKey)
                    : PerformDecryption(inputData, _privateRsaKey);
                _resultField!.Text = outputData;
            }
            catch
            {
                MessageBox.Show("Error in encryption/decryption. Check the keys and input.");
            }
        }
    }
}
