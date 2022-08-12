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
