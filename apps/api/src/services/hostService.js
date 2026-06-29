const hosts = new Map(); // hostId -> { id, name, lastSeen }

export function registerHost(payload) {
  const id = payload.id || `host_${Date.now()}`;
  const name = payload.name || "Unnamed host";
  const now = new Date().toISOString();

  const host = { id, name, lastSeen: now };
  hosts.set(id, host);
  return host;
}

export function listHosts() {
  return Array.from(hosts.values());
}

export function updateHostHeartbeat(id) {
  const host = hosts.get(id);
  if (!host) return null;
  host.lastSeen = new Date().toISOString();
  return host;
}