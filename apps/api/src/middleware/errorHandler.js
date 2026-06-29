export function errorHandler(err, req, res, next) {
  // eslint-disable-next-line no-console
  console.error(`[${req.id || "no-id"}]`, err);
  if (res.headersSent) return next(err);
  res.status(500).json({ error: "internal_error" });
}