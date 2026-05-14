# UI Specification: Product Information & BOM Section

I am building a C# Windows Forms application using HTML/CSS for the UI. I need to generate two side-by-side panels: "Product Information" (left) and "Production BOM" (right).

## 1. General Layout
- Both panels should have a dark background (`#0a192f` or similar).
- Titles for each section must have a vertical Cyan accent line (`4px` width) on the left.
- Use a clean Sans-serif font (e.g., Segoe UI).

## 2. Left Panel: Product Information (Thông tin sản phẩm)
This panel displays data in a key-value pair format with a specific alignment:
- **Structure**: Each row contains a label (left) and a value (right).
- **Alignment**: 
    - **Labels (Left text)**: Must be **aligned to the left**.
    - **Values (Right text)**: Must be **aligned to the right**.
- **Styling**:
    - Add a subtle horizontal divider (dashed or solid dark line) between rows.
    - Values should have specific colors: 
        - Batch ID & Operator: **Cyan**.
        - Loss Rate (Tỉ lệ hao hụt): **Red/Coral**.
        - Others: **White/Off-white**.
    - Labels should be a slightly muted gray.

## 3. Right Panel: Production BOM (BOM sản xuất)
This section should reuse the style from the previous `batch-stats-section` (industrial table style).
- **Table Columns (5 columns)**:
    1. **Item Code (Mã hàng)**: Text in **Cyan**.
    2. **Material Name (Tên NTL)**: Text in White.
    3. **Ratio (Tỉ lệ)**: Bold/White.
    4. **Unit (ĐVT)**: Muted gray.
    5. **Lot Number (LOT NTL)**: White.
- **Styling**:
    - Header row should have muted gray text and be all-caps.
    - Align "Item Code" to the left; numerical values (Ratio) should be centered or right-aligned for readability.

## 4. Technical Details for WinForms Context
- The two panels should be wrapped in a container using `display: flex` or a 2-column grid.
- Ensure the layout is responsive to the WinForms window size.
- Use `padding` (around 15-20px) for each panel to create a "card" effect.

## 5. Sample Content
### Product Info:
- Batch ID: A2B260504TX01 (Cyan, Right)
- Mass: 2000 kg (Right)
- Operator: Trần Quang Thảo (Cyan, Right)
- Loss Rate: 8.5kg/mẻ (Red, Right)

### BOM Table:
- AF-01 | 153 | 600 | kg | 20251222TRF
- CITRIC ACID | 10 | 40 | kg | BP00012-2