
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { LoginScreen } from './features/auth/LoginScreen';
import { TableGridScreen } from './features/tables/TableGridScreen';
import { TableSessionScreen } from './features/tables/TableSessionScreen';
import { MenuBrowserScreen } from './features/menu/MenuBrowserScreen';
import { OrderReviewScreen } from './features/order/OrderReviewScreen';
import { SessionSummaryScreen } from './features/session_summary/SessionSummaryScreen';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginScreen />} />
        <Route path="/tables" element={<TableGridScreen />} />
        <Route path="/session/:tableId" element={<TableSessionScreen />} />
        <Route path="/menu" element={<MenuBrowserScreen />} />
        <Route path="/order-review" element={<OrderReviewScreen />} />
        <Route path="/summary/:tableId" element={<SessionSummaryScreen />} />

        {/* Default route redirects to login */}
        <Route path="/" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
