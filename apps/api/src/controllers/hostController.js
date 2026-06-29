import { registerHost, listHosts, updateHostHeartbeat } from "../services/hostService.js";

export function registerHostHandler(req, res) {
  const host = registerHost(req.body || {});
  res.status(201).json(host);
}

export function listHostsHandler(req, res) {
  res.json({ hosts: listHosts() });
}

export function heartbeatHostHandler(req, res) {
  const { id } = req.params;
  const host = updateHostHeartbeat(id);
  if (!host) return res.status(404).json({ error: "host_not_found" });
  res.json(host);
}