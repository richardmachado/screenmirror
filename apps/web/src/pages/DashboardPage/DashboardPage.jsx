import { useEffect, useState } from "react";

export default function DashboardPage() {
  const [hosts, setHosts] = useState([]);
  const [sessions, setSessions] = useState([]);

  useEffect(() => {
    fetch("http://localhost:4000/hosts")
      .then((res) => res.json())
      .then((data) => setHosts(data.hosts || []))
      .catch((err) => console.error("hosts fetch error", err));

    fetch("http://localhost:4000/sessions")
      .then((res) => res.json())
      .then((data) => setSessions(data.sessions || []))
      .catch((err) => console.error("sessions fetch error", err));
  }, []);

  return (
    <div style={{ padding: 24 }}>
      <h1>Second Screen Dashboard</h1>

      <section>
        <h2>Hosts</h2>
        {hosts.length === 0 && <p>No hosts registered.</p>}
        <ul>
          {hosts.map((h) => (
            <li key={h.id}>
              {h.name} (id: {h.id}) – last seen {h.lastSeen}
            </li>
          ))}
        </ul>
      </section>

      <section>
        <h2>Sessions</h2>
        {sessions.length === 0 && <p>No sessions yet.</p>}
        <ul>
          {sessions.map((s) => (
            <li key={s.id}>
              {s.id} on host {s.hostId} – {s.status}
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}