# Tài liệu Yêu cầu Kỹ thuật (Requirements Document)

## 1. Giới thiệu (Introduction)
Dự án phát triển giải pháp đồng bộ và tích lũy thời gian thực tế của các công đoạn tại màn hình Tổng quan (Overview). Hiện tại, cơ chế cộng dồn thời gian chạy thực tế của các công đoạn khi mẻ sản xuất bị Tạm dừng/Chạy tiếp (Pause/Resume) đang được thực hiện ở Client-side JavaScript. Cơ chế này bị sai lệch số liệu và trễ do trình duyệt bị bóp hiệu năng (throttling) hoặc bị mất sự kiện reset của PLC. Giải pháp này chuyển đổi cơ chế tính toán tích lũy thời gian thực về phía C# Backend thông qua một dịch vụ chạy ngầm (Background Service) hoạt động 24/7 trên Web Server.
Đồng thời, giải pháp này tối ưu hóa việc nhận diện trạng thái Tạm dừng bằng cách kết hợp thanh ghi trạng thái dừng (`STOP` tag) hoặc trường dữ liệu trạng thái dừng của mẻ con (`is_paused` trong bảng `runs`) để bắt chính xác thời điểm chuyển giao trạng thái, loại bỏ hoàn toàn sai số do mất gói tin reset.

---

## 2. Các Yêu cầu (Requirements)

### 2.1 Yêu cầu 1: Quét dữ liệu thời gian thực từ PLC (PLC Realtime Polling)
**Objective:** As a System Service, I want to poll the PLC timer registers periodically, so that the system receives real-time stage execution time updates.

#### Acceptance Criteria
1. The Backend Accumulator Service shall read the 8 PLC timer registers corresponding to the 8 production stages using `RealtimeService.Instance.Read()` at a configurable frequency (default: 500ms).
2. While the active run status is "Active", the Backend Accumulator Service shall perform polling continuously.
3. If the connection to the PLC is lost, the Backend Accumulator Service shall log a warning and attempt to reconnect on the next poll cycle.

### 2.2 Yêu cầu 2: Thuật toán tích lũy thời gian chạy ở Backend kết hợp trạng thái Tạm dừng (State-driven Backend Accumulation Algorithm)
**Objective:** As a System Service, I want to calculate the accumulated run time of each step across Pause/Resume cycles using state transition signals, so that the calculation is simplified and immune to packet loss.

#### Acceptance Criteria
1. While the batch is running normally (PLC `STOP` is inactive and `is_paused = 0` in database), the Backend Accumulator Service shall calculate `Delta = T_new - T_prev` and accumulate this value.
2. When the system detects the batch is paused (either PLC `STOP` is active or `is_paused` in database transitions to `1`), the Backend Accumulator Service shall pause time accumulation and freeze the last read timer values (`T_frozen = T_last`).
3. When the system detects the batch transitions from paused back to running (PLC `STOP` becomes inactive or `is_paused` transitions from `1` to `0`), the Backend Accumulator Service shall flag a resume event, reset the timer baseline to `0` (or the new `T_new`), and continue accumulating delta values from the new baseline.
4. When the active `runId` changes or a new run starts, the Backend Accumulator Service shall reset all in-memory caches and initialize the database rows for the new run.

### 2.3 Yêu cầu 3: Lưu trữ dữ liệu tích lũy vào Cơ sở dữ liệu (Database Persistence)
**Objective:** As a System Service, I want to persist the accumulated times in the database, so that the data is not lost on server restart and is accessible by the web app controllers.

#### Acceptance Criteria
1. The database shall include a dedicated table `run_step_accumulated_times` with columns: `runId` (INT), `stepCode` (INT), and `accumulatedTime` (DOUBLE).
2. The Backend Accumulator Service shall write/update the computed accumulated times into the `run_step_accumulated_times` table in real-time.
3. The system shall maintain this table as the single source of truth for step execution times.

### 2.4 Yêu cầu 4: Đồng bộ hiển thị lên giao diện Web (UI Synchronization)
**Objective:** As a Web Operator, I want to view the accurate accumulated step times on the Overview timeline, so that I can monitor the batch progress correctly.

#### Acceptance Criteria
1. When a client requests `Overview/GetCurrentBatchStats`, the Overview Controller shall query the accumulated times from `run_step_accumulated_times` for the resolved `runId`.
2. The Overview Controller shall return these accumulated values in the JSON response under `batchInfo.accumulatedValues`.
3. When the Web Client receives the JSON response, the Frontend shall render the updated accumulated time values directly onto the timeline UI.
