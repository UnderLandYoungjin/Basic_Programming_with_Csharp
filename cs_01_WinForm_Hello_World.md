<img width="1365" height="906" alt="image" src="https://github.com/user-attachments/assets/3e36b6af-72e7-4220-bb15-f2fb498bdb5c" />
<img width="1365" height="908" alt="image" src="https://github.com/user-attachments/assets/5d0b2a64-8b05-438b-bc42-dc9f65558e48" />
GUI를 사용하기 위해 보기-도구상자 클릭
<img width="1360" height="999" alt="image" src="https://github.com/user-attachments/assets/f93748ad-fb0d-4fd2-8487-6f87b4d39d36" />
<img width="1520" height="704" alt="image" src="https://github.com/user-attachments/assets/946cbefb-2ada-4e93-853b-abf354ad0f92" />
라벨과 버튼을 생성하고 버튼을 클릭
<img width="1600" height="845" alt="image" src="https://github.com/user-attachments/assets/9c0bf7ca-b4a4-45e3-a707-308ab59576da" />


아래는 응용한것으, 버튼을 누를때 마다 바뀔수 있도록 수정


// 📂 경로: WindowsFormsApp3/Form1.cs
// 📌 목적: 버튼 클릭 시 Label 텍스트를 토글(변경/복구)하는 기능 구현

using System;                      // 기본 시스템 기능 사용
using System.Collections.Generic;  // 컬렉션 관련 기능
using System.ComponentModel;       // 컴포넌트 모델 관련
using System.Data;                 // 데이터 관련
using System.Drawing;              // 그래픽 관련
using System.Linq;                 // LINQ 기능
using System.Text;                 // 텍스트 관련
using System.Threading.Tasks;      // 비동기 작업 관련
using System.Windows.Forms;        // Windows Form 관련 기능

namespace WindowsFormsApp3         // 네임스페이스 정의
{
    public partial class Form1 : Form   // Form1 클래스 정의 (Form 상속)
    {
        // 🔹 현재 버튼 상태를 저장하는 변수
        // false = 기본 상태
        // true  = 버튼이 눌린 상태
        private bool isClicked = false;

        // 🔹 기본 텍스트를 저장하는 변수
        private string originalText = "안녕하세요";  // 초기 Label 텍스트 (원하는 값으로 수정 가능)

        public Form1()
        {
            InitializeComponent();      // 폼 초기화

            // 🔹 Label의 초기 텍스트 설정
            label1.Text = originalText;
        }

        // 🔹 버튼 클릭 이벤트
        private void button1_Click(object sender, EventArgs e)
        {
            // 현재 상태가 눌리지 않은 상태라면
            if (isClicked == false)
            {
                label1.Text = "버튼이 클릭되었습니다.";  // 텍스트 변경
                isClicked = true;                      // 상태 변경
            }
            else
            {
                label1.Text = originalText;            // 원래 텍스트로 복구
                isClicked = false;                     // 상태 원래대로 변경
            }
        }

        // 🔹 Label 클릭 이벤트 (현재 기능 없음)
        private void label1_Click(object sender, EventArgs e)
        {
            // 필요 시 기능 추가 가능
        }
    }
}


<img width="812" height="486" alt="image" src="https://github.com/user-attachments/assets/ab3c2fae-ed25-4a1e-a70e-0a97d4b16288" />
