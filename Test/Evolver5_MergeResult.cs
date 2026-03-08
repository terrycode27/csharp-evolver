using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public partial class b
{
    public void b_outside() { }

    public void b_outside2() { }
}

public class q { }

namespace Abc
{
    public class InsideABC { }
}

namespace C
{
    public partial class B { }
}

namespace D { }

namespace TestMerge
{
    public partial class a
    {
        public void MethodA()
        {
            Console.WriteLine("sourceA");
        }

        public void MethodA1()
        {
            Console.WriteLine("sourceA1");
        }

        public int Val;
        public int Val1;
        public int Val2;
        public int P { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
    }

    public partial class b
    {
        public void b_inside_ns() { }
    }

    public partial class c
    {
        public void c_method() { }
    }
}
