
namespace Ryuko.Dev
{
    using Ryuko.Diagnostics;
    using System;
    using System.Diagnostics;
    using System.IO;

    class Test
    {
        static void Main(string[] args)
        {
            if(RunningMode.IsDebug)
            {
                Console.WriteLine("Debugging...");
            }
            //Perplexed.Locate(99);


            var t = new Test();
          //  t.Item = null;
          //  t.get_Item(2);
            
            t[null] = null;
            _ = t[null];

            t = ((((~t + t - t * t / t % t ^ t | t++ & --t) >> 1) << 1) > t) < t;
            
            if (t)
            { 
            }
        }


        static Test()
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
        }
        public Test()
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
        }

        
        public Test this[object index]
        {
            get
            {
                Console.WriteLine(Perplexed.Locate().Attributes);
                return this;
            }
            set
            {
                Console.WriteLine(Perplexed.Locate().Attributes);
            }
        }

        public static implicit operator int (Test _)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);

            return 0;
        }

        public static Test operator +(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }

        public static Test operator -(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }

        public static Test operator *(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static Test operator /(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static Test operator %(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static Test operator &(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static Test operator ^(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static Test operator |(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }


        public static bool operator ==(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return true;
        }
        public static bool operator !=(Test a, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return false;
        }



        public static Test operator <<(Test s, int x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return s;
        }
        public static Test operator >> (Test s, int x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return s;
        }


        public static Test operator ~(Test a)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static Test  operator ++(Test a)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static Test operator --(Test a)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return a;
        }
        public static bool operator true(Test a)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return true;
        }
        public static bool operator false(Test a)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return false;
        }


        public static Test operator >(Test t, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return t;
        }
        public static Test operator <(Test t, Test x)
        {
            Console.WriteLine(Perplexed.Locate().Attributes);
            return t;
        }
         
    }
}
