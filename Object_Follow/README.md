## Object Follow

Intel RealSense D435i와 YOLOv8을 이용하여 탁구공을 인식하고, 터틀봇이 XY 정렬을 하는 프로젝트

[![YouTube](https://img.shields.io/badge/YouTube-FF0000?logo=youtube&logoColor=white)](https://youtube.com/shorts/XsSxA7pj_08?si=eO9btbUxAGqNdQLp)

---
## 💻 개발 환경 (Environment)
* **OS:** Ubuntu 22.04
* **ROS Version:** ROS2 Humble
* **Hardware:**
    * Intel RealSense D435i
    * TurtleBot3 Burger (Raspberry Pi 4 2GB)
    * Laptop (RTX4060)

---
## ✨ 주요 기능 (Features & Logic)

1.  **객체 탐지 (Object Detection):**
    * `YOLOv8n` TensorRT 모델(`yolov8n.engine`)을 사용하여 카메라 영상에서 **주황색(class_id 49)** 객체를 탐지

2.  **3D 위치 추정 (3D Position Estimation):**
    * RealSense 카메라의 Depth 정보를 이용해 탐지된 객체(탁구공)까지의 3차원 거리를 계산

3.  **로봇 제어 (Robot Control):**
    * 계산된 객체의 위치를 바탕으로 **PD 제어기**를 통해 로봇의 목표 선속도 및 각속도를 결정
    * 계산된 속도 값은 `geometry_msgs/msg/Twist` 메시지 형태로 터틀봇의 `/cmd_vel` 토픽으로 발행(publish)

---
## 📝 시스템 구성 및 제약사항 (System Configuration & Constraints)

* **분리된 처리 구조:** Raspberry Pi의 성능 이슈로 인해, RealSense 카메라는 **노트북에 직접 연결**하여 모든 비전 처리 및 제어 연산을 수행. 계산된 최종 속도 명령(`Twist`)만 네트워크를 통해 터틀봇으로 전송.

* **물리적 분리:** 현재 리얼센스 카메라를 터틀봇 상단에 고정할 마운트가 없어, **카메라와 로봇은 물리적으로 분리된 상태**로 프로젝트가 진행. 코드 내의 `static_transform_publisher`는 카메라가 로봇 위에 탑재된 상황을 가정하여 설정.

---
## ▶️ 실행 방법 (How to Run)

1.  **터틀봇 실행**
    ```bash
    # TurtleBot에서 실행
    ros2 launch turtlebot3_bringup robot.launch.py
    ```
2.  **메인 컨트롤러 실행**
    ```bash
    # 노트북에서 실행
    ros2 launch yolo_project_package object_follow.launch.py
    ```
