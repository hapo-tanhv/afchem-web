# Requirements Document

## Introduction
Tính năng Lựa chọn Batch Chạy Linh hoạt (flexible-batch-execution) cho phép người vận hành SCADA có thể tùy chọn mẻ chạy (run) tiếp theo từ danh sách các batch đang chờ (standby), thay vì bắt buộc chạy tuần tự theo thứ tự ban đầu. 

Hệ thống hiện tại đã có cấu trúc dữ liệu lưu trữ các batch (`batches`) và mẻ chạy (`runs`), cùng với helper `BatchResolver` để giải quyết batch/run hoạt động dựa trên trạng thái `Active`. Giao diện `Overview.cshtml` đã thực hiện polling realtime trạng thái thông qua `OverviewController.GetCurrentBatchStats` và `OverviewRealtime.js`.

Yêu cầu mới sẽ bổ sung giao diện chọn batch/mẻ ở phần đầu trang Overview, đồng thời bổ sung logic backend cập nhật trạng thái và trường thứ tự chạy `execution_order` trong database khi thay đổi mẻ chạy.

## Requirements

### Requirement 1: Giao diện chọn Batch/Mẻ chạy trên trang Overview
**Objective:** As an Operator, I want to select and start a standby batch and run from the top of the Overview page, so that I can customize the production sequence based on actual demands.

#### Acceptance Criteria
1. The System shall display a new premium card titled "Lựa chọn Batch" at the top of the Overview dashboard.
2. The System shall display the total count of standby batches and standby runs on the left side of the "Lựa chọn Batch" panel.
3. The System shall display two drop-down select controls (one for Batch and one for Run) and a "Bắt đầu mẻ" confirmation button on the right side of the "Lựa chọn Batch" panel.
4. While any run in the database has the status 'Active', the System shall disable the Batch select, Run select, and "Bắt đầu mẻ" button to prevent interrupting the running process.
5. While no runs in the database have the status 'Active', the System shall enable the Batch select, Run select, and "Bắt đầu mẻ" button.
6. When the user selects a Batch from the Batch selector, the Run selector shall dynamically filter and display only the standby runs (e.g. status 'Pending', 'Waiting', or 'Created') belonging to that Batch.
7. When the user clicks the "Bắt đầu mẻ" button and confirms, the System shall send an AJAX POST request containing the selected `batchId` and `runId` to the backend.

### Requirement 2: API cập nhật trạng thái Batch/Mẻ và thứ tự chạy ở Backend
**Objective:** As a SCADA System, I want to expose an API to change the active batch and run, updating their status and execution order in the database, so that the SCADA database state correctly reflects the operator's choice.

#### Acceptance Criteria
1. When the backend receives a request to start a specific batch and run, the System shall verify if there is any run in the `runs` table with status 'Active'.
2. If an active run is found, the System shall reject the request and return an error message to the client.
3. When the request is valid and no active run exists:
   - The System shall update the status of any currently 'Active' batches to 'Pending'.
   - The System shall update the status of the selected batch to 'Active'.
   - The System shall update the status of the selected run to 'Active' and set its `start_time` to the current system time.
   - The System shall query the maximum value of `execution_order` from the `runs` table.
   - The System shall set the selected run's `execution_order` to `MAX(execution_order) + 1` to place it next in the execution sequence.
4. When database updates succeed, the System shall return a JSON success response.
