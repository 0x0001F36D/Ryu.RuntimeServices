// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Ryuko.ConsoleTest
{ 
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Drawing;
    using System.IO;
    using System.Resources;
    using System.Resources.Tools;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CSharp;
    using Text;

    internal class Program
    {
        public static void Main()
        {
            var raw = new Source("0123456789")
                [0, 3].Insert(2, "#######").PushBack()
                [7, 5].Insert(0, "@@@@@").PushBack()
                [0].Replace("1", "2", ("4", "6")).PushBack();


            Console.WriteLine(raw);

            raw[0].Replace("2", "4", ("4", "8")).PushBack();

            Console.WriteLine(raw);

            var s = (raw[10] + "4").PushBack();
            Console.WriteLine(s);

            Console.ReadKey();
            return;


            const RegexOptions OPTIONS = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnoreCase;
            /*

            Regex.CompileToAssembly(new RegexCompilationInfo[] 
            {
                new RegexCompilationInfo("(?<Status>t|true|on|enable)", )
            },)
            */

            var mc = Regex.Matches("s: true\nv:on\ne:Disable", BuildSwitch(), OPTIONS);
            //Regex.Matches("s: 我你\r\n他:55", @"(?<Name>[\w]+)\s*:\s*(?<Status>[^\r\n]+)", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            foreach (Match m in mc)
            {
                var name = m.Groups["Name"].Value;
                var status = m.Groups["Status"].Value;

                Console.WriteLine($"{name}:> {status}");
            }
            Console.ReadKey();
            return;

        }

        private static string BuildSwitch()
        {
            return string.Format(NAME, string.Format(STATUS, VALUES));
        }

        private const string VALUES =  @"(true|on|enable|y|yes)|(false|off|disable|n|no)";

        private const string STATUS = @"\s*(?<Status>{0})";

        private const string NAME = @"(?<Name>[\w]+)\s*:{0}";



        public interface INode<T>: INode
        {
            new T Value { get; set; }
        }
        public interface INode
        {
            string Name { get; set; }
            object Value { get; set; }
        }


        public class MatchSegment
        {



        }




        /*

        public interface INodeSwitch: INode<bool>
        {
        }
        
        public class FyrSwitch : INodeSwitch
        {
            public string Name { get; set; }
            public bool Value { get; set; }
            object INode.Value
            {
                get => this.Value;
                set => this.Value = (bool)value;
            }
        }

        public interface IParser<TNodeType> where TNodeType : INode
        {
            void Prepare(Raw raw);

            INodeSwitch[] Decode();

            void Encode(INodeSwitch[] nodeSwitches);
        }

        public class NodeSwitchParser : IParser<INodeSwitch>
        {
            public void Prepare(Raw raw)
            {

            }

            public INodeSwitch[] Decode()
            {

            }

            public void Encode(INodeSwitch[] nodeSwitches)
            {

            }
        }*/
    }
}