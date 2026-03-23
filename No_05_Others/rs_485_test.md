# RS485 Modbus RTU 데이터 수신 모니터

## 1. 목적

LS XGB PLC에서 RS485 통신으로 전송하는 데이터를 PC에서 수신하여 정상적으로 데이터가 오고 있는지 확인하기 위한 모니터링 도구이다.

PLC가 인버터(LS G100)에 Modbus RTU로 보내는 데이터를 USB-RS485 컨버터를 통해 PC에서 가로채어 확인하는 용도로, 본격적인 제어가 아닌 **데이터 수신 확인용**이다.


## 2. 시스템 구성

```
PLC (XGB)  ──RS485──  USB-RS485 컨버터  ──USB──  PC (Python)
```

| 구성 요소 | 설명 |
|-----------|------|
| PLC | LS XGB, P2P 통신으로 1초 주기 데이터 전송 |
| 통신 규격 | RS485, Modbus RTU |
| 컨버터 | USB-RS485 (COM4로 인식) |
| PC 소프트웨어 | Python 3.12 + pyserial |


## 3. 통신 설정

| 항목 | 설정값 |
|------|--------|
| COM 포트 | COM4 |
| Baud Rate | 9600 bps |
| Data Bit | 8 bit |
| Parity | None |
| Stop Bit | 1 |
| 프레임 크기 | 9 bytes |


## 4. PLC 통신 주소 (LS G100 기준)

LS G100 인버터 매뉴얼(7.5 통신 호환 공통 영역 파라미터) 기준 주소 맵:

| 통신 번지 | 파라미터 | 스케일 | 단위 | R/W |
|-----------|----------|--------|------|-----|
| 0h0004 | Reserved | - | - | R/W |
| 0h0005 | 목표 주파수 | 0.01 | Hz | R/W |
| 0h0006 | 운전 지령(옵션) | - | - | R/W |

### 운전 지령(0h0006) 비트 할당

| 비트 | 기능 |
|------|------|
| B0 | 정지(S) |
| B1 | 정방향 운전(F) |
| B2 | 역방향 운전(R) |
| B3 | Trip Reset |
| B4 | 프리 런 정지 |

### PLC D 레지스터 → 운전 지령 값 예시

| D 레지스터 값 | 의미 |
|--------------|------|
| 1 (0x0001) | 정지 |
| 2 (0x0002) | 정방향 운전 |
| 4 (0x0004) | 역방향 운전 |


## 5. 작업 과정

### 5-1. 환경 준비

```bash
pip install pyserial
```

장치관리자에서 USB-RS485 컨버터의 COM 포트 번호를 확인한다.

### 5-2. 코드 작성 (rs485_monitor_v3.py)

핵심 동작 흐름:

1. `serial.Serial()`로 COM4 포트를 열고 통신 파라미터를 설정한다.
2. `ser.read(FRAME_SIZE)`로 정확히 9바이트(프레임 크기)만큼 읽는다.
3. 수신된 데이터를 HEX로 출력하고, Modbus RTU 프레임으로 파싱하여 슬레이브 ID, 기능코드, 레지스터 주소, 값을 표시한다.
4. 이전 프레임과 비교하여 데이터 변화가 있으면 알림을 출력한다.
5. Ctrl+C로 종료 시 총 수신 프레임 수를 표시한다.

### 5-3. 실행

```bash
python rs485_monitor_v3.py
```

### 5-4. 출력 예시

```
[연결 성공] COM4 / 9600bps / 8N1
수신 대기중... (Ctrl+C로 종료)
----------------------------------------------------------------------
#0001 (9bytes) HEX: 00 F3 FF F5 FF F5 4F CD FF
      슬레이브: 0, 기능코드: 0xF3, 레지스터: 0xFFF5, 값: 65525 (0xFFF5), CRC: 0xFFCD

#0002 (9bytes) HEX: 00 F3 FF F5 FF F5 4F CD FF
      슬레이브: 0, 기능코드: 0xF3, 레지스터: 0xFFF5, 값: 65525 (0xFFF5), CRC: 0xFFCD
```


## 6. 트러블슈팅

### 프레임 쪼개짐 현상

초기 버전에서 `ser.in_waiting`으로 버퍼에 있는 만큼만 읽었더니, 9바이트 프레임이 1바이트 + 8바이트로 쪼개져서 파싱이 깨지는 현상이 발생했다. PLC에서 1초 주기로 데이터를 전송하도록 수정한 후, `ser.read(FRAME_SIZE)`로 정확히 프레임 크기만큼 읽는 방식으로 해결했다.

### 데이터가 안 들어올 때 확인 순서

1. USB-RS485 컨버터 배선 (A+/B- 연결 확인)
2. COM 포트 번호 (장치관리자에서 확인)
3. 통신 속도 (PLC와 동일한지 확인: 9600/19200)
4. PLC 통신 설정 (P2P 모드, 슬레이브 주소 등)


## 7. 참고 사항

- 본 코드는 데이터 수신 확인 전용이며, 인버터 제어 기능은 포함하지 않는다.
- LS 인버터의 Modbus 주소는 PLC 종류에 따라 +1 오프셋이 필요할 수 있다.
- PLC에서 워드(D 레지스터)로 데이터를 전송하며, 비트 단위 제어가 필요한 운전 지령(0h0006)도 D 레지스터에 비트 조합 값을 넣어 워드 단위로 전송한다.



```python
"""
RS485 데이터 수신 모니터 (프레임 단위 수집 버전)
- PLC(XGB)에서 1초마다 보내는 데이터를 PC에서 확인
- USB-RS485 컨버터 사용
- pip install pyserial
"""

import serial

# ============ 설정 ============
PORT = "COM4"
BAUDRATE = 9600
PARITY = "N"
STOPBITS = 1
BYTESIZE = 8
FRAME_SIZE = 9          # 예상 프레임 크기 (바이트)
# ==============================

def read_frame(ser):
    """프레임 크기만큼 읽어서 반환"""
    data = ser.read(FRAME_SIZE)
    if not data:
        return None
    return bytes(data)


def parse_and_print(raw, count):
    """수신된 프레임을 파싱하여 출력"""
    hex_str = " ".join(f"{b:02X}" for b in raw)
    print(f"#{count:04d} ({len(raw)}bytes) HEX: {hex_str}")

    if len(raw) >= 8:
        slave_id = raw[0]
        func_code = raw[1]
        reg_addr = (raw[2] << 8) | raw[3]
        reg_value = (raw[4] << 8) | raw[5]
        crc_recv = (raw[-1] << 8) | raw[-2]
        print(f"      슬레이브: {slave_id}, "
              f"기능코드: 0x{func_code:02X}, "
              f"레지스터: 0x{reg_addr:04X}, "
              f"값: {reg_value} (0x{reg_value:04X}), "
              f"CRC: 0x{crc_recv:04X}")

    return raw


def main():
    try:
        ser = serial.Serial(
            port=PORT,
            baudrate=BAUDRATE,
            parity=PARITY,
            stopbits=STOPBITS,
            bytesize=BYTESIZE,
            timeout=5
        )
        print(f"[연결 성공] {PORT} / {BAUDRATE}bps / {BYTESIZE}{PARITY}{STOPBITS}")
        print(f"수신 대기중... (Ctrl+C로 종료)\n")
        print("-" * 70)

        count = 0
        prev_frame = None

        while True:
            frame = read_frame(ser)

            if frame is None:
                continue

            count += 1

            if prev_frame is not None and frame != prev_frame:
                print("  *** 데이터 변화 감지! ***")

            prev_frame = parse_and_print(frame, count)

            if len(frame) != FRAME_SIZE:
                print(f"  ⚠ 예상 {FRAME_SIZE}bytes인데 {len(frame)}bytes 수신됨")

            print()

    except serial.SerialException as e:
        print(f"[에러] 시리얼 포트 오류: {e}")
        print("→ COM 포트 번호 확인 / 다른 프로그램에서 사용중인지 확인")
    except KeyboardInterrupt:
        print(f"\n\n총 {count}개 프레임 수신. 종료합니다.")
    finally:
        if 'ser' in locals() and ser.is_open:
            ser.close()
            print("포트 닫힘.")

if __name__ == "__main__":
    main()
```
