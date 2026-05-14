import { useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import ProtectedRoute from './components/auth/ProtectedRoute';
import Sidebar from './components/layout/Sidebar';
import TopBar from './components/layout/TopBar';
import LoginPage from './pages/LoginPage';
import PatientPage from './pages/PatientPage';
import CycleDashboard from './pages/CycleDashboard';
import LabPage from './pages/LabPage';
import CryoPage from './pages/CryoPage';
import RevenuePage from './pages/RevenuePage';
import PatientsPage from './pages/PatientsPage';
import DoctorsPage from './pages/DoctorsPage';
import LabTechniciansPage from './pages/LabTechniciansPage';
import AdministratorsPage from './pages/AdministratorsPage';
import RoomsPage from './pages/RoomsPage';
import './App.css';

const routeMeta = {
  '/patient': { title: 'Patient', subtitle: 'Search, chart, and appointments.' },
  '/cycle': { title: 'Cycle', subtitle: 'IVF cycle monitoring and phase control.' },
  '/lab': { title: 'Lab', subtitle: 'Test queue and chain of custody.' },
  '/cryo': { title: 'Cryo', subtitle: 'Cryostorage and donation bank.' },
  '/revenue': { title: 'Revenue', subtitle: 'Billing and service catalog.' },
  '/admin/patients': { title: 'Patients', subtitle: 'Administrator patient registry.' },
  '/admin/doctors': { title: 'Doctors', subtitle: 'Clinical staff management.' },
  '/admin/lab-technicians': { title: 'Lab Technicians', subtitle: 'Laboratory staff.' },
  '/admin/administrators': { title: 'Administrators', subtitle: 'System administrators.' },
  '/admin/rooms': { title: 'Rooms', subtitle: 'Procedure rooms.' },
};

const roleHome = {
  Administrator: '/admin/patients',
  Doctor: '/cycle',
  LabTechnician: '/lab',
  Patient: '/patient',
};

function HomeRedirect() {
  const { role, isAuthenticated } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return <Navigate to={roleHome[role] ?? '/patient'} replace />;
}

function Shell({ dark, onToggleDark }) {
  const { pathname } = useLocation();
  const meta = routeMeta[pathname] ?? { title: 'IVF Management', subtitle: '' };

  return (
  <>
    <Sidebar />
    <span className="main-content">
      <TopBar title={meta.title} subtitle={meta.subtitle} dark={dark} onToggleDark={onToggleDark} />
      <main className="route-area">
        <Routes>
          <Route path="/" element={<HomeRedirect />} />

          <Route path="/patient" element={<ProtectedRoute allowedRoles={['Administrator', 'Doctor', 'Patient']}><PatientPage /></ProtectedRoute>} />
          <Route path="/cycle" element={<ProtectedRoute allowedRoles={['Administrator', 'Doctor']}><CycleDashboard dark={dark} onToggleDark={onToggleDark} /></ProtectedRoute>} />
          <Route path="/lab" element={<ProtectedRoute allowedRoles={['Administrator', 'Doctor', 'LabTechnician']}><LabPage /></ProtectedRoute>} />
          <Route path="/cryo" element={<ProtectedRoute allowedRoles={['Administrator', 'LabTechnician']}><CryoPage /></ProtectedRoute>} />
          <Route path="/revenue" element={<ProtectedRoute allowedRoles={['Administrator', 'Doctor', 'Patient']}><RevenuePage /></ProtectedRoute>} />

          <Route path="/admin/patients" element={<ProtectedRoute allowedRoles={['Administrator']}><PatientsPage /></ProtectedRoute>} />
          <Route path="/admin/doctors" element={<ProtectedRoute allowedRoles={['Administrator']}><DoctorsPage /></ProtectedRoute>} />
          <Route path="/admin/lab-technicians" element={<ProtectedRoute allowedRoles={['Administrator']}><LabTechniciansPage /></ProtectedRoute>} />
          <Route path="/admin/administrators" element={<ProtectedRoute allowedRoles={['Administrator']}><AdministratorsPage /></ProtectedRoute>} />
          <Route path="/admin/rooms" element={<ProtectedRoute allowedRoles={['Administrator']}><RoomsPage /></ProtectedRoute>} />

          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
    </span>
  </>
  );
}

function NotFound() {
  return (
    <span className="not-found">
      <h2>Page not found</h2>
      <p>The route you requested does not match any page in this app.</p>
    </span>
  );
}

function AppRoutes() {
  const [dark, setDark] = useState(false);
  return (
    <span className={dark ? 'dark' : ''}>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/*" element={<Shell dark={dark} onToggleDark={() => setDark((d) => !d)} />} />
      </Routes>
    </span>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <span className="app-shell">
          <AppRoutes />
        </span>
      </BrowserRouter>
    </AuthProvider>
  );
}
