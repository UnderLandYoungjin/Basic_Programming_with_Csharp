<img width="2171" height="1132" alt="image" src="https://github.com/user-attachments/assets/47780be9-b1f8-4889-8ce0-fbc2804ed195" />



```
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
```
