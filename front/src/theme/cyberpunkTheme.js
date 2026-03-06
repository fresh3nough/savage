import { createTheme } from '@mui/material/styles';

/**
 * Cyberpunk/futuristic Material UI theme.
 * Dark backgrounds with neon cyan and magenta accents, glowing borders and shadows.
 */
const cyberpunkTheme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#00f0ff',      // neon cyan
      light: '#66f7ff',
      dark: '#00b8c4',
      contrastText: '#0a0a0f',
    },
    secondary: {
      main: '#ff00ff',      // neon magenta
      light: '#ff66ff',
      dark: '#c400c4',
      contrastText: '#0a0a0f',
    },
    background: {
      default: '#0a0a0f',   // deep dark
      paper: '#12121a',     // slightly lighter dark
    },
    text: {
      primary: '#e0e0ff',
      secondary: '#a0a0c0',
    },
    error: {
      main: '#ff3366',
    },
    warning: {
      main: '#ffaa00',
    },
    success: {
      main: '#00ff88',
    },
    divider: 'rgba(0, 240, 255, 0.15)',
  },
  typography: {
    fontFamily: '"Rajdhani", "Orbitron", "Roboto Mono", monospace',
    h4: {
      fontWeight: 700,
      letterSpacing: '0.05em',
      textTransform: 'uppercase',
    },
    h5: {
      fontWeight: 600,
      letterSpacing: '0.03em',
    },
    h6: {
      fontWeight: 600,
      letterSpacing: '0.02em',
    },
    button: {
      fontWeight: 700,
      letterSpacing: '0.1em',
      textTransform: 'uppercase',
    },
  },
  shape: {
    borderRadius: 4,
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundImage: 'radial-gradient(ellipse at 50% 0%, rgba(0,240,255,0.05) 0%, transparent 60%)',
          minHeight: '100vh',
        },
        '@import': "url('https://fonts.googleapis.com/css2?family=Rajdhani:wght@400;500;600;700&family=Orbitron:wght@400;700&display=swap')",
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          border: '1px solid rgba(0, 240, 255, 0.12)',
          boxShadow: '0 0 15px rgba(0, 240, 255, 0.05), inset 0 0 15px rgba(0, 240, 255, 0.02)',
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 2,
          boxShadow: 'none',
          '&:hover': {
            boxShadow: '0 0 20px rgba(0, 240, 255, 0.3)',
          },
        },
        containedPrimary: {
          background: 'linear-gradient(135deg, #00f0ff 0%, #0080ff 100%)',
          '&:hover': {
            background: 'linear-gradient(135deg, #33f3ff 0%, #3399ff 100%)',
          },
        },
        containedSecondary: {
          background: 'linear-gradient(135deg, #ff00ff 0%, #8800ff 100%)',
          '&:hover': {
            background: 'linear-gradient(135deg, #ff33ff 0%, #aa33ff 100%)',
          },
        },
      },
    },
    MuiTextField: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            '& fieldset': {
              borderColor: 'rgba(0, 240, 255, 0.25)',
            },
            '&:hover fieldset': {
              borderColor: 'rgba(0, 240, 255, 0.5)',
            },
            '&.Mui-focused fieldset': {
              borderColor: '#00f0ff',
              boxShadow: '0 0 10px rgba(0, 240, 255, 0.2)',
            },
          },
        },
      },
    },
    MuiTableHead: {
      styleOverrides: {
        root: {
          '& .MuiTableCell-head': {
            backgroundColor: 'rgba(0, 240, 255, 0.08)',
            color: '#00f0ff',
            fontWeight: 700,
            textTransform: 'uppercase',
            letterSpacing: '0.05em',
            borderBottom: '2px solid rgba(0, 240, 255, 0.3)',
          },
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        root: {
          borderBottom: '1px solid rgba(0, 240, 255, 0.08)',
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: {
          fontWeight: 600,
          letterSpacing: '0.03em',
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          background: 'linear-gradient(90deg, #0a0a0f 0%, #12121a 50%, #0a0a0f 100%)',
          borderBottom: '1px solid rgba(0, 240, 255, 0.2)',
          boxShadow: '0 2px 20px rgba(0, 240, 255, 0.1)',
        },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          background: 'linear-gradient(180deg, #0a0a0f 0%, #0e0e18 100%)',
          borderRight: '1px solid rgba(0, 240, 255, 0.15)',
        },
      },
    },
  },
});

export default cyberpunkTheme;
