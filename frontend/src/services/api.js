import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

export const invitationService = {
  // Get all invitations
  getAll: () => api.get('/invitations'),

  // Get invitation by ID
  getById: (id) => api.get(`/invitations/${id}`),

  // Create new invitation
  create: (data) => api.post('/invitations', data),

  // Update invitation
  update: (id, data) => api.put(`/invitations/${id}`, data),

  // Delete invitation
  delete: (id) => api.delete(`/invitations/${id}`),

  // Upload image
  uploadImage: (formData) => api.post('/invitations/upload-image', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  }),

  // Upload music
  uploadMusic: (formData) => api.post('/invitations/upload-music', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

export const confirmationService = {
  // Submit confirmation
  submit: (invitationId, data) => api.post(`/confirmations/${invitationId}`, data),

  // Send confirmation email (without database)
  sendEmail: (data) => api.post('/confirmations/send-email', data)
}

export const guestListService = {
  // Upload Excel file with guest list
  uploadExcel: (invitationId, formData) => api.post(`/guestlist/upload/${invitationId}`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  }),

  // Get all guests for an invitation
  getGuests: (invitationId, page = 1, pageSize = 50) =>
    api.get(`/guestlist/${invitationId}?page=${page}&pageSize=${pageSize}`),

  // Get guest by phone
  getGuestByPhone: (invitationId, phone) =>
    api.get(`/guestlist/${invitationId}/phone/${phone}`),

  // Update guest
  updateGuest: (id, data) => api.put(`/guestlist/${id}`, data),

  // Delete guest
  deleteGuest: (id) => api.delete(`/guestlist/${id}`),

  // Add single guest
  addGuest: (invitationId, data) => api.post(`/guestlist/${invitationId}/guest`, data),

  // Clear all guests
  clearGuestList: (invitationId) => api.delete(`/guestlist/${invitationId}/clear`)
}

export default api
