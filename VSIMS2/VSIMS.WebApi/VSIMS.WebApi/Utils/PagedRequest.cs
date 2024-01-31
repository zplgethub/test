﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIMS.WebApi.Utils
{
    public class PagedRequest<T>
    {
        public int count { get; set; }
        public List<T> items { get; set; }
    }
}
