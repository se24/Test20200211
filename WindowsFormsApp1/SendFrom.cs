using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class SendFrom : Form
    {
        public SendFrom()
        {
            InitializeComponent();
        }
        IConnection Connection;

        private  static String EXCHANGE_NAME = "test_exchange_fanout";
        private void button1_Click(object sender, EventArgs e)
        {
            Test();
           
        }

        public String ShowContion()
        {
            String TempRes = "";
            MQContion TempMQContion = new MQContion();
            String TmepUrl = txtHostName.Text;
            String TempUserName = txtUserName.Text;
            String TempUserPassWord = txtPassword.Text;
            Connection = TempMQContion.Connection(TmepUrl, TempUserName, TempUserPassWord, ref TempRes);
            return TempRes;
        }

        public void Test()
        {
            try
            {
               
                //从连接中获取一个通道
                IModel TempChannelModel = Connection.CreateModel();
                //声明交换机（分发:发布/订阅模式）
                TempChannelModel.ExchangeDeclare(EXCHANGE_NAME, "fanout");
                //发送消息
                String message = txtMessage.Text;
                //发送消息
                TempChannelModel.BasicPublish(EXCHANGE_NAME, "", null, System.Text.Encoding.UTF8.GetBytes(message));
                TempChannelModel.ConfirmSelect();
                //等待服务器应答确认
                if (!TempChannelModel.WaitForConfirms())
                {
                    MessageBox.Show("发送失败");
                }
                TempChannelModel.Close();
                Connection.Close();
            }
            catch (Exception e)
            {
                String TempR = e.Message;
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            String TempRes = ShowContion();
            if (TempRes != "OK")
            {
                MessageBox.Show(TempRes);
                return;
            }
            MessageBox.Show("连接成功");
        }
    }
}
