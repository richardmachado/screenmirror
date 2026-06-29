import { Router } from "express";
import fs from "node:fs";
import path from "node:path";
import { config } from "../config/env.js";

export const snapshotRouter = Router();

// GET /snapshots/latest?device=<deviceName>
snapshotRouter.get("/snapshots/latest", (req, res, next) => {
  const device = req.query.device;
  if (!device) {
    return res.status(400).json({ error: "device_query_required" });
  }

  // For now, device should match the folder name used by SnapshotDebugService.
  const deviceDir = path.join(config.snapshotsDir, device);
  const latestPath = path.join(deviceDir, "latest.png");

  fs.access(latestPath, fs.constants.F_OK, (err) => {
    if (err) {
      return res.status(404).json({ error: "latest_snapshot_not_found" });
    }

    res.sendFile(latestPath, (sendErr) => {
      if (sendErr) {
        return next(sendErr);
      }
    });
  });
});