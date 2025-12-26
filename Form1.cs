using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnappyWinscard;
using NLog;

namespace NFC_ACR122U_Reader_Writer
{
    public partial class Form1 : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public CardIo CardIo { get; set; }
        
        public Form1()
        {
            InitializeComponent();
            Logger.Info("Application started");
            Initialize();
        }

        private void Initialize()
        {
            Logger.Debug("Initializing card reader");
            
            CardIo = new CardIo();
            CardIo.ReaderStateChanged += OnReaderStateChanged;

            if (ConnectCard())
            {
                CardUIdTextbox.Text = GetCardUId(CardIo);
                StatusTextBox.Text = GetCardStatus(CardIo);
                SubStatusTextBox.Text = GetCardSubStatus(CardIo);
                Logger.Info($"Card connected successfully. UID: {CardUIdTextbox.Text}");
            }
            else
            {
                Logger.Warn("No card detected during initialization");
            }
        }

        private void OnReaderStateChanged(CardIo.ReaderState readerState)
        {
            Logger.Debug($"Reader state changed: {readerState}");
            ChangeStatus();
            ChangeCardStatus(textBlockReaderState, readerState.ToString());
        }

        private static void ThreadSafe(Action callback, Form form)
        {
            var worker = new BackgroundWorker();
            worker.RunWorkerCompleted += (obj, e) =>
            {
                if (form.InvokeRequired)
                    form.Invoke(callback);
                else
                    callback();
            };
            worker.RunWorkerAsync();
        }


        public void ChangeStatus()
        {
            if (ConnectCard())
            {
                ChangeStatus(CardUIdTextbox, GetCardUId(CardIo));
                ChangeStatus(StatusTextBox, GetCardStatus(CardIo));
                ChangeStatus(SubStatusTextBox, GetCardSubStatus(CardIo));
                ChangeStatusColor(StatusColorLabel, Color.MediumSeaGreen);
                Logger.Info($"Card status updated - UID: {GetCardUId(CardIo)}");
            }
            else
            {
                ChangeStatus(CardUIdTextbox, "Not connected.");
                ChangeStatus(StatusTextBox, "Not connected.");
                ChangeStatus(SubStatusTextBox, "Not connected.");
                ChangeStatusColor(StatusColorLabel, Color.Red);
                Logger.Debug("Card disconnected");
            }
        }


        public Label ChangeCardStatus(Label label, string mesagge)
        {
            ThreadSafe(() => { label.Text = mesagge; }, this);

            return label;
        }

        public TextBox ChangeStatus(TextBox textBox, string message)
        {
            ThreadSafe(() => { textBox.Text = message; }, this);

            return textBox;
        }

        public Label ChangeStatusColor(Label label, Color color)
        {
            ThreadSafe(() => { label.BackColor = color; }, this);

            return label;
        }

        private bool ConnectCard()
        {
            try
            {
                var result = CardIo.ConnectCard();
                Logger.Debug($"Connect card attempt: {(result ? "Success" : "Failed")}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error connecting to card");
                return false;
            }
        }

        private string GetCardUId(CardIo cardIo)
        {
            try
            {
                var uid = cardIo.GetCardUID();
                Logger.Debug($"Retrieved card UID: {uid}");
                return uid;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting card UID");
                return "Error";
            }
        }

        private string GetCardStatus(CardIo cardIo)
        {
            return cardIo.StatusText;
        }

        private string GetCardSubStatus(CardIo cardIo)
        {
            return cardIo.SubStatusText;
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            var blockNumber = Convert.ToInt32(BlockNumericUpDown.Value);
            Logger.Info($"Read button clicked for block {blockNumber}");
            var readedText = ReadBlockAsText(blockNumber);

            ReadedTextTextBox.Text = readedText;
            Logger.Info($"Block {blockNumber} read successfully. Data: {readedText}");
        }

        private string ReadBlockAsText(int blockNumber)
        {
            try
            {
                Logger.Debug($"Reading block {blockNumber} as text");
                var readedBytes = Read(Convert.ToByte(blockNumber));
                var text = ConvertBytes(readedBytes, TextFormat.Stretched) ?? "";
                return text.Trim().Replace(" ", string.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error reading block {blockNumber} as text");
                return "";
            }
        }

        private IEnumerable<byte> Read(byte block)
        {
            try
            {
                Logger.Debug($"Reading block {block} with authentication key 0x60:0x0");
                var readedBytes = CardIo.ReadCardBlock(block, (byte)0x60, (byte)0x0);
                Logger.Debug($"Successfully read {readedBytes?.Count() ?? 0} bytes from block {block}");
                return readedBytes;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error reading block {block}");
                return Enumerable.Empty<byte>();
            }
        }

        private static string ConvertBytes(IEnumerable<byte> bytes, TextFormat textFormat)
        {
            try
            {
                var text = bytes.Aggregate("",
                    (s, b) =>
                    {
                        switch (textFormat)
                        {
                            case TextFormat.Hex:
                                return $"{s}{b:X2} ";
                            case TextFormat.Stretched:
                                return $"{s}{(char)b}  ";
                            case TextFormat.Normal:
                                return $"{s}{(char)b}";
                            default:
                                throw new ArgumentOutOfRangeException(nameof(textFormat), textFormat, null);
                        }
                    });
                return text;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting bytes to text");
                return "";
            }
        }

        private enum TextFormat
        {
            Hex,
            Normal,
            Stretched
        }


        private static byte[] GetBytes(string text, int length)
        {
            if (text.Length <= length)
                text += new string('\0', length - text.Length);
            else
                text = text.Substring(0, length);


            return text
                .ToCharArray()
                .Select(c => (byte)c)
                .ToArray();
        }

        private bool Write(string text, int blockNumber, int length)
        {
            try
            {
                Logger.Info($"Writing to block {blockNumber}: '{text}' (length: {text.Length})");
                var bytesToWrite = GetBytes(text, length);
                var block = Convert.ToByte(blockNumber);
                CardIo.WriteCardBlock(bytesToWrite, block, (byte)0x60, (byte)0x0);
                Logger.Info($"Successfully wrote {bytesToWrite.Length} bytes to block {blockNumber}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error writing to block {blockNumber}");
                return false;
            }
        }



        private void WriteButton_Click(object sender, EventArgs e)
        {
            Logger.Info("Write button clicked");
            
            if (ConnectCard())
            {
                var blockNumber = Convert.ToInt32(BlockNumericUpDown.Value);
                var textToWrite = TextToWriteTextBox.Text.Trim();
                Logger.Debug($"Attempting to write '{textToWrite}' to block {blockNumber}");
                var succes = Write(textToWrite, blockNumber, 16);

                MessageBox.Show(succes ? "Ok" : "Error");
                Logger.Info($"Write operation result: {(succes ? "Success" : "Failed")}");
            }
            else
            {
                MessageBox.Show("No Connected");
                Logger.Warn("Write attempted but no card connected");
            }
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            var blockNumber = Convert.ToInt32(BlockNumericUpDown.Value);
            Logger.Info($"Read button clicked for block {blockNumber}");
            var readedText = ReadBlockAsText(blockNumber);

            ReadedTextTextBox.Text = readedText;
            Logger.Info($"Block {blockNumber} read successfully. Data: {readedText}");
        }
    }
}
