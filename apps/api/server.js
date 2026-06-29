import express from "express";
import cors from "cors";
import { config } from "./src/config/env.js";
import { httpLogger } from "./src/utils/logger.js";
import { requestId } from "./src/middleware/requestId.js";
import { errorHandler } from "./src/middleware/errorHandler.js";
import { healthRouter } from "./src/routes/healthRoutes.js";
import { hostRouter } from "./src/routes/hostRoutes.js";
import { sessionRouter } from "./src/routes/sessionRoutes.js";
import { snapshotRouter } from "./src/routes/snapshotRoutes.js";

const app = express();

app.use(cors());
app.use(express.json());
app.use(requestId);
app.use(httpLogger);
app.use(snapshotRouter);

app.use(healthRouter);
app.use(hostRouter);
app.use(sessionRouter);

app.use(errorHandler);

app.listen(config.port, () => {
  console.log(`API listening on port ${config.port}`);
});