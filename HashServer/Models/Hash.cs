﻿using System;
using System.Collections.Generic;

namespace Models
{
    public partial class Hash
    {
        public Hash()
        {
            Messages = new HashSet<Message>();
        }

        public Guid Id { get; set; }
        public string Value { get; set; } = null!;

        public virtual ICollection<Message> Messages { get; set; }
    }
}
