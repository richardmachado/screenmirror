import { nanoid } from "nanoid";

export function requestId(req, res, next) {
  req.id = nanoid(8);
  res.setHeader("X-Request-Id", req.id);
  next();
}