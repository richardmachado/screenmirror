import { Router } from "express";
import {
  registerHostHandler,
  listHostsHandler,
  heartbeatHostHandler
} from "../controllers/hostController.js";

export const hostRouter = Router();

hostRouter.post("/hosts/register", registerHostHandler);
hostRouter.get("/hosts", listHostsHandler);
hostRouter.post("/hosts/:id/heartbeat", heartbeatHostHandler);