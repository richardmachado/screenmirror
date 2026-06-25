# screenmirror
Create a second screen using any browser on any device even if not on same network

Remote Browser Second Screen
A secure browser-based second-screen system that works from any network by using a local host agent on the computer, a cloud control plane, and a remote browser viewer. The system is designed so the browser never directly discovers or reaches the host without first passing strict authentication, authorization, and session checks.

Goal
Build a remote second-screen product that lets a user open a secure viewer in any browser and connect to a computer from anywhere, even across different networks. The main priority is security: no accidental access, no guessable room codes, and strong protection against brute-force attempts.

Core architecture
The system has three parts:

Host app on the computer

Runs locally on the machine that is sharing the screen.

Captures the chosen display or window.

Connects outward to the backend instead of exposing an inbound port.

Authenticates as a trusted device using revocable service credentials rather than a reusable shared password.

Cloud control plane

Handles authentication, device registration, signaling, session creation, audit logging, and policy enforcement.

Should be built so all sensitive access decisions happen on the backend, which matches the preferred security pattern of keeping privileged auth logic out of the frontend.

Remote browser viewer

Lets the user sign in from any browser.

Requests access to a specific host.

Only receives a short-lived session after identity checks, MFA, and approval pass.

Stays thin and untrusted; it should never hold backend secrets or make final authorization decisions.

Recommended stack
Frontend: React viewer app.

Backend: Node.js + Express API.

Database/Auth: Supabase for users, devices, session records, and audit logs.

Access gateway: Cloudflare Access / Zero Trust in front of the web app and sensitive backend routes.

Transport: WebRTC for the stream and control/data channel, with HTTPS/WSS signaling over TLS.

This stack fits existing development patterns well because backend-enforced auth and server-side privilege checks are already preferred over frontend-trusted logic.

Security principles
1. No public room codes
Do not use public join codes, predictable URLs, or static invite links. Access should require a real authenticated account, and session creation should happen only through the backend after all checks pass.

2. MFA required
Put the app behind an access layer that requires multi-factor authentication before a user can even reach the protected app. Cloudflare Access supports MFA policies at the application and policy level, which is a strong first gate before app-level auth even begins.

3. Host identity is separate from user identity
The host device should authenticate with service credentials, while the human user authenticates with their own account plus MFA. Cloudflare service tokens use a client ID and client secret model and can be revoked when needed, making them better than a shared static password for host-to-service trust.

4. Single-use session grants
Every connection attempt should generate a one-time session token that is:

bound to one host,

bound to one viewer,

short-lived,

unusable after first redemption.

This limits replay and makes leaked links or tokens far less useful.

5. Backend is the gatekeeper
The frontend should never decide who is allowed to connect. The server must verify user identity, MFA status, device trust, rate limits, session freshness, and host approval before releasing signaling data or session credentials.

6. Rate limiting everywhere
Protect login, device registration, session requests, join attempts, and signaling endpoints with rate limits. Supabase documents configurable auth rate limits, but app-specific and edge-level rate limiting should still be added for stricter brute-force defense.

Connection flow
User installs and signs into the host app on the computer.

Host app registers itself as a trusted device and receives revocable device credentials.

User opens the remote viewer in any browser.

Viewer is blocked by the access layer until the user passes login + MFA.

After entering the app, the user chooses a host machine.

Backend creates a short-lived one-time session grant.

Host receives a prompt like: "Approve connection from Chrome on macOS, San Diego, CA."

Host must explicitly approve the request unless the viewer device is already trusted.

Only then does the backend allow signaling details to be exchanged.

Viewer and host establish the WebRTC connection.

Session starts with strict idle timeout, revoke controls, and audit logging.

Strict security checklist
Require account login for every viewer session.

Require MFA for every protected app login.

Use service credentials for the host app, not a shared password.

Store only hashed or revocable secrets server-side.

Use one-time, short-lived session grants.

Add rate limiting on auth and session endpoints.

Lock out or heavily delay repeated failed attempts.

Require host approval for new viewer devices.

Maintain a trusted-device list with easy revoke capability.

Log every access attempt, approval, denial, start, stop, and revoke event.

Re-authenticate for especially sensitive actions such as adding a new trusted device or changing security settings.

Optionally add IP, country, and device posture restrictions at the access layer for an even tighter setup.

Suggested database tables
users
id

email / auth provider id

mfa_required

created_at

host_devices
id

user_id

device_name

public_key_or_token_id

last_seen_at

revoked_at

trusted_status

viewer_devices
id

user_id

browser_fingerprint_or_device_label

first_seen_at

last_seen_at

trusted_status

revoked_at

sessions
id

user_id

host_device_id

viewer_device_id

status

one_time_token_hash

expires_at

redeemed_at

approved_at

started_at

ended_at

audit_logs
id

user_id

host_device_id

viewer_device_id

event_type

ip_address

user_agent

geo_hint

created_at

API outline
Auth / access
POST /auth/login

POST /auth/logout

GET /me

POST /mfa/verify

Host devices
POST /devices/host/register

POST /devices/host/heartbeat

POST /devices/host/revoke

GET /devices/host

Viewer devices
POST /devices/viewer/trust

POST /devices/viewer/revoke

GET /devices/viewer

Sessions
POST /sessions/request

POST /sessions/approve

POST /sessions/deny

POST /sessions/redeem

POST /sessions/revoke

GET /sessions/active

Signaling
POST /signal/offer

POST /signal/answer

POST /signal/ice

All of these protected routes should be backend-validated and should never trust client-side flags alone.

WebRTC notes
WebRTC data channels are encrypted, and the browser APIs support secure peer communication, but encryption alone does not solve unauthorized access. The signaling path, token issuance, device trust, and approval flow are the real control points that determine whether the system is safe.

MVP plan
Phase 1
Build login, MFA gate, host registration, and session request flow.

Add manual host approval for every connection.

Create audit logs and a simple active session dashboard.

Phase 2
Add trusted viewer devices.

Add session revoke, timeout, and re-auth flows.

Add stronger rate limiting and abnormal-attempt monitoring.

Phase 3
Add optional stricter controls such as IP restrictions, geo restrictions, and higher-risk step-up auth for unusual logins.

Non-goals
No open public session links.

No permanent join codes.

No frontend-only authorization checks.

No direct inbound host exposure on the public internet.

No single shared password protecting all access.

First implementation recommendation
Start with the secure version, not the convenient version. The best first build is a React viewer, a Node/Express signaling backend, Supabase for users and records, Cloudflare Access in front, and a local host agent that authenticates with revocable service credentials and only accepts one-time approved sessions