import { Router } from "express";
import {
  createSessionHandler,
  listSessionsHandler,
  updateSessionStatusHandler
} from "../controllers/sessionController.js";

export const sessionRouter = Router();

sessionRouter.post("/sessions", createSessionHandler);
sessionRouter.get("/sessions", listSessionsHandler);
sessionRouter.patch("/sessions/:id", updateSessionStatusHandler);