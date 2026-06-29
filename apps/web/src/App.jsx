import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import ViewerPage from "./pages/ViewerPage/ViewerPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/" replace />} />
      
        <Route path="/viewer" element={<ViewerPage />} />
      </Routes>
    </BrowserRouter>
  );
}