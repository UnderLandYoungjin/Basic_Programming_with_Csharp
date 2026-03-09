```Csharp
namespace ConsoleApp9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string name = "허영진";
            short wasborn = 1984;
            double height = 169.5;
            double weight = 66.8;

            double height_m = height / 100;
            double bmi = weight / (height_m * height_m);
            double bmi_point2 = Math.Round(bmi, 2);         



            Console.WriteLine("===체질량계산기===");
            Console.WriteLine("이름 = " + name);
            Console.WriteLine("키 = " + height + "Cm");
            Console.WriteLine("몸무게 = " +weight +"Kg");
            Console.WriteLine("체질량 = " + bmi_point2);


            if (bmi_point2 < 18.5)
            {
                Console.WriteLine("저체중입니다.");
            }
            else if (bmi_point2 >= 18.5 && bmi_point2 < 23)
            {
                Console.WriteLine("정상체중입니다.");
            }
            else if (bmi_point2 >= 23 && bmi_point2 < 25)
            {
                Console.WriteLine("과체중입니다.");
            }
            else
            {
                Console.WriteLine("비만입니다.");
            }

        }
    }
}
```

```Csharp
namespace ConsoleApp9
{
    internal class Program
    {
        static double CalcBmi(double weight, double height)
        {
            double heightM = height / 100;
            return Math.Round(weight / (heightM * heightM), 2);
        }

        static string GetBmiStatus(double bmi)
        {
            if (bmi < 18.5) return "저체중";
            else if (bmi < 23) return "정상체중";
            else if (bmi < 25) return "과체중";
            else return "비만";
        }

        static void PrintResult(string name, int age, double height, double weight, double bmi)
        {
            Console.WriteLine("===체질량계산기===");
            Console.WriteLine("이름     : " + name);
            Console.WriteLine("나이     : " + age + "세");
            Console.WriteLine("키       : " + height + "cm");
            Console.WriteLine("몸무게   : " + weight + "kg");
            Console.WriteLine("체질량   : " + bmi);
            Console.WriteLine("판정     : " + GetBmiStatus(bmi));
        }

        static void Main(string[] args)
        {
            string name   = "허영진";
            short wasborn = 1984;
            int age       = 2025 - wasborn;
            double height = 169.5;
            double weight = 66.8;

            double bmi = CalcBmi(weight, height);

            PrintResult(name, age, height, weight, bmi);
        }
    }
}
```
