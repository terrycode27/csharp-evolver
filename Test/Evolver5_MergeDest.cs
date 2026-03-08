using System;
using System.Collections.Generic;
using System.Text;

namespace Abc
{
    public class InsideABC
    {

    }
}
public partial class b
{
    public void b_outside()
    {

    }
}
public partial class b
{
    public void b_outside2()
    {

    }
}
namespace TestMerge
{
    public partial class a
    {
        public void MethodA()
        {
            Console.WriteLine("destination-should be replaced");
        }

        public int Val;
        public int P { get; set; }
    }
    public partial class a
    {
        public int Val2;
        public int P2 { get; set; }
    }
    public partial class c
    {
        public void c_method()
        {

        }
    }

}
public namespace TestMerge
{
    public partial class a
    {
        public void MethodA1()
        {
            Console.WriteLine("destination-should be replaced");
        }

        public int Val1;
        public int P1 { get; set; }
    }
    public partial class b
    {
        public void b_inside_ns()
        {

        }
    }
}
//