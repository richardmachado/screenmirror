const sessions = new Map(); // sessionId -> { id, hostId, status, createdAt }

export function createSession(hostId) {
  const id = `sess_${Date.now()}`;
  const session = {
    id,
    hostId,
    status: "pending",
    createdAt: new Date().toISOString()
  };
  sessions.set(id, session);
  return session;
}

export function listSessions() {
  return Array.from(sessions.values());
}

export function updateSessionStatus(id, status) {
  const session = sessions.get(id);
  if (!session) return null;
  session.status = status;
  return session;
}