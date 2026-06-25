# Smart Rescue Mat

재난 현장에서 요구조자를 자율 추적하고 구조 매트를 전개하는 로봇 시스템

[![YouTube](https://img.shields.io/badge/YouTube-FF0000?logo=youtube&logoColor=white)](https://youtu.be/KUjfwRFFdnw?si=0Qv6k90GMNfeaLJs)

---

## 구성

### Unity — 시스템 시나리오 시뮬레이션
- 구조 매트 로봇의 다리 전개 및 매트 활성화 시뮬레이션
- 크레인 차량, 드론 시나리오 포함
- 
### ROS2 — 객체 추적 및 로봇 제어 (기능 구현 프로토타입)
[![README](https://img.shields.io/badge/README-Object__Follow-blue)](Object_Follow/README.md)
- Intel RealSense D435i + YOLOv8n (TensorRT) 으로 대상 객체 탐지
- Depth 카메라로 3D 위치 추정 후 PID 제어로 TurtleBot3 Burger 자율 추적
- 연산은 노트북에서 수행, Twist 명령만 네트워크로 전송