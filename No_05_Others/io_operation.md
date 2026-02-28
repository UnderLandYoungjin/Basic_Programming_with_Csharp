,,,

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("키보드를 입력 하세요, 프로그램을 종료하고 싶으시면 exit를 입력하세요");
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                    break;

                Console.WriteLine(input);
            }
        }
    }
}


,,,
