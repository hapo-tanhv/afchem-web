import os

files_to_fix = [
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Controllers\OverviewController.cs",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Views\Home\Overview.cshtml",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\JavaScript\RealTime\OverviewRealtime.js",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Controllers\EventController.cs",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Service\BatchResolver.cs",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Views\Shared\_LayoutMain.cshtml",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\JavaScript\Common\LayoutMain.js"
]

for file_path in files_to_fix:
    if os.path.exists(file_path):
        print(f"Reading {file_path}...")
        # Read content
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            print("Successfully read as UTF-8.")
        except UnicodeDecodeError:
            try:
                with open(file_path, 'r', encoding='utf-16') as f:
                    content = f.read()
                print("Successfully read as UTF-16.")
            except UnicodeDecodeError:
                with open(file_path, 'r', encoding='latin-1') as f:
                    content = f.read()
                print("Successfully read as Latin-1.")
        
        # Write back as UTF-8 with BOM (utf-8-sig)
        with open(file_path, 'w', encoding='utf-8-sig') as f:
            f.write(content)
        print(f"Converted {file_path} to UTF-8 with BOM (utf-8-sig).\n")
    else:
        print(f"File not found: {file_path}\n")
