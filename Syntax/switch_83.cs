// 파일 경로: ConsoleApp3/Program.cs
// 이 파일은 콘솔 프로그램의 시작점(Main 함수)을 포함하는 메인 실행 파일입니다.

namespace ConsoleApp3   // ConsoleApp3라는 이름의 공간(프로젝트 범위)을 정의
{
    internal class Program   // Program이라는 내부 클래스 선언 (프로그램의 실행 단위)
    {
        static void Main(string[] args)   // 프로그램이 시작되면 가장 먼저 실행되는 메인 함수
        {
            int a;   // 정수형 변수 a 선언 (반복문에서 사용할 숫자 변수)

            // a를 5부터 8까지 1씩 증가시키며 반복 실행
            for (a = 5; a <= 8; a++)
            {
                // a ÷ 3의 몫을 출력
                // ÷ 기호는 Windows에서 Alt + 0247 입력 후 Alt 키를 떼면 입력 가능
                // a / 3 은 정수 나눗셈이므로 소수점은 버려짐 (예: 5 / 3 = 1)
                Console.Write(a + "÷3=" + a / 3);

                // a를 3으로 나눈 나머지를 구함
                // % 연산자는 나머지를 구하는 연산자
                switch (a % 3)
                {
                    case 1:   // 나머지가 1일 경우
                        Console.WriteLine(":나머지는 1입니다.");   // 해당 문장 출력 후 줄바꿈
                        break;   // switch문 종료

                    case 2:   // 나머지가 2일 경우
                        Console.WriteLine(":나머지는 2입니다.");   // 해당 문장 출력 후 줄바꿈
                        break;   // switch문 종료

                    default:  // 위 조건에 해당하지 않는 경우 (즉, 나머지가 0인 경우)
                        Console.WriteLine(":나머지는 0입니다.");   // 해당 문장 출력 후 줄바꿈
                        break;   // switch문 종료
                }
            }
        }
    }
}
