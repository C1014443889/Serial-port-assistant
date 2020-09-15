using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace SerialDevelopment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            serialPort1.Encoding = Encoding.GetEncoding("GB2312");//֧�ֺ���

           //�����̲߳����ļ��رգ��������ݽ��մ���ʱ���Բ������̱߳��磺�����򣬰�ť��Ԫ��
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < 20; i++)
            {
                comboBox1.Items.Add("COM" + i.ToString());//����˿ں�
            }
            //��ʼ��
            comboBox1.Text = "COM1";//�˿ں�Ĭ��ֵ
            comboBox2.Text = "9600";//������Ĭ��ֵ

            //�����¼���ע�ᴮ�ڽ��մ���ص���
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        //���ڵĽ��մ���
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //���ж� ���յ�ѡ���ѡ�� 

            //����Ϊ�ַ� 
            if (!radioButton3.Checked)
            {
                string str = serialPort1.ReadExisting();//�ַ����Ķ�ȡ
                textBox1.AppendText(str);// ��ӽ��յ��ַ���   �൱�� textBox1.Text+=str;
            }
            else//����ģʽΪΪ��ֵ��תΪʮ��������ʾ��
            {
                byte[] data=new byte[serialPort1.BytesToRead];//���建��������Ϊ�����¼��Ƿ�ʵʱ�Ե�(һ���Կ��ܽ��ܶ��������ֹ������
                serialPort1.Read(data,0,data.Length);//��ȡ������������

                foreach(byte Member in data)
                {
                    string str = Convert.ToString(Member, 16).ToUpper();//ת��Ϊ��д��ʮ�������ַ���
                    textBox1.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + " ");//����Ŀ�λ���ڸ���
                }
                
            }
        }

        //�򿪴���
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;//���ڵĶ˿���
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);//ʮ��������ת��
                serialPort1.Open();
                button2.Enabled = false;//�򿪴��ڰ�ť������
                button3.Enabled = true;//�رմ��ڰ�ť����
            }
            catch
            {
                MessageBox.Show("�˿ڴ������鴮�ڣ�", "��ʾ");
            }

        }

        //�رմ���
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                button2.Enabled = true;//�򿪴��ڰ�ť����
                button3.Enabled = false;//�رմ��ڰ�ť������
            }
            catch (Exception err)//һ������¹رմ��ڲ���������Բ���Ҫ�Ӵ�����
            {

            }
        }

        //���ڷ���
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];//����һ������ ����һ���ֽ��ֽڵķ���

            if (serialPort1.IsOpen && textBox2.Text != "")//�жϴ����Ƿ��  �ҷ��������ݲ�Ϊ��
            {
                    if (!radioButton1.Checked)//�ַ�ģʽ����
                    {
                        try
                        {
                            serialPort1.WriteLine(textBox2.Text); //д����
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("��������д�����", "��ʾ");
                            serialPort1.Close();//�رմ���
                            button2.Enabled = true;
                            button3.Enabled = false;
                        }
                    }
                    else//��ֵģʽ����
                    {
                        //ѭ�� һ���ֽ�һ���ֽڷ���
                        for (int i = 0; i < (textBox2.Text.Length - textBox2.Text.Length % 2) / 2; i++)//��ֹ�û�����Ϊ������
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(i * 2, 2), 16);
                            serialPort1.Write(Data, 0, 1);//����������ַ�Ϊ0A0BB,��ֻ����0A,0B��

                        }
                        if (textBox2.Text.Length % 2 != 0)//ʣ��һλ��������
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(textBox2.Text.Length - 1, 1), 16);//��������B��0B��
                            serialPort1.Write(Data, 0, 1);//����
                        }

                    }
                
            }
        }
    }
}
