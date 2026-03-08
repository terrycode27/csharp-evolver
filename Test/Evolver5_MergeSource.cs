using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
        public int Val1;
        public int P1 { get; set; }
    }
}
namespace C
{
    public partial class B
    {

    }
}
namespace D
{

}
public class q
{

}