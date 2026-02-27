// ===============================
// 파일명: Form1.cs
// 목적: 버튼을 클릭하면 Label의 글자가 바뀌는 간단한 WinForms 예제
// ===============================

using System;                          // 기본 C# 기능 사용
using System.Collections.Generic;      // 제네릭 컬렉션 사용 (현재는 사용 안하지만 기본 포함)
using System.ComponentModel;           // 컴포넌트 관련 기능
using System.Data;                     // 데이터 관련 기능
using System.Drawing;                  // 색상, 크기, 폰트 등 그래픽 관련 기능
using System.Linq;                     // LINQ 기능
using System.Text;                     // 문자열 처리 기능
using System.Threading.Tasks;          // 비동기 작업 기능
using System.Windows.Forms;            // WinForms 사용을 위한 필수 네임스페이스

namespace WindowsFormsApp1              // 프로젝트 이름(공간 이름)
{
    // Form1은 Form을 상속받음 → 즉, 화면 창을 의미
    public partial class Form1 : Form
    {
        // ========================================
        // 변수 영역
        // ========================================

        // 🔹 버튼이 현재 눌린 상태인지 저장하는 변수
        // false → 아직 안 눌림
        // true  → 눌린 상태
        private bool isClicked = false;

        // 🔹 처음 Label에 표시할 기본 문구 저장
        private string originalText = "버튼을 클릭하세요";

        // ========================================
        // 생성자 (Form이 처음 만들어질 때 실행)
        // ========================================
        public Form1()
        {
            InitializeComponent();
            // ↑ 디자이너에서 만든 버튼, 라벨 등을 초기화하는 코드
            // 이 코드가 없으면 화면에 컨트롤이 안 나타남

            // 🔹 Label에 처음 보여줄 문구 설정
            label1.Text = originalText;
        }

        // ========================================
        // 버튼 클릭 이벤트
        // 버튼을 누를 때마다 실행되는 함수
        // ========================================
        private void button1_Click(object sender, EventArgs e)
        {
            // 현재 버튼이 눌리지 않은 상태라면
            if (isClicked == false)
            {
                // Label의 글자를 변경
                label1.Text = "버튼이 클릭되었습니다.";

                // 상태를 true로 변경 (이제 눌린 상태)
                isClicked = true;
            }
            else
            {
                // 다시 원래 글자로 복구
                label1.Text = originalText;

                // 상태를 false로 변경 (처음 상태로 되돌림)
                isClicked = false;
            }
        }

        // ========================================
        // Label 클릭 이벤트 (현재 기능 없음)
        // ========================================
        private void label1_Click(object sender, EventArgs e)
        {
            // 필요하면 여기 기능 추가 가능
        }

        // ========================================
        // 중복 생성된 Label 이벤트 (사용하지 않음)
        // ========================================
        private void label1_Click_1(object sender, EventArgs e)
        {
            // 실제로 사용하지 않는 이벤트
            // 디자이너에서 실수로 두 번 생성되었을 가능성 있음
            // 하나는 삭제해도 됨
        }
    }
}
