import { createSession, listSessions, updateSessionStatus } from "../services/sessionService.js";

export function createSessionHandler(req, res) {
  const { hostId } = req.body || {};
  if (!hostId) return res.status(400).json({ error: "hostId_required" });

  const session = createSession(hostId);
  res.status(201).json(session);
}

export function listSessionsHandler(req, res) {
  res.json({ sessions: listSessions() });
}

export function updateSessionStatusHandler(req, res) {
  const { id } = req.params;
  const { status } = req.body || {};
  if (!status) return res.status(400).json({ error: "status_required" });

  const session = updateSessionStatus(id, status);
  if (!session) return res.status(404).json({ error: "session_not_found" });
  res.json(session);
}