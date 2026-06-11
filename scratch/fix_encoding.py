import os

files = [
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Views\Home\Alarm.cshtml",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\JavaScript\Event\EventPage.js",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Views\Home\Report.cshtml",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Controllers\HomeController.cs",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Controllers\OverviewController.cs",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Views\Shared\_LayoutMain.cshtml",
    r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\JavaScript\Common\LayoutMain.js"
]

for file_path in files:
    if not os.path.exists(file_path):
        print(f"Error: File not found: {file_path}")
        continue
    
    # Try reading as utf-8-sig, fallback to utf-8, fallback to windows-1258/utf-8 with errors ignore
    content = None
    encodings = ['utf-8-sig', 'utf-8', 'windows-1258', 'utf-16']
    for enc in encodings:
        try:
            with open(file_path, 'r', encoding=enc) as f:
                content = f.read()
            print(f"Read {file_path} using encoding: {enc}")
            break
        except UnicodeDecodeError:
            continue
            
    if content is None:
        # Fallback reading with error ignoring
        with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
            content = f.read()
        print(f"Read {file_path} using utf-8 with errors ignored")
        
    # Write back as utf-8-sig (UTF-8 with BOM)
    with open(file_path, 'w', encoding='utf-8-sig', newline='\r\n') as f:
        f.write(content)
    print(f"Successfully converted and saved {file_path} as UTF-8 with BOM.\n")

print("All encoding fixes complete.")
