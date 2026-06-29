The MVP goal is simple: install a host component on a Windows PC, create one virtual monitor, let Windows extend the desktop onto it, and show that monitor inside a secure browser session. This is different from screen sharing because the streamed content should come from the virtual display, not from your current physical monitor.

For v1, skip multi-monitor management, fancy resizing, audio, and mobile support. Focus on one reliable virtual display, one browser viewer, strict auth, and basic host controls.

System pieces
Build the MVP as four parts:

React web app for signup, login, session creation, approval, and viewer UI.

Node/Express backend for auth verification, session state, signaling, and policy checks.

Windows host agent installed on the PC that creates or manages the virtual monitor and captures that display.

Virtual display driver that makes Windows think a second monitor exists. Existing projects show this is feasible without inventing the driver model yourself.

The browser cannot create the monitor, but it can absolutely act as the remote window onto that monitor. getDisplayMedia() is useful for capturing existing displays, but the real second-screen behavior comes from the host-side virtual display layer.

Build phases
Phase 1
Get a virtual display working locally on Windows first. The test is: Windows display settings show an extra monitor, and you can drag a window onto it with no web app involved. The Microsoft indirect display sample and existing virtual display driver projects are the right starting references here.

Phase 2
Build a small host agent that detects the virtual display, starts/stops it, and captures that display’s framebuffer or stream source. This host agent should expose only local controls and authenticate to your backend, rather than leaving any direct local endpoint open.

Phase 3
Use your existing React app for auth and session workflow: sign up, log in, create session, approve session, then connect viewer. Supabase React auth supports the client-side auth flow, while the backend should still verify tokens before allowing signaling or joining a session.

Phase 4
Wire the host agent to stream the virtual display to the browser viewer. For MVP, you can use WebRTC for low-latency viewing if the host agent can provide the video source cleanly; the important part is that the source is the virtual monitor, not the physical screen.

Data flow
The intended flow is:

User logs into the React app.

React gets the current session token.

User creates a remote-display session.

Host agent checks in and is approved for that session.

Backend marks the session active.

Host agent starts the virtual monitor if needed.

Windows extends the desktop onto that virtual monitor.

Viewer opens the browser page and receives the streamed virtual display.

That gives you the behavior you want: open an app on your PC, drag it to monitor 2, and see monitor 2 remotely in the browser. The desktop extension itself is done by Windows because the virtual driver makes the display real from the OS perspective.

Tech choices
For fastest MVP, use:

React + Vite for the web UI, since you already work comfortably there.

Node/Express for backend control and signaling.

Supabase Auth for login/session identity.

Existing virtual display driver rather than writing your own first. The Virtual Display Driver project and Microsoft’s sample show the practical base you’d build around.

Windows host agent in a language that gives you solid Windows integration, such as C#, C++, or Rust.

I would not write the driver from scratch for MVP. The lower-risk move is to integrate an existing driver and spend your time on the host agent, auth, session control, and stream pipeline.

Acceptance tests
Your MVP is successful when these all work:

Windows shows one extra monitor after host setup.

You can drag Notepad or Chrome onto that extra display.

The browser viewer shows the content from that virtual monitor, not your main monitor.

Unauthorized users cannot connect to the session.

Disconnecting the browser does not crash the host PC or leave the system unusable.

Biggest risks
The hardest part is not React or auth — it is the host-side display and capture pipeline. Virtual display drivers are real systems-level software, and getting stable creation, resolution handling, and stream capture around them is the main engineering risk.

The second biggest risk is assuming browser APIs alone can do this. They cannot create the monitor; they can only participate after the host-side virtual display exists.

Recommended next step
Start with this order:

Prove a virtual monitor appears on Windows.

Build a tiny host agent that can detect and manage it.

Reuse your React auth/session control page.

Add browser viewing last.

If you want, I can turn this into a concrete repo plan next — folders, services, milestones, and what each file/module should do.

yes please

Here’s the concrete MVP repo plan: one repo can work, but a monorepo with three apps is the cleanest structure — a React web app for auth/control/viewer, a Node/Express API for session state and signaling, and a Windows host agent that manages the virtual display and streams it. That matches the real system boundaries: React handles user workflow, Node handles policy and real-time coordination, and the Windows side handles the actual virtual monitor through the Indirect Display Driver path.

Repo layout
Use this structure:

text
second-screen/
  apps/
    web/
    api/
    host-agent/
  packages/
    shared-types/
    shared-config/
  infra/
    supabase/
    docs/
This separation keeps browser code away from OS-specific code, which matters because the virtual monitor piece depends on Windows display-driver behavior while the web UI does not. Microsoft’s indirect display model is specifically for non-traditional monitors, and existing virtual display driver projects are the right dependency/reference point for the host side.

apps/web
This is your React + Vite app. It should contain:

auth pages

session control page

host/viewer pages

basic dashboard showing host status and active session

later, the browser viewer for the virtual display stream

Suggested structure:

text
apps/web/
  src/
    lib/
      supabase.js
      api.js
    pages/
      AuthPage.jsx
      SessionControlPage.jsx
      HostPage.jsx
      ViewerPage.jsx
      DashboardPage.jsx
    components/
      AuthForm.jsx
      SessionCard.jsx
      HostStatusBadge.jsx
    hooks/
      useSessionAuth.js
      useHostStatus.js
Supabase has a React quickstart built around Vite and @supabase/supabase-js, so this part is very standard.

apps/api
This is your Node/Express backend. It should own:

verifying Supabase Bearer tokens

session creation and approval

host registration

host heartbeat

signaling for viewer/host coordination

strict access control for who can attach to which display session

Suggested structure:

text
apps/api/
  src/
    app.js
    server.js
    config/
      env.js
      supabaseAdmin.js
    middleware/
      requireAuth.js
    routes/
      authRoutes.js
      sessionRoutes.js
      hostRoutes.js
      signalRoutes.js
    services/
      sessionService.js
      hostService.js
      signalService.js
    utils/
      errors.js
WebSocket signaling belongs here because it is the coordination layer between browser viewer and host agent. WebSockets are designed for persistent two-way communication, which is exactly what you need for session state and signaling events.

apps/host-agent
This is the most important non-web piece. It should run on Windows and do four jobs:

register the host machine with your backend

start or verify the virtual display

capture or expose that virtual display as the stream source

connect to your backend/signaling service as the trusted host

Suggested structure:

text
apps/host-agent/
  src/
    main/
      agent-entry
      config
      logger
    auth/
      deviceAuth
    host/
      heartbeat
      sessionListener
    display/
      driverManager
      virtualDisplayManager
      displayEnumerator
      capturePipeline
    signaling/
      wsClient
      rtcBridge
The host agent should be where you integrate an existing virtual display driver or the Microsoft sample-based approach. That is the layer that makes Windows see a second monitor.

packages/shared-types
Put common DTOs and event names here so the web app, API, and host agent agree on shapes:

text
packages/shared-types/
  session.js
  host.js
  signal.js
Examples:

SessionStatus = pending | approved | active | closed

HostState = offline | online | busy

signaling events like viewer_join_requested, host_ready, session_approved

This keeps your frontend and backend from drifting.

packages/shared-config
Put common constants here:

route names

event names

session timeouts

environment variable names

feature flags

text
packages/shared-config/
  env.js
  constants.js
infra/supabase
Keep SQL and policies here:

tables

RLS policies

seed scripts

docs on auth flow

Suggested files:

text
infra/supabase/
  migrations/
    001_hosts.sql
    002_sessions.sql
    003_session_events.sql
  policies/
    hosts_rls.sql
    sessions_rls.sql
Core tables for MVP:

profiles

hosts

display_sessions

session_events

The web app signs users in with Supabase, but the backend still needs to verify identity and session access before creating or approving remote display sessions.

docs
Put planning docs here:

architecture

threat model

driver integration notes

protocol notes

text
infra/docs/
  architecture.md
  mvp-plan.md
  host-agent-notes.md
  virtual-display-notes.md
This is where I’d keep your research links for the Windows indirect display model and the virtual display driver integration choices.

MVP milestones
Build in this order:

Milestone	Goal
M1	React auth works with Supabase
M2	API can create/approve sessions
M3	Host agent registers and heartbeats
M4	Virtual monitor appears in Windows
M5	Host agent can target that virtual monitor
M6	Viewer can attach to an active session
M7	Secure end-to-end flow works
The key idea is to prove the Windows virtual display before spending too much time on the viewer. If Windows cannot see monitor 2, the project is still just screen sharing.

First files to create
Start with these exact first deliverables:

apps/web: supabase.js, AuthPage.jsx, SessionControlPage.jsx

apps/api: server.js, requireAuth.js, sessionRoutes.js, hostRoutes.js

apps/host-agent: agent-entry, deviceAuth, virtualDisplayManager, wsClient

infra/supabase: hosts.sql, display_sessions.sql

That gives you the smallest vertical slice: login, create session, approve session, host online, virtual display managed.