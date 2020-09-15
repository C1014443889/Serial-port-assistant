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
            serialPort1.Encoding = Encoding.GetEncoding("GB2312");//支持汉字

           //将跨线程操作的检查关闭，以在数据接收处理时可以操作主线程比如：下拉框，按钮等元素
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < 20; i++)
            {
                comboBox1.Items.Add("COM" + i.ToString());//分配端口号
            }
            //初始化
            comboBox1.Text = "COM1";//端口号默认值
            comboBox2.Text = "9600";//波特率默认值

            //串口事件（注册串口接收处理回调）
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        //串口的接收处理
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //先判断 接收单选框的选择 

            //接收为字符 
            if (!radioButton3.Checked)
            {
                string str = serialPort1.ReadExisting();//字符串的读取
                textBox1.AppendText(str);// 添加接收的字符串   相当于 textBox1.Text+=str;
            }
            else//接收模式为为数值（转为十六进制显示）
            {
                byte[] data=new byte[serialPort1.BytesToRead];//定义缓冲区，因为串口事件是非实时性的(一次性可能接受多个），防止丢数据
                serialPort1.Read(data,0,data.Length);//读取缓冲区的数据

                foreach(byte Member in data)
                {
                    string str = Convert.ToString(Member, 16).ToUpper();//转换为大写的十六进制字符串
                    textBox1.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + " ");//后面的空位用于隔开
                }
                
            }
        }

        //打开串口
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;//串口的端口名
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);//十进制数据转换
                serialPort1.Open();
                button2.Enabled = false;//打开串口按钮不可用
                button3.Enabled = true;//关闭串口按钮可用
            }
            catch
            {
                MessageBox.Show("端口错误，请检查串口！", "提示");
            }

        }

        //关闭串口
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                button2.Enabled = true;//打开串口按钮可用
                button3.Enabled = false;//关闭串口按钮不可用
            }
            catch (Exception err)//一般情况下关闭串口不会出错，所以不需要加处理函数
            {

            }
        }

        //串口发送
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];//定义一个变量 用于一个字节字节的发送

            if (serialPort1.IsOpen && textBox2.Text != "")//判断串口是否打开  且发送区数据不为空
            {
                    if (!radioButton1.Checked)//字符模式发送
                    {
                        try
                        {
                            serialPort1.WriteLine(textBox2.Text); //写数据
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("串口数据写入错误", "提示");
                            serialPort1.Close();//关闭串口
                            button2.Enabled = true;
                            button3.Enabled = false;
                        }
                    }
                    else//数值模式发送
                    {
                        //循环 一个字节一个字节发送
                        for (int i = 0; i < (textBox2.Text.Length - textBox2.Text.Length % 2) / 2; i++)//防止用户输入为奇数个
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(i * 2, 2), 16);
                            serialPort1.Write(Data, 0, 1);//（如果输入字符为0A0BB,则只发送0A,0B）

                        }
                        if (textBox2.Text.Length % 2 != 0)//剩下一位单独处理
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(textBox2.Text.Length - 1, 1), 16);//单独发送B（0B）
                            serialPort1.Write(Data, 0, 1);//发送
                        }

                    }
                
            }
        }
    }
}
