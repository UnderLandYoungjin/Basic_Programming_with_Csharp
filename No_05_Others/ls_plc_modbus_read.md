# PC → PLC Modbus RTU 통신 구축 정리

[네이버 카페 바로가기](https://cafe.naver.com/underfusion?iframe_url_utf8=%2FArticleRead.nhn%253Fclubid%3D30977017%2526articleid%3D821%2526menuid%3D50%2526boardtype%3DL)
 

<img width="1246" height="1166" alt="image" src="https://github.com/user-attachments/assets/25795f4c-75e0-4098-b37e-1de325babff3" />
<img width="274" height="197" alt="image" src="https://github.com/user-attachments/assets/ea445499-e5ad-4953-b88f-931fceb0666a" />

## 1. 시스템 구성

```
PC (Python Modbus RTU 마스터)
    │
    │ USB
    │
USB-RS485 컨버터
    │
    │ RS485 (A+/B-)
    │
PLC XBC-DR32H (Modbus RTU 서버/슬레이브)
```

| 구성 요소 | 설명 |
|-----------|------|
| PC | Python 3.x + pyserial, Modbus RTU 마스터 |
| 컨버터 | USB-RS485 변환기 (COM4) |
| PLC | LS XBC-DR32H, 채널2 RS485 |
| 통신 규격 | Modbus RTU, 9600bps, 8N1 |


---

## 2. PLC 설정 (XG5000)

### 2-1. 기본 파라미터 → Cnet 설정

| 항목 | 설정값 |
|------|--------|
| 채널2 통신 형태 | RS485 |
| 통신 속도 | 9600 bps |
| 국번 | 1 |
| 데이터 비트 | 8 |
| 패리티 | NONE |
| 정지 비트 | 1 |
| 동작 모드 | **모드버스 RTU 서버** |

### 2-2. Modbus 설정 (워드 영역)

| 항목 | 설정값 |
|------|--------|
| 워드 읽기 영역 시작주소 | **D00000** |
| 워드 쓰기 영역 시작주소 | **D00000** |

> Modbus 레지스터 주소 0x0000 = PLC D00000  
> Modbus 레지스터 주소 0x0001 = PLC D00001  
> (1:1 매핑)

### 2-3. P2P 블록

기존 P2P 블록은 **전부 삭제**  
(모드버스 RTU 서버 모드와 P2P 동시 사용 불가)

### 2-4. 다운로드

```
온라인 → 쓰기
→ 파라미터 ✓
→ 프로그램 ✓
→ 네트워크 구성 → [리셋]Cnet [base0, slot0] ✓
→ 확인 → PLC 리셋
```

> ⚠ **[리셋]Cnet 항목을 반드시 체크**해야 통신 파라미터가 PLC에 적용됨


---

## 3. 배선

```
USB-RS485 컨버터        PLC XBC-DR32H CH2
─────────────────       ─────────────────
A (+)          ────     A (+)
B (-)          ────     B (-)
GND            ────     SG
```

> ⚠ **A+/B- 극성이 바뀌면 통신 불가**  
> 반드시 A↔A, B↔B 로 연결할 것


---

## 4. Modbus RTU 프레임 구조

### Read (FC 0x03) - D 레지스터 읽기

**요청 (PC → PLC):**
```
01  03  00 00  00 01  84 0A
│   │   └──┬─┘ └──┬─┘ └──┬─┘
│   │    주소=0  개수=1   CRC
│  FC=Read
SlaveID=1
```

**응답 (PLC → PC):**
```
01  03  02  00 64  B8 44
│   │   │   └──┬─┘ └──┬─┘
│   │  바이트수 값=100  CRC
│  FC=Read
SlaveID=1
```

### Write (FC 0x06) - D 레지스터 쓰기

**요청 (PC → PLC):**
```
01  06  00 00  00 64  89 D4
│   │   └──┬─┘ └──┬─┘ └──┬─┘
│   │    주소=0  값=100   CRC
│  FC=Write
SlaveID=1
```

**응답 (PLC → PC):** 요청 프레임을 그대로 에코


---

## 5. Python 코드

### 설치

```bash
pip install pyserial
```

### 전체 코드 (rs485_plc_rw.py)

```python
import serial
import struct
import time

# ===== 설정 =====
PORT      = "COM4"
BAUDRATE  = 9600
SLAVE_ID  = 1       # PLC 국번
TIMEOUT   = 2.0     # 응답 대기 시간 (초)
TX_DELAY  = 0.1     # 송신 후 대기 시간 (초)
# =================

def calc_crc(data: bytes) -> bytes:
    crc = 0xFFFF
    for b in data:
        crc ^= b
        for _ in range(8):
            if crc & 0x0001:
                crc = (crc >> 1) ^ 0xA001
            else:
                crc >>= 1
    return struct.pack('<H', crc)

def check_crc(data: bytes) -> bool:
    if len(data) < 3:
        return False
    crc_recv = struct.unpack('<H', data[-2:])[0]
    crc_calc = struct.unpack('<H', calc_crc(data[:-2]))[0]
    return crc_recv == crc_calc

def send_frame(ser, frame):
    ser.reset_input_buffer()   # 송신 전 버퍼 비우기
    ser.write(frame)
    ser.flush()
    time.sleep(TX_DELAY)       # RS485 방향 전환 대기

def read_register(ser, slave, addr, count=1):
    """FC 0x03 - Read Holding Register"""
    pdu   = bytes([slave, 0x03]) + struct.pack('>HH', addr, count)
    frame = pdu + calc_crc(pdu)

    print(f"[송신] {' '.join(f'{b:02X}' for b in frame)}")
    send_frame(ser, frame)

    expected = 3 + 2 * count + 2
    resp = ser.read(expected)

    if len(resp) < expected:
        print(f"[수신] 응답 없음 (수신:{len(resp)}bytes / 기대:{expected}bytes)")
        if resp:
            print(f"       부분수신: {' '.join(f'{b:02X}' for b in resp)}")
        return None

    print(f"[수신] {' '.join(f'{b:02X}' for b in resp)}")

    if not check_crc(resp):
        print(f"       ⚠ CRC 불일치!")
        return None

    if resp[1] & 0x80:
        print(f"       ⚠ 에러 응답! 코드: 0x{resp[2]:02X}")
        return None

    values = []
    for i in range(count):
        val = (resp[3 + i*2] << 8) | resp[4 + i*2]
        values.append(val)
        print(f"       D{addr+i:05d} = {val} (0x{val:04X})")

    return values[0] if count == 1 else values

def write_register(ser, slave, addr, value):
    """FC 0x06 - Write Single Register"""
    pdu   = bytes([slave, 0x06]) + struct.pack('>HH', addr, value)
    frame = pdu + calc_crc(pdu)

    print(f"[송신] {' '.join(f'{b:02X}' for b in frame)}")
    send_frame(ser, frame)

    resp = ser.read(8)

    if len(resp) < 8:
        print(f"[수신] 응답 없음 (수신:{len(resp)}bytes / 기대:8bytes)")
        if resp:
            print(f"       부분수신: {' '.join(f'{b:02X}' for b in resp)}")
        return False

    print(f"[수신] {' '.join(f'{b:02X}' for b in resp)}")

    if not check_crc(resp):
        print(f"       ⚠ CRC 불일치!")
        return False

    if resp[1] & 0x80:
        print(f"       ⚠ 에러 응답! 코드: 0x{resp[2]:02X}")
        return False

    echo_addr  = (resp[2] << 8) | resp[3]
    echo_value = (resp[4] << 8) | resp[5]
    print(f"       ✓ 쓰기 성공! D{echo_addr:05d} = {echo_value} (0x{echo_value:04X})")
    return True

def print_help():
    print("  r        : D00000 읽기")
    print("  r [주소] : 해당 주소 읽기  (예: r 10 → D00010 읽기)")
    print("  숫자     : D00000 쓰기     (예: 100)")
    print("  숫자 주소: 해당 주소 쓰기  (예: 100 10 → D00010에 100 쓰기)")
    print("  h        : 도움말")
    print("  q        : 종료")

def main():
    print("=" * 55)
    print(f"  Modbus RTU 마스터  |  포트:{PORT}  국번:{SLAVE_ID}  {BAUDRATE}bps")
    print("=" * 55)
    print_help()
    print()

    try:
        ser = serial.Serial(
            port     = PORT,
            baudrate = BAUDRATE,
            bytesize = 8,
            parity   = serial.PARITY_NONE,
            stopbits = serial.STOPBITS_ONE,
            timeout  = TIMEOUT
        )
        ser.reset_input_buffer()
        print(f"[연결 성공] {PORT}\n")

    except serial.SerialException as e:
        print(f"[에러] 포트 열기 실패: {e}")
        return

    try:
        while True:
            try:
                cmd = input("명령> ").strip()
            except EOFError:
                break

            if not cmd:
                continue

            parts = cmd.split()

            if parts[0].lower() == 'q':
                break
            elif parts[0].lower() == 'h':
                print_help()
            elif parts[0].lower() == 'r':
                addr = 0
                if len(parts) >= 2:
                    try:
                        addr = int(parts[1])
                    except ValueError:
                        print("  주소는 숫자로 입력하세요  (예: r 10)")
                        print()
                        continue
                read_register(ser, SLAVE_ID, addr)
            elif parts[0].isdigit():
                try:
                    value = int(parts[0])
                    addr  = int(parts[1]) if len(parts) >= 2 else 0
                    if not (0 <= value <= 65535):
                        print("  값 범위 초과! 0 ~ 65535")
                        print()
                        continue
                    write_register(ser, SLAVE_ID, addr, value)
                except ValueError:
                    print("  올바른 형식: 숫자  또는  숫자 주소")
            else:
                print("  알 수 없는 명령. h 입력시 도움말")

            print()

    except KeyboardInterrupt:
        print("\n[종료]")
    finally:
        ser.close()
        print("포트 닫힘")

if __name__ == "__main__":
    main()
```


---

## 6. 사용법

```
명령> r          → D00000 읽기
명령> r 10       → D00010 읽기
명령> 100        → D00000 = 100 쓰기
명령> 200 10     → D00010 = 200 쓰기
명령> h          → 도움말
명령> q          → 종료
```

### 출력 예시

```
명령> r
[송신] 01 03 00 00 00 01 84 0A
[수신] 01 03 02 00 64 B8 44
       D00000 = 100 (0x0064)

명령> 200
[송신] 01 06 00 00 00 C8 88 5C
[수신] 01 06 00 00 00 C8 88 5C
       ✓ 쓰기 성공! D00000 = 200 (0x00C8)
```


---

## 7. 트러블슈팅

| 증상 | 원인 | 해결 |
|------|------|------|
| 응답 없음 | A+/B- 배선 반대 | A↔A, B↔B 재연결 |
| 응답 없음 | 파라미터 다운로드 안 됨 | [리셋]Cnet 체크 후 재다운로드 |
| 응답 없음 | P2P 블록 충돌 | P2P 블록 전부 삭제 |
| CRC 불일치 | 통신 속도 불일치 | PLC/PC 양쪽 9600 확인 |
| Timeout | 국번 불일치 | PLC 국번과 SLAVE_ID 일치 확인 |


---

## 8. D 레지스터 주소 매핑

| Modbus 주소 | PLC 레지스터 |
|-------------|-------------|
| 0x0000 | D00000 |
| 0x0001 | D00001 |
| 0x000A | D00010 |
| 0x0064 | D00100 |

> 공식: `Modbus 주소 = D 레지스터 번호`  
> 예) D00010 → 0x000A


---

## 9. 다음 단계 (인버터 연동)

인버터 추가 시 래더 프로그램에서:

```
PC → D00000 값 쓰기
  → 래더: MOV D00000 → 인버터 제어용 D 레지스터
  → P2P WRITE: 인버터 0x40005 (목표주파수) 에 전달
  → 인버터 동작
```
