
# WPA Architectural Rules

- WPA must never reference WinUI projects
- WPA must never access ApplicationDbContext directly
- WPA must only communicate via services
- WPA UI contains NO business logic
- All pricing, billing, and validation live in backend
