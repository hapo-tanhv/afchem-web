# Design Guidelines

## Styling System
- **CSS Framework:** Bootstrap 3.4.1 (integrated within AdminLTE templates)
- **Custom Enhancements:** Vanilla CSS styled dark dashboards
- **Fonts:** Outfits, Inter, Roboto (sourced dynamically via Google Fonts API)
- **Icons:** FontAwesome 5.15.4 Free (`fa-users-cog`, `fa-user-plus`, `fa-key`, `fa-save`)

## Design Tokens (Color Palette)
- **Backgrounds:**
  - Card Panel: `#1e293b` (Deep Slate Slate)
  - Inputs & General backgrounds: `#0f172a` (Slate Black)
- **Text:**
  - Primary text: `#ffffff`
  - Secondary text / metadata labels: `#cbd5e1` or `#94a3b8` (Slate Grey)
- **Accents:**
  - Active links, primary actions: `#3b82f6` (Cyan-blue gradient)
  - Warning/Key reset actions: `#f59e0b` (Orange-gold accent)
  - Successful / Admin badges: `#10b981` (Emerald green)
  - Danger / Alarm badges: `#ef4444` (Vibrant crimson)

## Layout Guidelines
- **Modals:** Modal header, body, and footer must have explicit borders `1px solid rgba(255, 255, 255, 0.08)`. Input forms within modals must use the custom `.form-control-custom` style to prevent default white browser inputs.
- **Tables:** DataTables must be styled with high-contrast slate text labels and integrated dark input search controls to reduce eye strain. Active page pagination numbers must use the blue gradient outline.
