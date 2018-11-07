using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ
{
    public class MQHelperFactory
    {
        public static RabbitMQHelper Default() =>
            new RabbitMQHelper("exchange_fanout");
    }
}
