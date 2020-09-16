using LZ4;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace unNROLZ
{
    public partial class Form1 : Form
    {
        OpenFileDialog openFileDialog;
        string fileContent = string.Empty;
        string filePath = string.Empty;
        long uncompressedSize;
        int inputLength;
        byte[] outputBuffer;
        byte[] inputBuffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                var fileStream = openFileDialog.OpenFile();
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    uncompressedSize = BigEndian(br.ReadInt32());
                    logBox.ForeColor = Color.White;                    
                    logBox.Text += "\n\r[INFO]: Decompression window (" + uncompressedSize.ToString("X2") + ") [OK]\n\r";
                    outputBuffer = new byte[uncompressedSize];
                    inputBuffer = br.ReadBytes((int)br.BaseStream.Length - 4);
                    inputLength = inputBuffer.Length;
                    try
                    {
                        byte[] decomp = LZ4Codec.Decode(inputBuffer, 0, inputLength, (int)uncompressedSize);
                        File.WriteAllBytes(filePath + "_uncomp.nro", decomp);
                        logBox.ForeColor = Color.Green;
                        logBox.Text += "\n\r[SUCCESS]: Uncompression succesful: (" + filePath + "_uncomp.nro" + ") [OK]\n\r";
                    }
                    catch (Exception)
                    {                        
                        logBox.ForeColor = Color.Red;
                        logBox.Text += "\n\r[ERROR]: Uncompression failed [FAIL]\n\r";                        
                    }                    
                }
            }
            else
            {
                var fileStream = openFileDialog.OpenFile();

                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    inputBuffer = br.ReadBytes((int)br.BaseStream.Length);
                }                    
                inputLength = inputBuffer.Length;
                try
                {
                    byte[] comp = LZ4Codec.EncodeHC(inputBuffer, 0, inputLength);
                    using (BinaryWriter bw = new BinaryWriter(new FileStream(filePath + "_new.nrolz", FileMode.Create)))
                    {
                        bw.Write((int)BigEndian(inputLength));                        
                        bw.Write(comp);
                    }
                    logBox.ForeColor = Color.Green;
                    logBox.Text += "\n\r[SUCCESS]: Compression succesful: (" + filePath + "_new.nrolz" + ") [OK]\n\r";
                }
                catch (Exception)
                {
                    logBox.ForeColor = Color.Red;
                    logBox.Text += "\n\r[ERROR]: Compression failed [FAIL]\n\r";
                    throw;
                }                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (openFileDialog = new OpenFileDialog())
            {
                if (radioButton1.Checked == true)
                    openFileDialog.Filter = "nrolz files (*.nrolz)|*.nrolz|All files (*.*)|*.*";
                else                
                    openFileDialog.Filter = "nro files (*.nro)|*.nro|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    logBox.ForeColor = Color.White;
                    logBox.Text += "\n\r[INFO]: File opened correctly (" + filePath + ") [OK]\n\r";
                    pathBox.Text = filePath;
                }
                else
                {
                    logBox.ForeColor = Color.Red;
                    logBox.Text += "\n\r[INFO]: Can't open file [FAIL]\n\r";
                }
            }
        }

        private long BigEndian(int value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 | (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;

            System.Diagnostics.Process.Start("https://github.com/Megaflan/");
        }
    }    
}
