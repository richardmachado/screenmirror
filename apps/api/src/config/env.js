import path from "node:path";
import os from "node:os";

const defaultSnapshotsDir = path.join(
  os.homedir(),
  "Documents",
  "SecondScreen",
  "Snapshots"
);


export const config = {
  port: process.env.PORT || 4000,
  // For now assume a single host and a single device; we’ll refine later.
  snapshotsDir: process.env.SNAPSHOTS_DIR || defaultSnapshotsDir
};