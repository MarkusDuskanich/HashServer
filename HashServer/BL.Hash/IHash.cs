﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Hash
{
    public interface IHash
    {
        string ComputeHash(string value);
    }
}
