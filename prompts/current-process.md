# UI Specification: Industrial Dashboard Table (C# WinForms / Web Context)

I am building a C# Windows Forms application that renders UI using HTML/CSS. I need the code for a production monitoring table with the following specifications:

## 1. Grid Layout & Columns
The table consists of 10 logical data points organized into 9 visual columns:
1. **Process Step**: (Text)
2. **Standard Time**: (e.g., 12m)
3. **Start Time**: (Time format)
4. **End Time**: (Time format)
5. **Duration**: (Highlighted text)
6. **Temp Top**: (Numeric range)
7. **Temp Mid**: (Numeric range)
8. **Temp Bottom**: (Numeric range)
9. **Status**: (Status Badge/Label)
10. **Alerts**: **CRITICAL:** This column must have a width equivalent to **3 standard columns** (Colspan = 3) to accommodate warning messages.

## 2. Visual Style (Dark Mode)
* **Background**: Deep Navy/Dark Blue (`#0a192f`).
* **Row Hover/Selection**: Subtle highlight with a brighter border.
* **Header**: Small, muted gray text, all-caps.
* **Typography**: Clean Sans-serif. Use **Cyan** for active values and **Orange** for warnings.

## 3. Status Badge Logic
* **Completed (Hoàn thành)**: Dark green background, bright green text, outlined.
* **In Progress (Đang thực hiện)**: Dark orange background, bright orange text.
* **Pending (Chưa thực hiện)**: Dimmed gray text.

## 4. Specific Requirements
* **Left Accent**: The table header must have a vertical Cyan line (`4px` width) next to the "Current Batch Statistics" title.
* **Column Alignment**: Process Step should be left-aligned; numerical values should be center-aligned; Status and Alerts should be center/right-aligned.
* **Responsive**: Ensure the table fills the width of the WinForms container.

## 5. Sample Row Data
- Step: "Mixing 2"
- Standard: "20m"
- Start: "9:10:00 AM"
- End: "-"
- Duration: "10m" (Cyan)
- Temp: "29-32°C"
- Status: "In Progress" (Orange Badge)
- Alert: "7 minutes remaining" (Orange text, spanning 3-column width)