﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class JumpListGroup<T> : List<object>
{
    public object Key { get; set; }

    public new IEnumerator<object> GetEnumerator()
    {
        return (System.Collections.Generic.IEnumerator<object>)base.GetEnumerator();
    }
}
