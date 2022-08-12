using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class MessageDTO
    {
        public string Message { get; set; }

        public MessageDTO(Message message)
        {
            Message = message.Value;
        }
    }
}
