## 소개
Jupporter는 윈도우에서 프로세스가 종료되면 다시 실행하는 프로그램입니다.

## 사용법

### arguments
* /autostart - 실행중인 프로그램을 종료 및 윈도우 숨김처리

### option

| option          | description                                         |
| --------------- | --------------------------------------------------- |
| targetPath      | 실행할 프로그램 경로 (C:\Windows\notepad.exe)            |
| runOption       | 실행 프로그램 아규먼트 (C:\test.txt)                      |
| refreshCycle    | 프로그램을 몇 초 마다 감지할것인지 (ms)                      |
| autoRestart     | 특정 시간에 자동 재시작 사용 여부 (Y or n)                  |
| autoRestartTime | 특정 시간에 자동 재시작 (hh,mm,ss)                        |

* 특정 시간에 자동 재시작은 프로그램 실행 시 남음 시간을 계산하여 메모리에 갖고 있기때문에 시스템 시간을 바꿔도 작동이 안 될 수 있습니다.
