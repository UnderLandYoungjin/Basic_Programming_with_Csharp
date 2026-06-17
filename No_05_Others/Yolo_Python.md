```python
import cv2
from ultralytics import YOLO

# YOLO 세그멘테이션 모델 로드 (사전 학습된 모델 사용 권장)
model = YOLO("yolov8n-seg.pt")

# 비디오 파일 로드 및 객체 탐지
video_path = 'C:\\one\\bad\\xu1.mp4'
cap = cv2.VideoCapture(video_path)

# 비디오 파일 저장 설정
fourcc = cv2.VideoWriter_fourcc(*'mp4v')
out = cv2.VideoWriter('C:\\one\\bad\\output_xu1.mp4', fourcc, 30.0, (int(cap.get(3)), int(cap.get(4))))

while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break
    
    # 프레임을 YOLO 모델에 전달하여 객체 탐지 및 세그멘테이션 수행
    results = model(frame)
    
    # 결과를 프레임에 시각화
    annotated_frame = frame.copy()
    for result in results:
        # result.plot()을 사용하여 세그멘테이션 결과를 시각화
        annotated_frame = result.plot()  # 세그멘테이션 결과 시각화
    
    # 결과 프레임을 비디오 파일로 저장
    out.write(annotated_frame)

# 비디오 캡처 및 출력 종료
cap.release()
out.release()
cv2.destroyAllWindows()

print("객체 탐지 및 세그멘테이션 결과가 비디오 파일로 저장.")
```
