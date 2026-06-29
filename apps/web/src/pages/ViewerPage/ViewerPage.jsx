import { useEffect, useState } from "react";

export default function ViewerPage() {
  const [device, setDevice] = useState("__._DISPLAY6");
  const [tick, setTick] = useState(0);

  useEffect(() => {
    const id = setInterval(() => {
      setTick((t) => t + 1);
    }, 1000); // refresh every 1s
    return () => clearInterval(id);
  }, []);

  const src =
    device && device.length > 0
      ? `http://localhost:4000/snapshots/latest?device=${encodeURIComponent(
          device
        )}&t=${tick}`
      : null;

  return (
    <div style={{ padding: 24 }}>
      <h1>Viewer</h1>

      <label>
        Device folder:
        <input
          value={device}
          onChange={(e) => setDevice(e.target.value)}
          style={{ marginLeft: 8 }}
        />
      </label>

      <div style={{ marginTop: 16 }}>
        {src ? (
          <img
            src={src}
            alt="Latest snapshot"
            style={{ border: "1px solid #ccc", maxWidth: "100%" }}
          />
        ) : (
          <p>Enter a device folder to view snapshots.</p>
        )}
      </div>
    </div>
  );
}