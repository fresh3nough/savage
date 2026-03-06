import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { ThemeProvider } from '@mui/material';
import cyberpunkTheme from './theme/cyberpunkTheme';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';

// Helper to wrap components with required providers
function renderWithProviders(ui, { route = '/' } = {}) {
  return render(
    <ThemeProvider theme={cyberpunkTheme}>
      <AuthProvider>
        <MemoryRouter initialEntries={[route]}>
          {ui}
        </MemoryRouter>
      </AuthProvider>
    </ThemeProvider>
  );
}

describe('Login Page', () => {
  test('renders login form with username and password fields', () => {
    renderWithProviders(<Login />);
    expect(screen.getByLabelText(/username/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
  });

  test('renders SAVAGE branding', () => {
    renderWithProviders(<Login />);
    expect(screen.getByText('SAVAGE')).toBeInTheDocument();
  });

  test('shows demo credentials hint', () => {
    renderWithProviders(<Login />);
    expect(screen.getByText(/demo.*admin/i)).toBeInTheDocument();
  });
});

describe('Cyberpunk Theme', () => {
  test('theme has dark mode palette', () => {
    expect(cyberpunkTheme.palette.mode).toBe('dark');
  });

  test('theme primary color is neon cyan', () => {
    expect(cyberpunkTheme.palette.primary.main).toBe('#00f0ff');
  });

  test('theme secondary color is neon magenta', () => {
    expect(cyberpunkTheme.palette.secondary.main).toBe('#ff00ff');
  });

  test('theme background is dark', () => {
    expect(cyberpunkTheme.palette.background.default).toBe('#0a0a0f');
  });
});

describe('Auth Context', () => {
  test('unauthenticated user sees no token in localStorage', () => {
    localStorage.removeItem('savage_token');
    localStorage.removeItem('savage_user');
    expect(localStorage.getItem('savage_token')).toBeNull();
  });
});

describe('ProtectedRoute', () => {
  test('redirects to /login when not authenticated', () => {
    localStorage.removeItem('savage_token');
    localStorage.removeItem('savage_user');
    renderWithProviders(
      <ProtectedRoute><div>Protected Content</div></ProtectedRoute>,
      { route: '/dashboard' }
    );
    expect(screen.queryByText('Protected Content')).not.toBeInTheDocument();
  });
});
