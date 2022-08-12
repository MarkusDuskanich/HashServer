using System;
using System.Collections.Generic;

namespace Models
{
    public partial class Message
    {
        public Guid Id { get; set; }
        public Guid Hashid { get; set; }
        public string Value { get; set; } = null!;
        public virtual Hash Hash { get; set; } = null!;
    }
}
