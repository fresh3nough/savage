import React, { useEffect, useState } from 'react';
import {
  Box, Typography, Table, TableHead, TableRow, TableCell, TableBody,
  Paper, Button, TextField, IconButton, Dialog, DialogTitle,
  DialogContent, DialogActions, Chip, CircularProgress, Alert, TableContainer,
  FormControl, InputLabel, Select, MenuItem,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { vendorApi } from '../../services/api';

const emptyForm = { name: '', contactEmail: '', contactPhone: '', address: '', status: 'Active' };

/**
 * Vendor management page for admins. Full CRUD with status toggling.
 */
function VendorsPage() {
  const [vendors, setVendors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editId, setEditId] = useState(null);
  const [form, setForm] = useState(emptyForm);

  const loadVendors = async () => {
    try {
      setLoading(true);
      const { data } = await vendorApi.getAll();
      setVendors(data);
    } catch { setError('Failed to load vendors'); }
    finally { setLoading(false); }
  };

  useEffect(() => { loadVendors(); }, []);

  const handleSave = async () => {
    try {
      if (editId) await vendorApi.update(editId, form);
      else await vendorApi.create(form);
      setDialogOpen(false); setForm(emptyForm); setEditId(null); loadVendors();
    } catch { setError('Failed to save vendor'); }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this vendor?')) return;
    try { await vendorApi.delete(id); loadVendors(); }
    catch { setError('Failed to delete vendor'); }
  };

  const openEdit = (v) => {
    setEditId(v.id);
    setForm({ name: v.name, contactEmail: v.contactEmail, contactPhone: v.contactPhone, address: v.address, status: v.status });
    setDialogOpen(true);
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4" sx={{ color: '#00f0ff' }}>Vendors</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => { setEditId(null); setForm(emptyForm); setDialogOpen(true); }}>
          Add Vendor
        </Button>
      </Box>
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {loading ? <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box> : (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Phone</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {vendors.map((v) => (
                <TableRow key={v.id} hover>
                  <TableCell>{v.name}</TableCell>
                  <TableCell>{v.contactEmail}</TableCell>
                  <TableCell>{v.contactPhone}</TableCell>
                  <TableCell><Chip label={v.status} color={v.status === 'Active' ? 'success' : 'default'} size="small" /></TableCell>
                  <TableCell align="center">
                    <IconButton size="small" onClick={() => openEdit(v)} sx={{ color: '#00f0ff' }}><Edit fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => handleDelete(v.id)} sx={{ color: '#ff3366' }}><Delete fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editId ? 'Edit Vendor' : 'Add Vendor'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '8px !important' }}>
          <TextField label="Name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
          <TextField label="Email" value={form.contactEmail} onChange={(e) => setForm({ ...form, contactEmail: e.target.value })} />
          <TextField label="Phone" value={form.contactPhone} onChange={(e) => setForm({ ...form, contactPhone: e.target.value })} />
          <TextField label="Address" value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} />
          {editId && (
            <FormControl>
              <InputLabel>Status</InputLabel>
              <Select value={form.status} label="Status" onChange={(e) => setForm({ ...form, status: e.target.value })}>
                <MenuItem value="Active">Active</MenuItem>
                <MenuItem value="Inactive">Inactive</MenuItem>
              </Select>
            </FormControl>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default VendorsPage;
