using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ
{
    public class RabbitMQHelper
    {
        ConnectionFactory connectionFactory;
        IConnection connection;
        IModel channel;
        string exchangeName;

        public RabbitMQHelper(string changeName= "fanout_mq") {
            this.exchangeName = changeName;
            //创建连接工厂
            connectionFactory = new ConnectionFactory
            {
                HostName = "192.168.8.203",
                UserName = "superrd",
                Password = "superrd"
            };
            //创建连接
            connection = connectionFactory.CreateConnection();
            //创建通道
            channel = connection.CreateModel();
            //声明交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
            
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queName"></param>
        /// <param name="msg"></param>
        public void SendMsg<T>(string queName,T msg)where T:class
        {
            //声明一个队列
            channel.QueueDeclare(queName, true, false, false, null);
            //绑定队列，交换机，路由键
            channel.QueueBind(queName, exchangeName, queName);

            var basicProperties = channel.CreateBasicProperties();
            //1：非持久化 2：可持久化
            basicProperties.DeliveryMode = 2;
            var payload = Encoding.UTF8.GetBytes("我发出的消息");
            var address = new PublicationAddress(ExchangeType.Direct, exchangeName, queName);
            channel.BasicPublish(address, basicProperties, payload);
        }

        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="queName"></param>
        /// <param name="received"></param>
        public void Receive(string queName,Action<string> received)
        {
            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                string message = Encoding.UTF8.GetString(ea.Body);
                received(message);
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queName, false, consumer);
            
        }
    }
}
