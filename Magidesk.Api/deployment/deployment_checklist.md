# Production Readiness Checklist

## Infrastructure & Environment
- [ ] **Database**: PostgreSQL 14+ Instance provisioned and accessible from Web Server.
- [ ] **Network**: Web Server must be able to reach Printer IPs (Port 9100).
- [ ] **Time Sync**: Web Server NTP synchronization is active and verified.
- [ ] **SSL/TLS**: Valid certificate installed for HTTPS.
- [ ] **Firewall**: Port 443 open for WPA clients.

## Configuration (appsettings.Production.json)
- [ ] `ConnectionStrings:DefaultConnection` set to Production DB.
- [ ] `ConnectionStrings:Redis` (if used) set.
- [ ] `Logging:LogLevel:Default` set to "Warning" or "Error".
- [ ] `AllowedHosts` configured for specific domain.
- [ ] `Jwt:Key` and `Jwt:Issuer` match the Auth Service tokens.

## Client Assumptions
- [ ] Tablets (iPad/Android) are on the same subnet or have routed access to Web Server.
- [ ] Static IPs assigned to Kitchen Printers.

## Backend Wiring
- [ ] `Magidesk.Api` build artifacts deployed.
- [ ] Database Migrations applied (`dotnet ef database update`).
- [ ] Feature Flags: Printing ENABLED (if verified).

## Fallback
- [ ] "Break Glass" procedure: Shut down Web Server process. Operations continue on Desktop Terminals (DB is shared).
